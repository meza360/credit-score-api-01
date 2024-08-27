using Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Querying
{
    public class SatFunction : BaseFunction
    {
        private readonly Services.Querying.Sat.Contributor _contributorService;
        private const string basePath = "v1/querying/sat";
        public SatFunction(Services.Querying.Sat.Contributor service)
        {
            _contributorService = service;
        }

        [Function(nameof(ContributorList))]
        public async Task<HttpResponseData> ContributorList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/contributor")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.SAT.Contributor>>(await _contributorService.ListAll(req), req, context);
        }

    }
}