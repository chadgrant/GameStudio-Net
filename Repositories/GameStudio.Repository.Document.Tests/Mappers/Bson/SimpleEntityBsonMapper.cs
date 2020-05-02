using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests
{
    public static class SimpleDocumentFields
    {
        public const string Name = "name";
    }

    public class SimpleEntityBsonMapper : BsonMapper<Guid, SimpleEntity>
    {
        public SimpleEntityBsonMapper(IOptions<MongoOptions> options) : base(options)
        {
        }

        public override BsonDocument Map(Document<Guid,SimpleEntity> document, BsonDocument bson)
        {
            if (!string.IsNullOrWhiteSpace(document.Item.Name))
                bson.Add(SimpleDocumentFields.Name, document.Item.Name);

            return base.Map(document, bson);
        }

        public override Document<Guid,SimpleEntity> Map(BsonDocument bson, Document<Guid,SimpleEntity> document)
        {
            if (bson.TryGetValue(SimpleDocumentFields.Name, out var name) && name != BsonNull.Value)
                document.Item.Name = name.AsString;

            return base.Map(bson, document);
        }
    }
}
