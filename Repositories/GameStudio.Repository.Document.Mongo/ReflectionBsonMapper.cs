using System;
using System.ComponentModel;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace GameStudio.Repository.Document.Mongo
{
    /// <summary>
    /// Maps documents to bson using reflection
    /// override RegisterClassMap(BsonClassMap TDocument cm) to specify any custom mappings needed
    /// </summary>
    [Description("probably shouldn't use, it's 2x slower than BsonMapper")]
    public abstract class ReflectionBsonMapper<TId,TEntity>
        : Mapper<Document<TId, TEntity>, BsonDocument> where TEntity : new()
    {
        //Cosmos DB will add a generated _id field this is here to set both id/_id to the same value
        protected string AdditionalIdField;

        protected ReflectionBsonMapper(IOptions<MongoOptions> options)
        {
            AdditionalIdField = options.Value.AdditionalIdField;
        }

        public virtual void RegisterClassMap(BsonClassMap<TEntity> cm)
        {
        }

        public virtual void RegisterClassMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(TEntity)))
                return;

            try
            {
                BsonClassMap.RegisterClassMap<TEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);

                    if (typeof(ICreated).IsAssignableFrom(cm.ClassType))
                        cm.MapMember(c => ((ICreated) c).Created).SetElementName(MetadataFields.Created);

                    if (typeof(IUpdated).IsAssignableFrom(cm.ClassType))
                        cm.MapMember(c => ((IUpdated) c).Updated).SetElementName(MetadataFields.Updated);

                    if (typeof(IAuditor).IsAssignableFrom(cm.ClassType))
                    {
                        cm.MapMember(c => ((IAuditor) c).CreatedBy).SetElementName(MetadataFields.CreatedBy);
                        cm.MapMember(c => ((IAuditor) c).UpdatedBy).SetElementName(MetadataFields.UpdatedBy);
                    }

                    if (typeof(IVersionable).IsAssignableFrom(cm.ClassType))
                        cm.MapMember(c => ((IVersionable) c).Version).SetElementName(MetadataFields.Version);
                    
                    RegisterClassMap(cm);
                });
            }
            catch (ArgumentException)
            {
                //TODO: for some reason the if guard above is not working
            }
        }

        public override BsonDocument Map(Document<TId,TEntity> document, BsonDocument _)
        {
            RegisterClassMap();

            //TODO: ignoring input bson, prob shouldn't

            var entity = document.Item;
            var bson = entity.ToBsonDocument();

            var id = BsonValue.Create(document.Id);

            bson.Set(MetadataFields.Id, id);

            if (!string.IsNullOrWhiteSpace(AdditionalIdField))
                bson.Set(AdditionalIdField, id);

            if (!string.IsNullOrWhiteSpace(document.Revision))
                bson.Set(MetadataFields.Revision, document.Revision);

            return bson;
        }

        public override Document<TId,TEntity> Map(BsonDocument bson, Document<TId,TEntity> document)
        {
            RegisterClassMap();

            //TODO: ignoring document, prob shouldn't

            document.Item = BsonSerializer.Deserialize<TEntity>(bson);

            document.Id = (TId)BsonTypeMapper.MapToDotNetValue(bson.GetValue(MetadataFields.Id));

            if (bson.TryGetValue(MetadataFields.Revision, out var revision))
                document.Revision = revision.AsString;

            return document;
        }
    }
}
