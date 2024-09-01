using System.Collections.Specialized;
using Domain.Relational.EEGSA;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Relational;
using Services.Core;

namespace Services.Querying.EEGSA
{
    public class Customer
    {
        private EEGSAContext _context;

        public Customer(Persistence.Relational.EEGSAContext _context)
        {
            this._context = _context;
        }

        public async Task<List<Domain.Relational.EEGSA.Customer>> QueryHandler(HttpRequestData req, DbSet<Domain.Relational.EEGSA.Customer> set)
        {
            //IIncludableQueryable<Domain.Relational.EEGSA.Customer, List<Domain.Relational.EEGSA.Contract>> ss = set.Include(c => c.Contracts);
            var ss = set.Include(c => c.Contracts);
            List<IIncludableQueryable<Domain.Relational.EEGSA.Customer, List<Domain.Relational.EEGSA.Contract>>> queries = new List<IIncludableQueryable<Domain.Relational.EEGSA.Customer, List<Domain.Relational.EEGSA.Contract>>>();
            //var queries;
            // extract query parameters from the request
            NameValueCollection query = req.Query;
            //display the query parameters
            dynamic query1 = "";
            foreach (string key in query.AllKeys)
            {
                string[] values = query.GetValues(key);
                if (values.Length > 0)
                {
                    foreach (string value in values)
                    {
                        System.Console.WriteLine($"{key}: {value}");
                        if (key == "include")
                        {

                            if (value == "contracts")
                            {
                                query1 = set.Include(c => c.Contracts).AsQueryable();
                            }
                            if (value == "bills")
                            {
                                query1 = set.Include(c => c.Contracts).ThenInclude(cont => cont.Bills).AsQueryable();
                            }

                        }
                    }
                }
            }
            return await query1.ToListAsync();
        }

        /// <summary>
        /// Lists all customers in the EEGSA database
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A list of customers with basic information</returns>
        public async Task<Result<List<Domain.Relational.EEGSA.Customer>>> ListAll(HttpRequestData req)
        {
            //var customers = await QueryHandler(req, _context.Customers);
            var customers = await _context.Customers.ToListAsync();
            return Result<List<Domain.Relational.EEGSA.Customer>?>.Success(customers);
        }

        /// <summary>
        /// Lists all customers in the EEGSA database with their contracts
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A list of customers with all active and inactive contracts</returns>
        public async Task<Result<List<Domain.Relational.EEGSA.Customer>>> ListAllWithContractsL1(HttpRequestData req)
        {
            var customersWithContracts = await _context.Customers
                .Include(c => c.Contracts)
                .ToListAsync();
            return Result<List<Domain.Relational.EEGSA.Customer>?>.Success(customersWithContracts);
        }

        /// <summary>
        /// Lists all customers in the EEGSA database with their contracts and bills in basic form
        /// </summary>
        /// <param name="req"></param>
        /// <returns>A list of customers with all active and inactive contracts and their bills</returns>
        public async Task<Result<List<Domain.Relational.EEGSA.Customer>>> ListAllWithContractsL2(HttpRequestData req)
        {
            //QueryHandler(req);
            var customersWithContractsAndBills = await _context.Customers
                .Include(cust => cust.Contracts)
                .ThenInclude(cont => cont.Bills)
                .ToListAsync();
            return Result<List<Domain.Relational.EEGSA.Customer>?>.Success(customersWithContractsAndBills);
        }

    }
}