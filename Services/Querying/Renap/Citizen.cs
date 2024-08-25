using Microsoft.EntityFrameworkCore;
using Domain.Relational.Renap;
using Persistence.Relational;
using Services.Core;
using Microsoft.Azure.Functions.Worker.Http;

namespace Services.Querying.Renap
{
    public class Citizen
    {
        private RenapContext _context;

        public Citizen(RenapContext _context)
        {
            this._context = _context;
        }

        public async Task<Result<List<Domain.Relational.Renap.Citizen>>> ListAll(HttpRequestData req)
        {
            var citizens = await _context.Citizens.ToListAsync();
            return Result<List<Domain.Relational.Renap.Citizen>?>.Success(citizens);
        }
    }
}