using Microsoft.EntityFrameworkCore;

namespace Persistence.Relational;

public class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {

    }


}