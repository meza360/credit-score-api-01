namespace Infra.Env
{
    public class BancoUnionEnv
    {
        public static string PGSQL_BANCOUNION_DB { get; set; } = Environment.GetEnvironmentVariable("POSTGRES_BANCO_UNION_CONNECTION_STRING") ?? "Host=localhost;Port=5432;Username=PROJG2;Password=1873$$$JASashk;Database=banco_union";
    }
}