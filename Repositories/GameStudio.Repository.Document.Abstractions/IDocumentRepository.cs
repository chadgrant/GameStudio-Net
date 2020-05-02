using System.Threading;
using System.Threading.Tasks;

namespace GameStudio.Repository.Document
{
    public interface IAddAsync<TId, TEntity> where TEntity : new()
    {
        Task<Document<TId, TEntity>> AddAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IUpdateAsync<TId, TEntity> where TEntity : new()
    {
        Task<Document<TId, TEntity>> UpdateAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IUpsertAsync<TId, TEntity> where TEntity : new()
    {
        Task<Document<TId, TEntity>> UpsertAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IGetAsync<TId, TEntity> where TEntity : new()
    {
        Task<Document<TId, TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IDeleteAsync<TId, TEntity> where TEntity : new()
    {
        Task<Document<TId, TEntity>> DeleteAsync(TId id, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IPagedDocumentRepository<TId,TEntity> where TEntity : new()
    {
        Task<IGetPagedResults<Document<TId, TEntity>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default(CancellationToken));
    }

    //Default Repository Implementation: Upsert, Get, Delete
    public interface IDocumentRepository<TId,TEntity> :
        IUpsertAsync<TId, TEntity>,
        IGetAsync<TId,TEntity>,
        IDeleteAsync<TId, TEntity>
        where TEntity : new()
    {
    }

    //Explicit Add/Update Implementation
    public interface IAddUpdateDocumentRepository<TId, TEntity> :
        IAddAsync<TId, TEntity>,
        IUpdateAsync<TId, TEntity>,
        IGetAsync<TId, TEntity>,
        IDeleteAsync<TId, TEntity>
        where TEntity : new()
    {
    }
}
