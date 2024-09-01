
using System.Net;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Functions.Core
{
    public class ExceptionMiddleware : IFunctionsWorkerMiddleware
    {

        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private readonly JsonSerializerSettings _serializeOptions;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _serializeOptions = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented,
                // camel case property names
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("\n\nError en la invocacion del metodo: {0}", ex.Message);
                _logger.LogError("\n\nStackTrace: {0}", ex.StackTrace);
                var req = await context.GetHttpRequestDataAsync();
                var newResponse = req.CreateResponse(HttpStatusCode.Conflict);
                var appException = _env.IsDevelopment()
                ? new AppException(newResponse.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new AppException(newResponse.StatusCode, "Server Error from MDW");
                var serializer = new NewtonsoftJsonObjectSerializer(_serializeOptions);
                await newResponse.WriteAsJsonAsync<AppException>(appException, serializer, newResponse.StatusCode);
                var inResult = context.GetInvocationResult();
                inResult.Value = newResponse;
            }
        }

    }
}