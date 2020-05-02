using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStudio.Repository.Document.Mongo
{     
    public class MongoRepository<TId, TEntity>
        : MongoDocumentRepositoryBase<BsonDocument>,
            IDocumentRepository<TId, TEntity>,
            IAddUpdateDocumentRepository<TId, TEntity>,
            IPagedDocumentRepository<TId, TEntity>
        where TEntity : new()
    {
        readonly Mapper<Document<TId, TEntity>, BsonDocument> _mapper;

        public MongoRepository(IOptions<MongoOptions> options, Mapper<Document<TId, TEntity>, BsonDocument> mapper, string defaultCollectionName = null)
            : base(options, defaultCollectionName)
        {
            _mapper = mapper;
        }

        public async Task<Document<TId,TEntity>> AddAsync(TId id, Document<TId,TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsIdDefaultorEmpty(id))
                throw new ArgumentException("id cannot be empty / null");

            document.Revision =  Guid.NewGuid().ToString();

            await GetCollection()
                .InsertOneAsync( _mapper.Map(document), new InsertOneOptions(), cancellationToken);

            return document;
        }

        public async Task<Document<TId,TEntity>> UpdateAsync(TId id, Document<TId,TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsIdDefaultorEmpty(id))
                throw new ArgumentException("id cannot be empty / null");

            var filter = GetFilter(id, document.Revision);

            var options = new FindOneAndReplaceOptions<BsonDocument>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After
            };

            try
            {
                document.Revision = Guid.NewGuid().ToString();

                var bson = await GetCollection()
                    .FindOneAndReplaceAsync(filter, _mapper.Map(document), options, cancellationToken);

                if (bson != null)
                    return _mapper.Map(bson);

                //Have to re-lookup to see if it was a concurrency problem or document doesn't exist
                bson = (await GetCollection()
                        .FindAsync(GetIdFilter(id), new FindOptions<BsonDocument> {Limit = 1}, cancellationToken))
                    .FirstOrDefault();

                if (bson == null)
                    throw new DocumentNotFoundException($"Document not found: {id}");

                throw new ConcurrencyException("Concurrency Exception : [Update]", $"id: {id}. document id: {document.Id}. revision: {document.Revision}.");
            }
            catch (MongoCommandException cex)
            {
                if (cex.ErrorMessage.Contains(DuplicateKeyError) || cex.ErrorMessage.Contains("Non-unique id"))
                    throw new ConcurrencyException("Concurrency Exception : [Update]", $"for id: {id}. document id: {document.Id}. revision: {document.Revision}. {cex.ErrorMessage}", cex);

                throw;
            }
        }

        public virtual async Task<Document<TId, TEntity>> UpsertAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsIdDefaultorEmpty(id))
                throw new ArgumentException("id cannot be empty / null");

            var filter = GetFilter(id, document.Revision);

            //Apparently this is extremely common, annoying Mongo issue, using the exists the server won't retry
            //https://stackoverflow.com/questions/29305405/mongodb-impossible-e11000-duplicate-key-error-dup-key-when-upserting

            var options = new FindOneAndReplaceOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            try
            {
                document.Revision = Guid.NewGuid().ToString();

                var bson = await GetCollection()
                    .FindOneAndReplaceAsync(filter, _mapper.Map(document), options, cancellationToken);

                if (bson != null)
                    return _mapper.Map(bson);
            }
            catch (MongoCommandException cex)
            {
                if (cex.ErrorMessage.Contains(DuplicateKeyError) || cex.ErrorMessage.Contains("Non-unique id"))
                    throw new ConcurrencyException("Concurrency Exception [Upsert]",$"for id: {id}. document id: {document.Id} revision: {document.Revision}. {cex.ErrorMessage}", cex);

                throw;
            }

            return default(Document<TId, TEntity>);
        }

        public virtual async Task<Document<TId, TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsIdDefaultorEmpty(id))
                throw new ArgumentException("id cannot be empty / null");

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq(MetadataFields.Id, id);

            var bson = await GetCollection().FindSync<BsonDocument>(filter).FirstOrDefaultAsync(cancellationToken);

            if (bson == null)
                return default(Document<TId, TEntity>);

            return _mapper.Map(bson);
        }

        public virtual async Task<Document<TId, TEntity>> DeleteAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsIdDefaultorEmpty(id))
                throw new ArgumentException("id cannot be empty / null");

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq(MetadataFields.Id, id);

            var bson = await GetCollection()
                .FindOneAndDeleteAsync(filter, null , cancellationToken);

            if (bson == null)
                return default(Document<TId, TEntity>);

            return _mapper.Map(bson);
        }

        public async Task<IGetPagedResults<Document<TId, TEntity>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = new List<Document<TId, TEntity>>();
            var filter = FilterDefinition<BsonDocument>.Empty;
            var mquery = new FindOptions<BsonDocument, BsonDocument>
            {
                Skip = 0,
                Limit = query.Size
            };

            if (query.Page > 1)
                mquery.Skip = query.Page * query.Size - 1;

            var sort = query.Options?.Sort;
            if (sort != null)
            {
                if (!string.IsNullOrWhiteSpace(sort.Field))
                {
                    var sorter = Builders<BsonDocument>.Sort.Ascending(sort.Field);

                    if (sort.Descending.GetValueOrDefault(false))
                        sorter = Builders<BsonDocument>.Sort.Descending(sort.Field);

                    mquery.Sort = sorter;
                }
            }

            using (var cursor = await GetCollection().FindAsync(filter, mquery, cancellationToken))
            {
                while (await cursor.MoveNextAsync(cancellationToken))
                    list.AddRange(cursor.Current.Select(b => _mapper.Map(b)));
            }

            return GetPagedResults<Document<TId, TEntity>>.FromQueryResults(list, query.Page, query.Size);
        }

        bool IsIdDefaultorEmpty(TId id)
        {
            if (Equals(id, default(TId)))
                return true;

            if (typeof(TId) == typeof(string))
                return string.IsNullOrWhiteSpace(id.ToString());

            return false;
        }

        FilterDefinition<BsonDocument> GetFilter(TId id, string revision, FilterDefinitionBuilder<BsonDocument> builder = null)
        {
            var bld = builder ?? Builders<BsonDocument>.Filter;
            var filter = GetIdFilter(id);

            if (string.IsNullOrWhiteSpace(revision))
                return filter;
            
            return filter &
                     (bld.Eq(MetadataFields.Revision, revision) |
                      bld.Exists(MetadataFields.Revision, false));
        }

        FilterDefinition<BsonDocument> GetIdFilter(TId id, FilterDefinitionBuilder<BsonDocument> builder = null)
        {
            return (builder ?? Builders<BsonDocument>.Filter).Eq(MetadataFields.Id, id);
        }
    }
}
