using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using MongoDB.Driver;
using GameStudio.Repository.Document.Mongo;
using Xunit;

namespace GameStudio.Repository.Document.Tests
{
    public abstract class DocumentRepositoryTests<TId,TEntity> where TEntity : new()
    {
        readonly TestDocumentProvider<TId, TEntity> _docProvider;

        protected DocumentRepositoryTests(TestDocumentProvider<TId, TEntity> docProvider)
        {
            _docProvider = docProvider;
        }

        public abstract bool Enabled { get; }

        public abstract MongoRepository<TId, TEntity> GetRepository();

        [SkippableFact]
        public async Task Add_Does_Not_Throw()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();

            var document = _docProvider.CreateTestDocument();

            var added = await repo.AddAsync(document.Id, document);
            Assert.True(added != null);
            _docProvider.AssertEqual(document, added);
            Assert.False(string.IsNullOrEmpty(added.Revision));
        }

        [SkippableFact]
        public async Task Upsert_Does_Not_Throw()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();

            var document = _docProvider.CreateTestDocument();

            var upserted = await repo.UpsertAsync(document.Id, document);
            Assert.True(upserted != null);
            _docProvider.AssertEqual(document, upserted);
            Assert.False(string.IsNullOrEmpty(upserted.Revision));
        }

        [SkippableFact]
        public async Task Update_Throws_If_Not_Found()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();

            var document = _docProvider.CreateTestDocument();

            await Assert.ThrowsAsync<DocumentNotFoundException>( async () => await repo.UpdateAsync(document.Id, document) );
        }

        [SkippableFact]
        public async Task Update_Document()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();

            var document = _docProvider.CreateTestDocument();

            var added = await repo.AddAsync(document.Id, document);
            Assert.True(added != null);
            _docProvider.AssertEqual(document, added);
            Assert.False(string.IsNullOrEmpty(added.Revision));

            var toUpdate = _docProvider.UpdateTestDocument(document);

            var updated = await repo.UpdateAsync(toUpdate.Id, toUpdate);

            _docProvider.AssertUpdated(added, toUpdate, updated);
        }


        [SkippableFact]
        public async Task Add_And_Get_By_Key()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();

            var document = _docProvider.CreateTestDocument();

            var added = await repo.AddAsync(document.Id, document);
            Assert.True(added != null);
            _docProvider.AssertEqual(document, added);
            Assert.False(string.IsNullOrEmpty(added.Revision));

            var fromget = await repo.GetAsync(document.Id);
            Assert.NotNull(fromget);
            _docProvider.AssertEqual(document, fromget);
            Assert.Equal(document.Revision, fromget.Revision);
        }

        [SkippableFact]
        public async Task Upsert_And_Get_By_Key()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();

            var document = _docProvider.CreateTestDocument();

            var upserted = await repo.UpsertAsync(document.Id, document);
            Assert.True(upserted != null);
            _docProvider.AssertEqual(document, upserted);
            Assert.False(string.IsNullOrEmpty(upserted.Revision));

            var fromget = await repo.GetAsync(document.Id);
            Assert.NotNull(fromget);
            _docProvider.AssertEqual(document, fromget);
            Assert.Equal(upserted.Revision, fromget.Revision);
        }

        [SkippableFact]
        public async Task Upsert_Fails_If_Revision_Changed()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();
            var document = _docProvider.CreateTestDocument();

            var first = await repo.UpsertAsync(document.Id, document);
            Assert.False(string.IsNullOrEmpty(document.Revision));
            Assert.Equal(document.Id, first.Id);

            var second = await repo.UpsertAsync(first.Id, first);
            //Assert.NotEqual(first.Revision.Value, second.Revision.Value);
            Assert.Equal(first.Id,second.Id);

            var rev = second.Revision;
            second.Revision = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<ConcurrencyException>(async () => await repo.UpsertAsync(second.Id, second));

            second.Revision = rev;
            await repo.UpsertAsync(second.Id, second);
        }

        [SkippableFact]
        public async Task Update_Fails_If_Revision_Changed()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();
            var document = _docProvider.CreateTestDocument();

            var first = await repo.AddAsync(document.Id, document);
            Assert.False(string.IsNullOrEmpty(first.Revision));
            Assert.Equal(document.Id, first.Id);

            var second = await repo.UpdateAsync(first.Id, first);
            Assert.False(string.IsNullOrEmpty(second.Revision));
            Assert.Equal(first.Id, second.Id);

            second.Revision = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<ConcurrencyException>(async () => await repo.UpdateAsync(second.Id, second));
        }

        [SkippableFact]
        public async Task Duplicate_Add_Fails()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();
            var document = _docProvider.CreateTestDocument();

            var first = await repo.AddAsync(document.Id, document);
            Assert.False(string.IsNullOrEmpty(first.Revision));
            Assert.Equal(document.Id, first.Id);

            await Assert.ThrowsAsync<MongoWriteException>(async () => await repo.AddAsync(first.Id, first));
        }

        [SkippableFact]
        public async Task Can_Get_Paged()
        {
            Skip.IfNot(Enabled);

            var repo = GetRepository();
            Assert.NotNull(repo);
            var paged = await ((IPagedDocumentRepository<TId, TEntity>)repo).GetPagedAsync(PagedQuery.Create());
            Assert.True(paged.Results.Any());

            foreach (var d in paged.Results)
            {
                var doc = await repo.GetAsync(d.Id);
                Assert.Equal(d.Id, doc.Id);
            }
        }
    }

    public abstract class TestDocumentProvider<TId,TEntity> where TEntity : new()
    {
        public abstract Document<TId,TEntity> CreateTestDocument();
        public abstract void AssertEqual(Document<TId,TEntity> expected, Document<TId,TEntity> actual);

        public abstract Document<TId, TEntity> UpdateTestDocument(Document<TId, TEntity> document);
        public abstract void AssertUpdated(Document<TId, TEntity> orig, Document<TId, TEntity> toUpdate, Document<TId, TEntity> updated);

        protected Document<TId,TEntity> Clone(Document<TId,TEntity> obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (Document<TId, TEntity>)formatter.Deserialize(ms);
            }
        }
    }

    public class SimpleTestDocumentProvider : TestDocumentProvider<Guid,SimpleEntity>
    {
        public override Document<Guid,SimpleEntity> CreateTestDocument()
        {
            var guid = Guid.NewGuid();

            return Document.Create(guid,new SimpleEntity
            {
                Id = guid,
                Name = "Simple " + Guid.NewGuid()
            });
        }

        public override void AssertEqual(Document<Guid,SimpleEntity> expected, Document<Guid, SimpleEntity> actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Item.Id, actual.Id);
            Assert.Equal(expected.Id, actual.Item.Id);
            Assert.Equal(expected.Item.Id, actual.Item.Id);
            Assert.Equal(expected.Item.Name, actual.Item.Name);
        }

        public override Document<Guid,SimpleEntity> UpdateTestDocument(Document<Guid,SimpleEntity> document)
        {
            var clone = Clone(document);
            clone.Item.Name = "Updated - " + document.Item.Name;
            return clone;
        }

        public override void AssertUpdated(Document<Guid, SimpleEntity> orig, Document<Guid, SimpleEntity> toUpdate, Document<Guid, SimpleEntity> updated)
        {
            Assert.Equal(orig.Id, updated.Id);
            Assert.Equal(toUpdate.Item.Name,updated.Item.Name);
            Assert.NotEqual(orig.Item.Name,updated.Item.Name);
        }
    }

    public class ComplexTestDocumentProvider : TestDocumentProvider<string, ComplexEntity>
    {
        public override Document<string,ComplexEntity> CreateTestDocument()
        {
            var id = Guid.NewGuid().ToString();

            return Document.Create(id, new ComplexEntity
            {
                Id = id,
                DateProperty = DateTime.UtcNow.AddDays(-99),
                IntProperty = 42,
                NullInt = 42,
                LongProperty = 422222L,
                NullLong = 4222L,
                String = "this is a string",
                CreatedBy = "Unit Tests",
                Created = DateTime.UtcNow
            });
        }

        public override void AssertEqual(Document<string,ComplexEntity> expected, Document<string,ComplexEntity> actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Item.Id, actual.Item.Id);
            Assert.Equal(expected.Item.Created, actual.Item.Created, TimeSpan.FromMinutes(1));
            Assert.Equal(expected.Item.CreatedBy, actual.Item.CreatedBy);
            Assert.Equal(expected.Item.DateProperty, actual.Item.DateProperty, TimeSpan.FromMinutes(1));
            Assert.Equal(expected.Item.IntProperty, actual.Item.IntProperty);
            Assert.Equal(expected.Item.LongProperty, actual.Item.LongProperty);
            Assert.Equal(expected.Item.NullDateTime.GetValueOrDefault(DateTime.MinValue), actual.Item.NullDateTime.GetValueOrDefault(DateTime.MinValue), TimeSpan.FromMinutes(1));
            Assert.Equal(expected.Item.NullInt, actual.Item.NullInt);
            Assert.Equal(expected.Item.NullLong, actual.Item.NullLong);
        }

        public override Document<string,ComplexEntity> UpdateTestDocument(Document<string,ComplexEntity> document)
        {
            var clone = Clone(document);
            clone.Item.IntProperty++;
            clone.Item.LongProperty++;
            clone.Item.UpdatedBy = "Unit Test";
            clone.Item.Updated = DateTime.UtcNow;
            clone.Item.Created = DateTime.SpecifyKind(clone.Item.Created, DateTimeKind.Utc);
            return clone;
        }

        public override void AssertUpdated(Document<string, ComplexEntity> orig, Document<string, ComplexEntity> toUpdate, Document<string, ComplexEntity> updated)
        {
            Assert.Equal(orig.Id, updated.Id);
            Assert.NotEqual(orig.Item.IntProperty, updated.Item.IntProperty);
            Assert.NotEqual(orig.Item.LongProperty, updated.Item.LongProperty);
            Assert.Equal("Unit Test", updated.Item.UpdatedBy);
            Assert.Equal(orig.Item.CreatedBy, updated.Item.CreatedBy);
            Assert.Equal(orig.Item.Created,updated.Item.Created, TimeSpan.FromMinutes(1));
            //Assert.NotEqual(orig.Revision, updated.Revision);
            Assert.NotNull(updated.Item.Updated);
        }
    }
}
