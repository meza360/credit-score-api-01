using Infra;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Persistence.NoSQL;

public class CosmosContext
{
    private readonly CosmosEnv _cosmosEnv;
    public CosmosContext(CosmosEnv cosmosEnv)
    {
        _cosmosEnv = cosmosEnv;
    }

    public MongoClient getCosmosClient()
    {
        MongoClientSettings settings = MongoClientSettings.FromConnectionString(_cosmosEnv.CosmosAcc);
        var camelCaseConvention = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
        var mongoClient = new MongoClient(settings);
        return mongoClient;

    }
}
