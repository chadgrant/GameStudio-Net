using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using GameStudio.Repository.Document.Mongo;
using Xunit;

namespace GameStudio.Repository.Document.Tests.Mapper
{
    public abstract class MapperTests<TId, TEntity> where TEntity : IAudit, IAuditor, IVersionable, new()
    {
        protected abstract BsonMapper<TId,TEntity> GetMapper();

        [Fact]
        public void Maps_Document_Audit_Fields()
        {
            var created = DateTime.UtcNow;
            var updated = DateTime.UtcNow.AddDays(1);
            var rev = Guid.NewGuid().ToString();

            var bson = new BsonDocument
            {
                { MetadataFields.Created, created },
                { MetadataFields.Updated,  updated },
                { MetadataFields.CreatedBy, "Bob" },
                { MetadataFields.UpdatedBy, "Alice" },
                { MetadataFields.Version, 42 },
                { MetadataFields.Id, "player-666" },
                { MetadataFields.Revision, rev }
            };

            var mapper = GetMapper();

            var document = mapper.Map(bson);
            var sut = document.Item;

            AssertEqual(created, sut.Created);
            AssertEqual(updated, sut.Updated);
            Assert.Equal("Bob", sut.CreatedBy);
            Assert.Equal("Alice", sut.UpdatedBy);
            Assert.Equal(42, sut.Version);
            Assert.Equal("player-666", document.Id.ToString());
            Assert.Equal(rev, document.Revision);
        }

        [Fact]
        public void Maps_Entity_Audit_Fields()
        {
            var entity = new TEntity
            {
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow.AddDays(1),
                CreatedBy = "Bob",
                UpdatedBy = "Alice",
                Version = 42
            };

            var document = new Document<TId, TEntity>
            {
                Id = (TId)Convert.ChangeType("player-666", typeof(TId)),
                Item = entity,
                Revision = Guid.NewGuid().ToString()
            };

            var mapper = GetMapper();
            
            var bson = mapper.Map(document);

            AssertEqual(entity.Created,bson.GetValue(MetadataFields.Created).ToUniversalTime());
            AssertEqual(entity.Updated, bson.GetValue(MetadataFields.Updated).ToNullableUniversalTime());
            Assert.Equal(entity.CreatedBy, bson.GetValue(MetadataFields.CreatedBy).AsString);
            Assert.Equal(entity.UpdatedBy, bson.GetValue(MetadataFields.UpdatedBy).AsString);
            Assert.Equal(entity.Version, bson.GetValue(MetadataFields.Version).AsInt32);
            Assert.Equal(document.Id.ToString() , bson.GetValue(MetadataFields.Id).AsString);
            Assert.Equal(document.Revision, bson.GetValue(MetadataFields.Revision).AsString);
        }

        protected void AssertEqual(DateTime expected, DateTime actual)
        {
            var diff = expected - actual;
            Assert.True(diff.TotalSeconds < 1);
        }

        protected void AssertEqual(DateTime? expected, DateTime? actual)
        {
            var diff = expected.GetValueOrDefault(DateTime.MinValue) - actual.GetValueOrDefault(DateTime.MinValue);
            Assert.True(diff.TotalSeconds < 1);
        }
    }

    public abstract class ComplexDocumentMapperTests : MapperTests<string, ComplexEntity>
    {
        [Fact]
        public void Maps_To_Document_Fields()
        {
            var bson = new BsonDocument
            {
                { MetadataFields.Id, "player-666" },
                { ComplexDocumentFields.IntProperty, 42 },
                { ComplexDocumentFields.LongProperty, 999L },
                { ComplexDocumentFields.NullInt, 12 },
                { ComplexDocumentFields.NullLong, 55L },
                { ComplexDocumentFields.String, "test" },
                { ComplexDocumentFields.DateProperty, DateTime.UtcNow },
                { ComplexDocumentFields.NullDateTime, DateTime.UtcNow },
            };

            var mapper = GetMapper();

            var document = mapper.Map(bson);

            Assert.Equal("player-666", document.Id);
            Assert.Equal(42, document.Item.IntProperty);
            Assert.Equal(999L, document.Item.LongProperty);
            Assert.Equal(12, document.Item.NullInt);
            Assert.Equal(55L, document.Item.NullLong);
            Assert.Equal("test", document.Item.String);
            AssertEqual(DateTime.UtcNow, document.Item.DateProperty);
            AssertEqual(DateTime.UtcNow, document.Item.NullDateTime);
        }

        [Fact]
        public void Maps_To_Entity_Fields()
        {
            var document = Document.Create("player-666",new ComplexEntity
            {
                Id = "player-666",
                IntProperty = 42,
                LongProperty =  999L,
                NullInt = 12,
                NullLong = 55L,
                String = "test",
                DateProperty = DateTime.UtcNow,
                NullDateTime = DateTime.UtcNow
            });

            var mapper = GetMapper();

            var bson = mapper.Map(document);

            Assert.Equal("player-666", bson.GetValue(MetadataFields.Id).AsString);
            Assert.Equal(42, bson.GetValue(ComplexDocumentFields.IntProperty).AsInt32);
            Assert.Equal(999L, bson.GetValue(ComplexDocumentFields.LongProperty).AsInt64);
            Assert.Equal(12, bson.GetValue(ComplexDocumentFields.NullInt).AsNullableInt32);
            Assert.Equal(55L,bson.GetValue(ComplexDocumentFields.NullLong).AsNullableInt64);
            Assert.Equal("test", bson.GetValue(ComplexDocumentFields.String).AsString);
            AssertEqual(DateTime.UtcNow, bson.GetValue(ComplexDocumentFields.DateProperty).ToUniversalTime());
            AssertEqual(DateTime.UtcNow, bson.GetValue( ComplexDocumentFields.NullDateTime).ToNullableUniversalTime());
        }
    }
}

namespace GameStudio.Repository.Document.Tests.Mapper.Bson
{
    public class ComplexDocumentBsonMapperTests : ComplexDocumentMapperTests
    {
        protected override BsonMapper<string, ComplexEntity> GetMapper()
        {
            return new ComplexEntityBsonMapper(Options.Create(new MongoOptions()));
        }
    }
}
