using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Relational;
using Microsoft.EntityFrameworkCore;

namespace Functions;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(services =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddDbContext<BancoUnionContext>(options =>
                        options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING"))
                    );
                })
                .Build();

        await host.RunAsync();
    }

}