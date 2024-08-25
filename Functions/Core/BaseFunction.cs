using System.Net;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Services.Core;

namespace Functions.Core;

public class BaseFunction
{

    protected async Task<HttpResponseData> HandleResult<T>(Result<T> result, HttpRequestData req, FunctionContext context)
    {
        var s = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var serializer = new NewtonsoftJsonObjectSerializer(s);
        context.GetLogger("BaseFunction").LogInformation("Handling result");
        if (result == null)
        {
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            //response.Headers.Add("Content-Type", "application/json");
            await response.WriteAsJsonAsync<Result<T>>(result, serializer);
            return response;
        }
        if (result.IsSuccess && result.Value != null)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "application/json");
            await response.WriteAsJsonAsync<Result<T>>(result, serializer);
            return response;
        }
        if (result.IsSuccess && result.Value == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync<Result<T>>(result, serializer);
            return response;
        }
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }
}