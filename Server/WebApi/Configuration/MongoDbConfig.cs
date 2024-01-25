namespace WebApi.Configuration;

public class MongoDbConfig
{
    public static string MongoDb = "MongoDb";
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}