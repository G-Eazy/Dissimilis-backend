using System;
using System.Net;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace Dissimilis.WebAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly TelemetryClient _apITelemetryClient;

        public ErrorHandlingMiddleware(RequestDelegate next,
            TelemetryClient apITelemetryClient)
        {
            this.next = next;
            _apITelemetryClient = apITelemetryClient;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            const int internalError = (int)HttpStatusCode.InternalServerError;
            const int badRequest = (int)HttpStatusCode.BadRequest;
            const int notFound = (int)HttpStatusCode.NotFound;
            const int unauthorized = (int)HttpStatusCode.Unauthorized;
            const int forbidden = (int)HttpStatusCode.Forbidden;
            int statusKode;
            string errorMessage;
            var callStack = default(string);

            if (exception is InvalidOperationException)
            {
                statusKode = internalError;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is InternalErrorException)
            {
                statusKode = internalError;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is NotFoundException)
            {
                statusKode = notFound;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is BadHttpRequestException)
            {
                statusKode = badRequest;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusKode = unauthorized;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is ForbiddenException)
            {
                statusKode = forbidden;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is NullReferenceException)
            {
                statusKode = internalError;
                errorMessage = exception.Message;
                callStack = exception.StackTrace;
                _apITelemetryClient.TrackException(exception);
            }
            else if (exception is OverflowException)
            {
                statusKode = internalError;
                errorMessage = exception.Message;
                callStack = exception.StackTrace;
                _apITelemetryClient.TrackException(exception);
            }
            else
            {
                statusKode = internalError;
                errorMessage = exception.Message;
                _apITelemetryClient.TrackException(exception);
            }

            var test = new
            {
                ErrorMessage = errorMessage,
                StatusKode = statusKode,
                CallStack = callStack,
                request = (string)context.Request.Path,
                httpVerb = context.Request.Method
            };
            var resultToConsole = JsonConvert.SerializeObject(test, Formatting.Indented);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusKode;

            Console.WriteLine(resultToConsole);

            var resultToClient = JsonConvert.SerializeObject(new { errorMessage = exception.Message });
            return context.Response.WriteAsync(resultToClient);
        }
    }
}