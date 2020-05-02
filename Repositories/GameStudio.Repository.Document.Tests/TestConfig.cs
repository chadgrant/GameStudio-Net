using System;
using Microsoft.Extensions.Options;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests
{
    public class TestConfig
    {
        static TestConfig()
        {
            VanillaConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTIONSTRING");
            VanillaEnabled = !string.IsNullOrWhiteSpace(VanillaConnectionString);

            CosmosConnectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTIONSTRING");
            CosmosDbEnabled = !string.IsNullOrWhiteSpace(CosmosConnectionString);

            AwsDbConnectionString = Environment.GetEnvironmentVariable("AWSDB_CONNECTIONSTRING");
            AwsEnabled = !string.IsNullOrWhiteSpace(AwsDbConnectionString);

            if (string.IsNullOrWhiteSpace(CosmosConnectionString))
                CosmosConnectionString = "fake";

            if (string.IsNullOrWhiteSpace(VanillaConnectionString))
                VanillaConnectionString = "fake";

            if (string.IsNullOrWhiteSpace(AwsDbConnectionString))
                AwsDbConnectionString = "fake";
        }

        public static bool VanillaEnabled { get; }
        public static string VanillaConnectionString { get; }

        public static bool CosmosDbEnabled { get; }
        public static string CosmosConnectionString { get; }

        public static bool AwsEnabled { get; }
        public static string AwsDbConnectionString { get; }
        

        public static IOptions<MongoOptions> GetOptions(string connectionString, string includeIdField)//)"id")
        {
            var opt = new MongoOptions { ConnectionString = connectionString, AdditionalIdField = includeIdField };
            var options = Options.Create(opt);

            //var factory = new OptionsFactory<MongoOptions>(Enumerable.Empty<IConfigureOptions<MongoOptions>>(), Enumerable.Empty<IPostConfigureOptions<MongoOptions>>());
            //var cache = new OptionsCache<MongoOptions>();
            //cache.TryAdd(Options.DefaultName, opt);

            //var optionsMonitor = new OptionsMonitor<MongoOptions>(factory, Enumerable.Empty<IOptionsChangeTokenSource<MongoOptions>>(), cache);

            //var fto = Options.Create(new FaultTolerantDocumentRepositoryOptions());
            return options;
        }
    }
}
