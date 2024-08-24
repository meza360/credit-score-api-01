using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.ETL.Bank
{
    public class BancoUnionETLFunction
    {
        private const string _basePath = "v1/etl/bank/banco-union";
        [Function(nameof(Hello))]
        public IActionResult Hello([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = $"{_basePath}/hello")] HttpRequestData req)
        {
            return new OkObjectResult("Hello, World!");
        }
    }
}