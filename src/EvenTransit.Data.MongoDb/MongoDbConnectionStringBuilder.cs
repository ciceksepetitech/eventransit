using System.Text;
using EvenTransit.Data.MongoDb.Settings;
using Microsoft.Extensions.Options;

namespace EvenTransit.Data.MongoDb;

public class MongoDbConnectionStringBuilder
{
    private const string EmptyCredentialName = "<no value>";
    private const int Port = 27017;

    public string ConnectionString { get; set; }

    public MongoDbConnectionStringBuilder(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var connectionString = new StringBuilder("mongodb://");
        var hasCredentials = !string.IsNullOrEmpty(mongoDbSettings.Value.UserName) &&
                             !string.IsNullOrEmpty(mongoDbSettings.Value.Password) &&
                             mongoDbSettings.Value.UserName != EmptyCredentialName;
        
        if (hasCredentials)
            connectionString.AppendFormat("{0}:{1}@", mongoDbSettings.Value.UserName, mongoDbSettings.Value.Password);

        connectionString.Append(mongoDbSettings.Value.Host);

        var port = Port;
        if (mongoDbSettings.Value.Port != default)
            port = mongoDbSettings.Value.Port;

        connectionString.AppendFormat(":{0}", port);

        ConnectionString = connectionString.ToString();
    }
}
