namespace Infra.Env
{
    public class EEGSAEnv
    {
        public static string PGSQL_EEGSA_DB { get; private set; } = Environment.GetEnvironmentVariable("POSTGRES_EEGSA_CONNECTION_STRING") ?? "Host=oddbbsrv01.gt2software.dev;Port=5432;Username=PROJG2;Password=1873$$$JASashk;Database=eegsa";
    }

}