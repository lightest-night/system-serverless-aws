namespace LightestNight.System.Serverless.AWS.ApiGateway.Authorization
{
    public class AuthorizationConfiguration
    {
        /// <summary>
        /// The Absolute Url of the server issuing tokens
        /// </summary>
        public string IssuerUrl { get; set; }

        /// <summary>
        /// The Well Known Open Id Configuration Resource to use when getting details about a JWT
        /// </summary>
        /// <remarks>If following convention, this should be .well-known/openid-configuration and defaults to this</remarks>
        public string OpenIdConfigurationResource { get; set; } = ".well-known/openid-configuration";
    }
}