namespace Infra.Env
{
    public class SatEnv
    {
        public static string PGSQL_SAT_DB { get; private set; } = Environment.GetEnvironmentVariable("POSTGRES_SAT_CONNECTION_STRING") ?? "Host=oddbbsrv01.gt2software.dev;Port=5432;Username=PROJG2;Password=1873$$$JASashk;Database=sat";
    }
}