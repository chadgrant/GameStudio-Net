using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests
{
    public class SimpleEntityReflectionBsonMapper : ReflectionBsonMapper<Guid, SimpleEntity>
    {
        public SimpleEntityReflectionBsonMapper(IOptions<MongoOptions> options) : base(options)
        {
        }

        public override void RegisterClassMap(BsonClassMap<SimpleEntity> cm)
        {
            cm.MapMember(m => m.Name).SetElementName(SimpleDocumentFields.Name);
            cm.MapMember(m => m.Id).SetElementName(MetadataFields.Id);
        }
    }
}
