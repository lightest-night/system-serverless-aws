namespace LightestNight.System.Serverless.AWS.ApiGateway.Authorization.Tokens
{
    public class Jwks
    {
        /// <summary>
        /// The keys used for signing requests
        /// </summary>
        public JwksKey[] Keys { get; set; }
    }
}