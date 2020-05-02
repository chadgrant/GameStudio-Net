using System;

namespace GameStudio.Repository.Document
{
    public class Document
    {
        public static Document<TId, TEntity> Create<TId,TEntity>(TId id, TEntity item) where TEntity : new()
        {
            return new Document<TId, TEntity> { Id = id, Item = item };
        }
    }

    [Serializable]
    public class Document<TId,TEntity> where TEntity : new()
    {
        public TId Id { get; set; }

        public TEntity Item { get; set; } = new TEntity();

        public string Revision { get; set; }
    }
}
