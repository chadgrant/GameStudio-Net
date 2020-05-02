using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using GameStudio.Repository.Document.Mongo;

namespace GameStudio.Repository.Document.Tests
{
    public static class ComplexDocumentFields
    {
        public const string IntProperty = "ti";
        public const string NullInt = "tni";
        public const string LongProperty = "tl";
        public const string NullLong = "tnl";
        public const string String = "ts";
        public const string DateProperty = "td";
        public const string NullDateTime = "tnd";
    }

    public class ComplexEntityBsonMapper : BsonMapper<string, ComplexEntity>
    {
        public ComplexEntityBsonMapper(IOptions<MongoOptions> options) : base(options)
        {
        }


        public override BsonDocument Map(Document<string,ComplexEntity> document, BsonDocument bson)
        {
            var entity = document.Item;

            bson.AddRange(
                new Dictionary<string, object>
                {
                    { ComplexDocumentFields.IntProperty, entity.IntProperty },
                    { ComplexDocumentFields.LongProperty, entity.LongProperty },
                    { ComplexDocumentFields.DateProperty, entity.DateProperty }
                }
            );

            if (entity.NullDateTime.HasValue)
                bson.Add(ComplexDocumentFields.NullDateTime, entity.NullDateTime.Value);

            if (!string.IsNullOrWhiteSpace(entity.String))
                bson.Add(ComplexDocumentFields.String, entity.String);

            if (entity.NullLong.HasValue)
                bson.Add(ComplexDocumentFields.NullLong, entity.NullLong.Value);

            if (entity.NullInt.HasValue)
                bson.Add(ComplexDocumentFields.NullInt, entity.NullInt.Value);

            return base.Map(document, bson);
        }

        public override Document<string,ComplexEntity> Map(BsonDocument bson, Document<string,ComplexEntity> document)
        {
            document = base.Map(bson, document);

            var entity = document.Item;
            entity.Id = document.Id;
            if (bson.TryGetValue(ComplexDocumentFields.IntProperty, out var tint))
                entity.IntProperty = tint.AsInt32;

            if (bson.TryGetValue(ComplexDocumentFields.NullInt, out var tnint))
                entity.NullInt = tnint.AsNullableInt32;

            if (bson.TryGetValue(ComplexDocumentFields.LongProperty, out var tlong))
                entity.LongProperty = tlong.AsInt64;

            if (bson.TryGetValue(ComplexDocumentFields.NullLong, out var tnlong))
                entity.NullLong = tnlong.AsNullableInt64;

            if (bson.TryGetValue(ComplexDocumentFields.DateProperty, out var tdate))
                entity.DateProperty = tdate.ToUniversalTime();

            if (bson.TryGetValue(ComplexDocumentFields.NullDateTime, out var tndate))
                entity.NullDateTime = tndate.ToNullableUniversalTime();

            if (bson.TryGetValue(ComplexDocumentFields.String, out var tstring) && tstring != BsonNull.Value)
                entity.String = tstring.AsString;
            
            return document;
        }
    }
}
