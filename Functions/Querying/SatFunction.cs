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

        /// <summary>
        /// This function is used to list all contributors with no extra level of detail
        /// </summary>
        /// <param name="req"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Function(nameof(ContributorList))]
        public async Task<HttpResponseData> ContributorList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/contributor")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.SAT.Contributor>>(await _contributorService.ListAll(req), req, context);
        }

        /// <summary>
        /// This function is used to list all contributors with impositions
        /// </summary>
        /// <param name="req"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Function(nameof(ContributorListWithImpositions))]
        public async Task<HttpResponseData> ContributorListWithImpositions(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/contributor/impositions")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.SAT.Contributor>>(await _contributorService.ListAllContributorsWithImposition(req), req, context);
        }

        /// <summary>
        /// This function is used to list all contributors with impositions
        /// </summary>
        /// <param name="req"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Function(nameof(ContributorListReport))]
        public async Task<HttpResponseData> ContributorListReport(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/contributor/credit-score")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<Domain.NoSQL.SAT.Contributor>(await _contributorService.ListAllContributorsReport(req), req, context);
        }

    }
}