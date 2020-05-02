using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests.Mongo
{
    public class SimpleEntityBsonMapper : BsonMapper<Guid,SimpleEntity>
    {
        static class Fields
        {
            public static string Name = "name";
        }

        public override BsonDocument Map(Document<Guid,SimpleEntity> document, BsonDocument bson)
        {
            bson.Add(Fields.Name, document.Item.Name);

            return base.Map(document, bson);
        }

        public override Document<Guid,SimpleEntity> Map(BsonDocument bson, Document<Guid,SimpleEntity> document)
        {
            document = base.Map(bson, document);

            if (bson.TryGetValue(Fields.Name, out var name))
                document.Item.Name = name.AsString;

            document.Item.Id = document.Id;

            return document;
        }

        public SimpleEntityBsonMapper(IOptions<MongoOptions> options) : base(options)
        {
        }
    }

    public class MongoSimpleEntityBsonRepository : MongoRepository<Guid,SimpleEntity>
    {
        public MongoSimpleEntityBsonRepository(IOptions<MongoOptions> options, string collection)
            : base(options, new SimpleEntityBsonMapper(options), collection)
        {
        }
    }
}
