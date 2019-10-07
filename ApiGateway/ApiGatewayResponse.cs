using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace LightestNight.System.Serverless.AWS.ApiGateway
{
    public static class ApiGatewayResponse
    {
        /// <summary>
        /// Creates an API Response of the correct format for AWS API Gateway
        /// </summary>
        /// <param name="statusCode">The <see cref="HttpStatusCode" /> to return in the response</param>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /></returns>
        public static APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode)
            => CreateResponse<object>(statusCode);
        
        /// <summary>
        /// Creates an API Response of the correct format for AWS API Gateway
        /// </summary>
        /// <param name="statusCode">The <see cref="HttpStatusCode" /> to return in the response</param>
        /// <param name="responseData">The data to include within the response body</param>
        /// <typeparam name="TResponse">The type of the data included within the response body</typeparam>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /></returns>
        public static APIGatewayProxyResponse CreateResponse<TResponse>(HttpStatusCode statusCode, TResponse responseData = default)
            => new APIGatewayProxyResponse
            {
                StatusCode = (int) statusCode,
                Body = EqualityComparer<TResponse>.Default.Equals(responseData, default) 
                    ? null
                    : typeof(TResponse) == typeof(string)
                      ? responseData.ToString()
                      : JsonConvert.SerializeObject(responseData)
            };

        /// <summary>
        /// Creates a No Content API Response in the correct format for AWS API Gateway
        /// </summary>
        /// <remarks>Http Status Code 204</remarks>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /> with a NoContent status code</returns>
        public static APIGatewayProxyResponse NoContent()
            => CreateResponse(HttpStatusCode.NoContent);

        /// <summary>
        /// Creates an OK API Response in the correct format for AWS API Gateway
        /// </summary>
        /// <param name="responseData">The data to include within the response body</param>
        /// <typeparam name="TResponse">The type of the data included within the response body</typeparam>
        /// <remarks>Http Status Code 200</remarks>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /> with an OK status code</returns>
        public static APIGatewayProxyResponse Ok<TResponse>(TResponse responseData = default)
            => CreateResponse(HttpStatusCode.OK, responseData);

        /// <summary>
        /// Creates an Internal Server Error Response in the correct format for AWS API Gateway
        /// </summary>
        /// <param name="ex">The exception that has been caught</param>
        /// <remarks>Http Status Code 500</remarks>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /> with an Internal Server Error status code</returns>
        public static APIGatewayProxyResponse InternalServerError(Exception ex)
            => CreateResponse(HttpStatusCode.InternalServerError, new
            {
                ex.Message,
                ex.StackTrace,
                ex.InnerException,
                ex.Source
            });

        /// <summary>
        /// Creates a Conflict Response in the correct format for AWS API Gateway
        /// </summary>
        /// <param name="message">Any message to include in the response body</param>
        /// <remarks>Http Status Code 409</remarks>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /> with a Conflict status code</returns>
        public static APIGatewayProxyResponse Conflict(string message)
            => CreateResponse(HttpStatusCode.Conflict, message);

        /// <summary>
        /// Creates a Not Found Response in the correct format for AWS API Gateway
        /// </summary>
        /// <param name="message">Any message to include in the response body</param>
        /// <remarks>Http Status Code 404</remarks>
        /// <returns>A new instance of <see cref="APIGatewayProxyResponse" /> with a Not Found status code</returns>
        public static APIGatewayProxyResponse NotFound(string message)
            => CreateResponse(HttpStatusCode.NotFound, message);
    }
}