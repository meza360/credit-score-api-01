using System.Collections.Specialized;
using Infra;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Services.Core;

namespace Services.Querying.Sat
{
    public class Contributor
    {
        private Persistence.Relational.SatContext _context;
        private readonly ILogger<Contributor> logger;
        private readonly CosmosContext cosmosContext;
        private readonly IMongoDatabase _database;
        private readonly string _databaseName;

        public Contributor(Persistence.Relational.SatContext repository, ILogger<Services.Querying.Sat.Contributor> logger, CosmosContext _cosmosContext)
        {
            this._context = repository;
            this.logger = logger;
            this._databaseName = CosmosEnv.COSMOS_SAT_DB;
            this.cosmosContext = _cosmosContext;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<List<Domain.Relational.SAT.Contributor>>> ListAll(HttpRequestData req)
        {
            return Result<List<Domain.Relational.SAT.Contributor>>.Success(await _context.Contributors.ToListAsync());
        }

        public async Task<Result<List<Domain.Relational.SAT.Contributor>>> ListAllContributorsWithImposition(HttpRequestData req)
        {
            return Result<List<Domain.Relational.SAT.Contributor>>
            .Success(
                await _context.Contributors?
                .Include(c => c.Impositions)
                .ToListAsync()
                );
        }

        public async Task<Result<List<Domain.Relational.SAT.Contributor>>> ListAllContributorsWithImposition()
        {
            return Result<List<Domain.Relational.SAT.Contributor>>
            .Success(
                await _context.Contributors?
                .Include(c => c.Statements).Include(c => c.Impositions)
                .ToListAsync()
                );
        }

        public async Task<Result<Domain.NoSQL.SAT.Contributor>> ListAllContributorsReport(HttpRequestData req)
        {
            string? searchMethod = "";
            string? searchValue = "";
            NameValueCollection query = req.Query;
            foreach (string? key in query.AllKeys)
            {
                string[]? values = query.GetValues(key);
                if (values?.Length > 0)
                {
                    foreach (string value in values)
                    {
                        System.Console.WriteLine($"{key}: {value}");
                        if (key == "nit")
                        {
                            searchValue = query?.Get("nit");
                            searchMethod = "nit";
                            return Result<Domain.NoSQL.SAT.Contributor>
                            .Success(
                                await _database.GetCollection<Domain.NoSQL.SAT.Contributor>("contributors")
                                .FindAsync(c => c.NIT == searchValue).Result.FirstOrDefaultAsync()
                            );
                        }
                        if (key == "cui")
                        {
                            searchValue = query?.Get("cui");
                            searchMethod = "cui";
                            return Result<Domain.NoSQL.SAT.Contributor>
                           .Success(
                               await _database.GetCollection<Domain.NoSQL.SAT.Contributor>("contributors")
                               .FindAsync(c => c.CUI == searchValue).Result.FirstOrDefaultAsync()
                            );
                        }
                    }
                }
            }

            return Result<Domain.NoSQL.SAT.Contributor>
            .Failure("No search method provided");
        }
    }
}