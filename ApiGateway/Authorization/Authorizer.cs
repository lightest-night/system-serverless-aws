using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using LightestNight.System.Api;
using LightestNight.System.Configuration;
using LightestNight.System.Serverless.AWS.ApiGateway.Authorization.Exceptions;
using LightestNight.System.Serverless.AWS.ApiGateway.Authorization.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace LightestNight.System.Serverless.AWS.ApiGateway.Authorization
{
    public class Authorizer
    {
        private readonly IServiceProvider _serviceProvider;

        public Authorizer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<APIGatewayCustomAuthorizerResponse> Authorize(APIGatewayCustomAuthorizerRequest request)
        {
            try
            {
                var jwt = await DecodeJwt(GetToken(request));

                return new APIGatewayCustomAuthorizerResponse
                {
                    PrincipalID = jwt.Subject,
                    PolicyDocument = new APIGatewayCustomAuthorizerPolicy
                    {
                        Version = "2012-10-17",
                        Statement =
                        {
                            new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                            {
                                Action = new HashSet<string> {"execute-api:Invoke"},
                                Effect = "Allow",
                                Resource = new HashSet<string> {request.MethodArn}
                            }
                        }
                    }
                };
            }
            catch
            {
                // TODO: LOGGER
                throw new UnauthorizedException();
            }
        }

        private static string GetToken(APIGatewayCustomAuthorizerRequest request)
        {
            const string tokenType = "TOKEN";
            var requestType = request.Type;
            if (string.IsNullOrEmpty(requestType) || requestType != tokenType)
                throw new TokenValidationException($"Expected {nameof(request.Type)} parameter to have value {tokenType}");

            var token = request.AuthorizationToken;
            if (string.IsNullOrEmpty(token))
                throw new TokenValidationException($"Expected {nameof(request.AuthorizationToken)} parameter to have value");

            var tokenMatch = Regex.Match(token, "^Bearer (.*)$");
            if (tokenMatch.Length != 2)
                throw new TokenValidationException($"Expected {nameof(request.AuthorizationToken)} parameter to have value Bearer .*");

            return tokenMatch.Groups[1].Value;
        }

        private async Task<JwtSecurityToken> DecodeJwt(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(jwt);
            if (jwtToken?.Header == null || string.IsNullOrEmpty(jwtToken.Header.Kid))
                throw new TokenValidationException();

            var configurationManager = _serviceProvider.GetRequiredService<ConfigurationManager>();
            var configuration = configurationManager.Bind<AuthorizationConfiguration>();

            var jwtConfiguration = await GetJwtConfiguration(configuration);
            var jwks = await GetJwks(new Uri(jwtConfiguration.jwks_uri.ToString()), configuration);
            var signingKey = jwks.Keys.FirstOrDefault(key => key.KeyId == jwtToken.Header.Kid);

            return ValidateToken(jwt, signingKey);
        }
        
        private static async Task<dynamic> GetJwtConfiguration(AuthorizationConfiguration configuration)
        {
            var restClient = new CoreClient(configuration.IssuerUrl);
            var configurationRequest = new RestRequest(configuration.OpenIdConfigurationResource, Method.GET);
            return (await restClient.MakeRequest<dynamic>(configurationRequest, true, false)).Data;
        }
        
        private static async Task<Jwks> GetJwks(Uri jwksUri, AuthorizationConfiguration configuration)
        {
            var restClient = new CoreClient(configuration.IssuerUrl);
            var jwksRequest = new RestRequest(jwksUri.PathAndQuery, Method.GET);
            return (await restClient.MakeRequest<Jwks>(jwksRequest, true, false)).Data;
        }

        private static JwtSecurityToken ValidateToken(string jwt, JwksKey key)
        {
            byte[] FromBase64Url(string base64Url)
            {
                var result = string.Empty;

                if (string.IsNullOrEmpty(base64Url))
                    return Convert.FromBase64String(result);

                var padded = base64Url.Length % 4 == 0
                    ? base64Url
                    : $"{base64Url}{"====".Substring(base64Url.Length % 4)}";

                result = padded.Replace("_", "/").Replace("-", "+");

                return Convert.FromBase64String(result);
            }

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(new RSAParameters
            {
                Modulus = FromBase64Url(key.Modulus),
                Exponent = FromBase64Url(key.Exponent)
            });

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                // TODO: Should probably validate audience, but only when on a custom domain
                ValidateAudience = false,
                // TODO: Should probably validate issuer, but only when on a custom domain
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };
            
            var handler = new JwtSecurityTokenHandler();
            SecurityToken validatedSecurityToken;
            
            try
            {
                handler.ValidateToken(jwt, validationParameters, out validatedSecurityToken);
            }
            catch (Exception ex)
            {
                throw new TokenValidationException(ex);
            }
            
            return validatedSecurityToken as JwtSecurityToken;
        }
    }
}