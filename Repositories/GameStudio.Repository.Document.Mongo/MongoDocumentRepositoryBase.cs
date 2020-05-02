using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace GameStudio.Repository.Document.Mongo
{
    public abstract class MongoDocumentRepositoryBase<TCollection>
    {
        protected const string DuplicateKeyError = "E11000";

        readonly IOptions<MongoOptions> _options;
        readonly string _defaultCollectionName;

        protected IMongoCollection<TCollection> _collection;
        protected IMongoDatabase _db;
        
        protected MongoDocumentRepositoryBase(IOptions<MongoOptions> options, string defaultCollectionName = null)
        {
            var val = options.Value;

            if (string.IsNullOrWhiteSpace(val.ConnectionString))
                throw new ArgumentNullException(nameof(val.ConnectionString));

            _options = options;
            _defaultCollectionName = defaultCollectionName;
        }
        
        protected virtual IMongoDatabase GetDatabase()
        {
            if (_db != null) return _db;

            try
            {
                var url = new MongoUrl(_options.Value.ConnectionString);
                var client = new MongoClient(MongoClientSettings.FromUrl(url));

                //BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

                ConventionRegistry.Register("IgnoreIfDefault",
                    new ConventionPack
                    {
                        new IgnoreIfDefaultConvention(true),
                        new CamelCaseElementNameConvention(),
                    },
                    t => true);

                return _db = client.GetDatabase(url.DatabaseName);
            }
            catch (MongoServerException mse)
            {
                throw new DocumentServerException($"GetDatabase() {_defaultCollectionName}", mse);
            }
        }

        protected virtual IMongoCollection<TCollection> GetCollection(string name = null)
        {
            var key = name ?? _defaultCollectionName;

            try
            {
                return _collection ?? (_collection = GetDatabase().GetCollection<TCollection>(key));
            }
            catch (MongoServerException mse)
            {
                throw new DocumentServerException($"GetCollection() {key}", mse);
            }
        }
    }
}
