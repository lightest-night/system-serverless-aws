using System;

namespace LightestNight.System.Serverless.AWS.ApiGateway.Authorization.Exceptions
{
    public class TokenValidationException : Exception
    {
        private const string Text = "An error occurred validating the authorization token. See inner exception for more information.";
        public TokenValidationException() : base(Text){}
        public TokenValidationException(Exception inner) : base(Text, inner){}
        public TokenValidationException(string message) : base($"{Text}{Environment.NewLine}{message}"){}
        public TokenValidationException(string message, Exception inner) : base($"{Text}{Environment.NewLine}{message}", inner){}
    }
}