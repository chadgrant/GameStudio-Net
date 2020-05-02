using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests
{
    public class ComplexEntityReflectionBsonMapper : ReflectionBsonMapper<string, ComplexEntity>
    {
        public override void RegisterClassMap(BsonClassMap<ComplexEntity> cm)
        {
            cm.MapMember(m => m.IntProperty).SetElementName(ComplexDocumentFields.IntProperty);
            cm.MapMember(m => m.NullInt).SetElementName(ComplexDocumentFields.NullInt);
            cm.MapMember(m => m.LongProperty).SetElementName(ComplexDocumentFields.LongProperty);
            cm.MapMember(m => m.NullLong).SetElementName(ComplexDocumentFields.NullLong);
            cm.MapMember(m => m.String).SetElementName(ComplexDocumentFields.String);
            cm.MapMember(m => m.DateProperty).SetElementName(ComplexDocumentFields.DateProperty);
            cm.MapMember(m => m.NullDateTime).SetElementName(ComplexDocumentFields.NullDateTime);
        }

        public ComplexEntityReflectionBsonMapper(IOptions<MongoOptions> options) : base(options)
        {
        }
    }
}
