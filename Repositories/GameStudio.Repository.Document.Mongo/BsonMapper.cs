using System;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace GameStudio.Repository.Document.Mongo
{
    /// <summary>
    /// Maps Bson to Document
    /// Base class to be derived from so that metadata can be handled generically
    /// Documents can simply implement the IAudit, IAuditor etc.. interfaces
    /// </summary>
    /// <typeparam name="TId">
    /// Type of the document's Id field
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// Type of the Entity
    /// </typeparam>
    public abstract class BsonMapper<TId,TEntity>
        : Mapper<Document<TId,TEntity>, BsonDocument> where TEntity : new()
    {
        //Cosmos DB will add a generated _id field this is here to set both id/_id to the same value
        protected string AdditionalIdField;

        protected BsonMapper(IOptions<MongoOptions> options)
        {
            AdditionalIdField = options.Value.AdditionalIdField;
        }

        public override BsonDocument Map(Document<TId,TEntity> document, BsonDocument bson)
        {
            var entity = document.Item;

            var id = BsonValue.Create(document.Id);

            bson.Set(MetadataFields.Id, BsonValue.Create(id));
            if (!string.IsNullOrWhiteSpace(AdditionalIdField))
                bson.Set(AdditionalIdField, id);

            var revision = document.Revision;
            if (!string.IsNullOrWhiteSpace(revision))
                bson.Set(MetadataFields.Revision, revision);

            if (entity is IVersionable versionable)
                bson.Set(MetadataFields.Version, versionable.Version.GetValueOrDefault(0));

            if (entity is IAuditor auditor)
            {
                if (!string.IsNullOrWhiteSpace(auditor.CreatedBy))
                    bson.Set(MetadataFields.CreatedBy, auditor.CreatedBy);

                if (!string.IsNullOrWhiteSpace(auditor.UpdatedBy))
                    bson.Set(MetadataFields.UpdatedBy, auditor.UpdatedBy);
            }

            if (entity is ICreated created)
            {
                if (created.Created != default(DateTime))
                    bson.Set(MetadataFields.Created, created.Created);
            }

            if (entity is IUpdated updated)
            {
                if (updated.Updated.HasValue)
                    bson.Set(MetadataFields.Updated, updated.Updated);
            }
            
            return bson;
        }

        public override Document<TId,TEntity> Map(BsonDocument bson, Document<TId,TEntity> document)
        {
            var entity = document.Item;

            document.Id = (TId)BsonTypeMapper.MapToDotNetValue(bson.GetValue(MetadataFields.Id));

            if (bson.TryGetValue(MetadataFields.Revision, out var revision))
                document.Revision = revision.AsString;

            if (entity is IVersionable versionable)
            {
                if (bson.TryGetValue(MetadataFields.Version, out var version))
                    versionable.Version = version.AsNullableInt32;
            }

            if (entity is IAuditor auditor)
            {
                if (bson.TryGetValue(MetadataFields.CreatedBy, out var createdBy))
                    auditor.CreatedBy = createdBy.AsString;

                if (bson.TryGetValue(MetadataFields.UpdatedBy, out var updatedBy) && updatedBy != BsonNull.Value)
                    auditor.UpdatedBy = updatedBy.AsString;
            }

            if (entity is IAudit audit)
            {
                if (bson.TryGetValue(MetadataFields.Created, out var createdDate))
                    audit.Created = createdDate.ToUniversalTime();

                if (bson.TryGetValue(MetadataFields.Updated, out var updatedDate))
                    audit.Updated = updatedDate.ToNullableUniversalTime();
            }

            return document;
        }
    }
}
