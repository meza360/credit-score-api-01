using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                double monthlyOutcomeFromTributaryImpositions = 0;
                double monthlyOutcomeFromTributaryStatements = 0;
                double totalOutcomeFromPaymentsTributaryImpositions = 0;
                double totalOutcomeFromPaymentsTributaryStatements = 0;
                double totalOutcomeFromCreditPayments = 0;
                double totalOutcomeFromLoanPayments = 0;
                double monthlyOutcomeFromBankPayments = 0;
                int bankCreditPayments = 0;
                int bankLoanPayments = 0;
                int tributaryImpositions = 0;
                int tributaryStatements = 0;
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


                _logger.LogInformation($"Citizen and contributor {citizen.FirstName} {citizen.LastName}");
                taxOveralls
                .GroupBy(t => new { t.Year, t.Month })
                .ToList()
                .ForEach(t =>
                {
                    tributaryStatements++;
                    totalOutcomeFromPaymentsTributaryStatements += t.Sum(t => t.Total);
                    //_logger.LogInformation($"Year: {t.Key.Year}, Month: {t.Key.Month}, Total: {t.Sum(t => t.Total)}");
                });

                _logger.LogInformation($"Total tributary statements: {tributaryStatements}");
                monthlyOutcomeFromTributaryStatements = tributaryStatements > 0 ? (totalOutcomeFromPaymentsTributaryStatements / tributaryStatements) : 0;


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

                purchaseOveralls
                .GroupBy(t => new { t.Year, t.Month })
                .ToList()
                .ForEach(t =>
                {
                    tributaryImpositions++;
                    totalOutcomeFromPaymentsTributaryImpositions += t.Sum(t => t.Total);
                    //_logger.LogInformation($"Year: {t.Key.Year}, Month: {t.Key.Month}, Total: {t.Sum(t => t.Total)}");
                });
                _logger.LogInformation($"Total tributary statements: {tributaryStatements}");
                monthlyOutcomeFromTributaryImpositions = tributaryImpositions > 0 ? (totalOutcomeFromPaymentsTributaryImpositions / tributaryImpositions) : 0;
                _logger.LogInformation($"Monthly mean outcome from tributary impositions: {monthlyOutcomeFromTributaryImpositions}");
                _logger.LogInformation($"Monthly mean outcome from tributary statements: {monthlyOutcomeFromTributaryStatements}");
                _logger.LogInformation($"Total outcome from tributary obligations: {monthlyOutcomeFromTributaryImpositions * monthlyOutcomeFromTributaryStatements}");

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
                        Total = loan.Amount,
                        Type = "Loan installment payment"
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
                        Total = credit.Amount,
                        Type = "Credit payment"
                    });
                });

                _logger.LogInformation($"Consolidating report for {citizen.FirstName} {citizen.LastName}");
                // counts loan installments
                bankPaymentsOveralls
                .GroupBy(b => new { b.Year, b.Month, b.Type })
                .Where(b => b.Key.Type == "Loan installment payment")
                .Select(b => new Domain.NoSQL.Report.BankPaymentOverall
                {
                    Year = b.Key.Year,
                    Month = b.Key.Month,
                    Total = b.Sum(t => t.Total),
                    Type = b.Key.Type
                })
                .ToList()
                .ForEach(b =>
                {
                    bankLoanPayments++;
                    totalOutcomeFromLoanPayments += b.Total;
                    _logger.LogInformation($"Year: {b.Year}, Month: {b.Month}, Total: {b.Total}");
                });
                // counts credit card payments
                bankPaymentsOveralls
                .GroupBy(b => new { b.Year, b.Month, b.Type })
                .Where(b => b.Key.Type == "Credit payment")
                .Select(b => new Domain.NoSQL.Report.BankPaymentOverall
                {
                    Year = b.Key.Year,
                    Month = b.Key.Month,
                    Total = b.Sum(t => t.Total),
                    Type = b.Key.Type
                })
                .ToList()
                .ForEach(b =>
                {
                    bankCreditPayments++;
                    totalOutcomeFromCreditPayments += b.Total;
                    _logger.LogInformation($"Year: {b.Year}, Month: {b.Month}, Total: {b.Total}");
                });

                _logger.LogInformation($"Consolidated bank payments for {citizen.FirstName} {citizen.LastName}");
                monthlyOutcomeFromBankPayments = (bankLoanPayments > 0 ? (totalOutcomeFromLoanPayments / bankLoanPayments) : 0) + (bankCreditPayments > 0 ? (totalOutcomeFromCreditPayments / bankCreditPayments) : 0);
                _logger.LogInformation($"Monthly outcome from bank payments: {monthlyOutcomeFromBankPayments}");

                Domain.NoSQL.Report.EntityReport consolidatedReport = new Domain.NoSQL.Report.EntityReport
                {
                    FullName = $"{citizen.FirstName} {citizen.LastName}",
                    Cui = citizen.Cui,
                    Nit = contributor?.Value?.Nit ?? citizen.Cui,
                    PrivateOverallScore = "A",
                    BankOverallScore = "A",
                    TaxOverallScore = "A",
                    MonthlyOutcome = monthlyOutcomeFromBankPayments + monthlyOutcomeFromTributaryImpositions + monthlyOutcomeFromTributaryStatements,
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