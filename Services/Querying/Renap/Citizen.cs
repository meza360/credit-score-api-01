using Microsoft.EntityFrameworkCore;
using Domain.Relational.Renap;
using Persistence.Relational;
using Services.Core;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Specialized;

namespace Services.Querying.Renap
{
    public class Citizen
    {
        private RenapContext _context;

        public Citizen(RenapContext _context)
        {
            this._context = _context;
        }

        public async Task<Result<List<Domain.Relational.Renap.Citizen>?>> ListAll(HttpRequestData req)
        {
            var citizens = await _context.Citizens.ToListAsync();
            return Result<List<Domain.Relational.Renap.Citizen>?>.Success(citizens);
        }

        public async Task<Result<Domain.Relational.Renap.Citizen?>> GetCitizenByCui(HttpRequestData req)
        {
            // extract query params
            NameValueCollection query = req.Query;
            //display the query parameters
            dynamic query1 = "";
            foreach (string? key in query.AllKeys)
            {
                string[]? values = query?.GetValues(key);
                if (values?.Length > 0)
                {
                    foreach (string value in values)
                    {
                        System.Console.WriteLine($"{key}: {value}");
                        if (key == "cui")
                        {
                            return Result<Domain.Relational.Renap.Citizen?>
                                .Success(await _context.Citizens
                                            .Where(c => c.Cui == value)
                                            .FirstOrDefaultAsync());
                        }
                    }
                }
            }
            return Result<Domain.Relational.Renap.Citizen?>
                                .Failure("No CUI provided");
        }
    }
}