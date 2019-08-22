using Newtonsoft.Json;

namespace LightestNight.System.Serverless.AWS.ApiGateway.Authorization.Tokens
{
    public class JwksKey
    {
        /// <summary>
        /// The algorithm used to sign this key
        /// </summary>
        [JsonProperty("alg")]
        public string Algorithm { get; set; }
        
        /// <summary>
        /// The type of key
        /// </summary>
        [JsonProperty("kty")]
        public string Type { get; set; }
        
        /// <summary>
        /// The intended use of the public key
        /// </summary>
        [JsonProperty("use")]
        public string PublicKeyUse { get; set; }
        
        /// <summary>
        /// A chain of one or more PKIX Certificates
        /// </summary>
        [JsonProperty("x5c")]
        public string[] X509CertificateChain { get; set; }
        
        /// <summary>
        /// The RSA Public Modulus n
        /// </summary>
        [JsonProperty("n")]
        public string Modulus { get; set; }
        
        /// <summary>
        /// The RSA Public Exponent
        /// </summary>
        [JsonProperty("e")]
        public string Exponent { get; set; }
        
        /// <summary>
        /// The Identifier of the key
        /// </summary>
        [JsonProperty("kid")]
        public string KeyId { get; set; }
        
        /// <summary>
        /// The SHA-1 Thumbprint of the DER encoding of the x.509 Certificate
        /// </summary>
        [JsonProperty("x5t")]
        public string X509CertificateThumbprint { get; set; }
    }
}