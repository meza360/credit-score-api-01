using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infra;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Services.Core;
using Services.Querying.EEGSA;

namespace Services.ETL.Private
{
    public class EEGSACustomerETL
    {
        private readonly Services.Querying.EEGSA.Customer _customerService;
        private readonly ILogger<EEGSACustomerETL> _logger;
        private readonly CosmosContext _cosmosContext;
        private readonly string _databaseName;
        private readonly IMongoDatabase _database;

        public EEGSACustomerETL(Services.Querying.EEGSA.Customer customerService, ILogger<EEGSACustomerETL> logger, CosmosContext cosmosContext)
        {
            this._customerService = customerService;
            this._logger = logger;
            this._cosmosContext = cosmosContext;
            this._databaseName = CosmosEnv.COSMOS_EEGSA_DB;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>?>> TransformToMonthlyBills(HttpRequestData req)
        {
            List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>? customersTransformed = new List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>();
            Result<List<Domain.Relational.EEGSA.Customer>> customers = await _customerService.ListAllWithContractsL2(req);

            if (customers == null || customers?.Value?.Count == 0)
            {
                return Result<List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>?>
                    .Failure("No customers found");
            }
            _logger.LogInformation($"Customers found: {customers?.Value?.Count}");
            //Process each customer to transform into ETL model
            customers?
            .Value?
            .ForEach((customer) =>
                {
                    List<Domain.Relational.EEGSA.Contract>? contracts = customer?.Contracts;
                    List<Domain.NoSQL.Private.HistoricalRecord> customerHistoricalRecords = new List<Domain.NoSQL.Private.HistoricalRecord>();
                    double calculatedDebt = 0;
                    _logger.LogDebug($"Customer {customer?.Cui} has: {contracts?.Count} contracts");
                    if (contracts != null || contracts?.Count > 0)
                    {
                        contracts?
                        .Where(c => c?.Bills != null || c?.Bills?.Count > 0)
                        .ToList()
                        .ForEach((contract) =>
                        {
                            _logger.LogDebug($"Contract {contract?.Id} has: {contract?.Bills?.Count} bills");
                            if (contract?.Bills != null || contract?.Bills?.Count > 0)
                            {
                                contract?
                                .Bills?
                                .Where(b => b?.ContractId != null || b?.Payments?.Count > 0 && b?.Payments != null && b != null)
                                .OrderBy(b => b.IssueDate)
                                .ToList()
                                .ForEach(bill =>
                                {
                                    if (bill.BillOverdue)
                                    {
                                        calculatedDebt += bill.BillAmount - (bill?.Payments?.Where(p => p?.BillId == bill?.Id).Sum(p => p?.PaymentAmount) ?? 0);
                                    }
                                    customerHistoricalRecords.Add(
                                        new Domain.NoSQL.Private.HistoricalRecord
                                        {
                                            Month = bill.IssueDate.Month,
                                            Year = bill.IssueDate.Year,
                                            WasDue = bill.BillOverdue,
                                            DaysDue = bill.DaysOverdue
                                        }
                                    );
                                });
                            }
                        });
                    }

                    customersTransformed.Add(
                        new Domain.NoSQL.Private.EEGSA.EEGSSACustomer
                        {
                            FullName = $"{customer?.FirstName} {customer?.LastName}",
                            Cui = customer?.Cui,
                            LastUpdate = DateTime.Now,
                            PrivateScore = "0",
                            AccumulatedDebt = calculatedDebt,
                            HistoricalRecord = customerHistoricalRecords
                        }
                    );
                });
            bool isFinished = await SaveETLCustomer(customersTransformed);
            if (!isFinished)
            {
                return Result<List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>?>
                    .Failure("Error saving customers etl");
            }
            return Result<List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>?>
                    .Success(customersTransformed?.Count > 0 ? customersTransformed : null);
        }

        public async Task<bool> SaveETLCustomer(List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer> customers)
        {
            // saves contributor transformed data into CosmosDB
            await _database.GetCollection<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>(CosmosEnv.COSMOS_EEGSA_CX_COL)
            .InsertManyAsync(customers);
            return true;
        }
    }
}