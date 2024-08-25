using Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Persistence.Relational;

namespace Functions.Querying
{
    public class RenapFunction : BaseFunction
    {
        private readonly Services.Querying.Renap.Citizen _citizenService;
        private const string basePath = "v1/querying/renap";
        public RenapFunction(Services.Querying.Renap.Citizen service)
        {
            _citizenService = service;
        }

        [Function(nameof(CitizenList))]
        public async Task<HttpResponseData> CitizenList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/citizen")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.Renap.Citizen>>(await _citizenService.ListAll(req), req, context);
        }

    }
}