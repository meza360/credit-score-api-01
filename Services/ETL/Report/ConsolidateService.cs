using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infra;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Services.Core;

namespace Services.ETL.Report
{
    public class ConsolidateService
    {
        private readonly Services.Querying.Sat.Contributor _contributorService;
        private readonly Services.Querying.EEGSA.Customer _eegsaCustomerService;
        private readonly Services.Querying.BancoUnion.Customer _bancoUnionCustomerService;
        private readonly Services.Querying.Renap.Citizen _citizenService;
        private readonly ILogger<ConsolidateService> _logger;
        private readonly CosmosContext _cosmosContext;
        private readonly IMongoDatabase _database;
        private readonly string _databaseName;
        public ConsolidateService(
            ILogger<ConsolidateService> logger,
            CosmosContext cosmosContext,
            Services.Querying.Sat.Contributor contributorService,
            Services.Querying.EEGSA.Customer eegsaCustomerService,
            Services.Querying.BancoUnion.Customer bancoUnionCustomerService,
            Services.Querying.Renap.Citizen citizenService)
        {
            this._contributorService = contributorService;
            this._eegsaCustomerService = eegsaCustomerService;
            this._bancoUnionCustomerService = bancoUnionCustomerService;
            this._citizenService = citizenService;
            this._logger = logger;
            this._cosmosContext = cosmosContext;
            this._databaseName = CosmosEnv.COSMOS_REPORT_DB;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<List<Domain.NoSQL.Report.EntityReport>?>> Consolidate(HttpRequestData req)
        {
            List<Domain.NoSQL.Report.EntityReport>? consolidatedReports = new List<Domain.NoSQL.Report.EntityReport>();
            Result<List<Domain.Relational.Renap.Citizen>> citizens = await _citizenService.ListAll(req);
            citizens
            .Value?
            .ToList()
            .ForEach(async (citizen) =>
            {
                Result<Domain.NoSQL.SAT.Contributor?> contributor = await _contributorService.GetContributorReportByCui(citizen.Cui);
                Result<Domain.NoSQL.Private.EEGSA.EEGSSACustomer?> eegsaCustomer = await _eegsaCustomerService.GetCustomersReportByCui(citizen.Cui);
                Result<Domain.NoSQL.Bank.BancoUnionCustomerETL?> bancoUnionCustomer = await _bancoUnionCustomerService.GetCustomerReportById("cui", citizen.Cui);

                List<Domain.NoSQL.Report.TaxOverall> taxOveralls = new List<Domain.NoSQL.Report.TaxOverall>();
                contributor?
                .Value?
                .StatementHistoricalRecord
                .ToList()
                .ForEach(statement =>
                {
                    taxOveralls.Add(new Domain.NoSQL.Report.TaxOverall
                    {
                        Year = statement.Year,
                        Month = statement.Month,
                        Total = statement.StatementAmount
                    });
                });

                List<Domain.NoSQL.Report.PurchaseOverall> purchaseOveralls = new List<Domain.NoSQL.Report.PurchaseOverall>();
                contributor?
                .Value?
                .ImpositionHistoricalRecord
                .ToList()
                .ForEach(imposition =>
                {
                    purchaseOveralls.Add(new Domain.NoSQL.Report.PurchaseOverall
                    {
                        Year = imposition.Year,
                        Month = imposition.Month,
                        Total = imposition.PaymentAmount
                    });
                });

                List<Domain.NoSQL.Report.BankPaymentOverall> bankPaymentsOveralls = new List<Domain.NoSQL.Report.BankPaymentOverall>();
                bancoUnionCustomer?
                .Value?
                .LoanHistoricalRecord?
                .ToList()
                .ForEach(loan =>
                {
                    bankPaymentsOveralls.Add(new Domain.NoSQL.Report.BankPaymentOverall
                    {
                        Year = loan.Year,
                        Month = loan.Month,
                        Total = loan.Amount
                    });
                });
                bancoUnionCustomer?
                .Value?
                .CreditHistoricalRecord?
                .ToList()
                .ForEach(credit =>
                {
                    bankPaymentsOveralls.Add(new Domain.NoSQL.Report.BankPaymentOverall
                    {
                        Year = credit.Year,
                        Month = credit.Month,
                        Total = credit.Amount
                    });
                });

                Domain.NoSQL.Report.EntityReport consolidatedReport = new Domain.NoSQL.Report.EntityReport
                {
                    FullName = $"{citizen.FirstName} {citizen.LastName}",
                    Cui = citizen.Cui,
                    Nit = contributor?.Value?.Nit ?? citizen.Cui,
                    PrivateOverallScore = "A",
                    BankOverallScore = "A",
                    TaxOverallScore = "A",
                    MonthlyOutcome = bankPaymentsOveralls.Sum(b => b.Total) + purchaseOveralls.Sum(p => p.Total),
                    AccumulatedDebt = (contributor?.Value?.AccumulatedDebt ?? 0) + (eegsaCustomer?.Value?.AccumulatedDebt ?? 0) + (bancoUnionCustomer?.Value?.AccumulatedDebt ?? 0),
                    TaxOverallHistory = taxOveralls,
                    PurchaseOverallHistory = purchaseOveralls,
                    BankPaymentOverallHistory = bankPaymentsOveralls
                };
                await SaveETLReportConsolidated(consolidatedReport);
                consolidatedReports.Add(consolidatedReport);
            });

            return Result<List<Domain.NoSQL.Report.EntityReport>?>.Success(consolidatedReports);
        }
        private async Task<bool> SaveETLReportConsolidated(Domain.NoSQL.Report.EntityReport consolidatedReport)
        {
            await _database.GetCollection<Domain.NoSQL.Report.EntityReport>(CosmosEnv.COSMOS_REPORT_DB_COL)
                                                            .InsertOneAsync(consolidatedReport);
            return true;
        }
    }
}