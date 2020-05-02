namespace GameStudio.Repository.Document
{
    /// <summary>
    /// Base class for Mappers
    /// two primary implementations :
    ///     BsonMapper: perform mapping manually by specifying fields on document to record
    ///     ReflectionMapper: automaps fields with reflection and class maps
    /// </summary>
//    public abstract class DocumentMapper<TId,TDocument,TEntity> : Mapper<TDocument,TEntity>
//        where TDocument : Document<TEntity>, new()
//        where TEntity : new()
//    {
//        public abstract void SetId(TDocument document, TId id);
//        public abstract TId GetId(TDocument document);
//    }
}
