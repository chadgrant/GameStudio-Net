using System;
using System.Threading;
using System.Threading.Tasks;
using GameStudio.Metrics;

namespace GameStudio.Repository.Document
{
    public class MetricsDocumentRepository<TId,TEntity> : MetricsDocumentRepositoryBase, IDocumentRepository<TId,TEntity> where TEntity : new()
    {
        protected readonly IDocumentRepository<TId, TEntity> _repo;
        
        public MetricsDocumentRepository(IDocumentRepository<TId,TEntity> repo, DocumentRepositoryMetricsRegistry metrics) : base(metrics)
        {
            _repo = repo;
        }

        public Task<Document<TId, TEntity>> UpsertAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Upsert, _errors.Upsert, _histograms.Upsert, () => _repo.UpsertAsync(id, document, cancellationToken));
        }

        public Task<Document<TId, TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Get, _errors.Get, _histograms.Get, () => _repo.GetAsync(id, cancellationToken));
        }

        public Task<Document<TId, TEntity>> DeleteAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Delete, _errors.Delete, _histograms.Delete, () => _repo.DeleteAsync(id, cancellationToken));
        }
    }

    public class MetricsAddUpdateDocumentRepository<TId, TEntity> : MetricsDocumentRepositoryBase, IAddUpdateDocumentRepository<TId, TEntity> where TEntity : new()
    {
        protected readonly IAddUpdateDocumentRepository<TId, TEntity> _repo;

        public MetricsAddUpdateDocumentRepository(IAddUpdateDocumentRepository<TId, TEntity> repo, DocumentRepositoryMetricsRegistry metrics) : base(metrics)
        {
            _repo = repo;
        }

        public Task<Document<TId, TEntity>> AddAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Add, _errors.Add, _histograms.Add, () => _repo.AddAsync(id, document, cancellationToken));
        }

        public Task<Document<TId, TEntity>> UpdateAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Update, _errors.Update, _histograms.Update, () => _repo.UpdateAsync(id, document, cancellationToken));
        }

        public Task<Document<TId, TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Get, _errors.Get, _histograms.Get, () => _repo.GetAsync(id, cancellationToken));
        }

        public Task<Document<TId, TEntity>> DeleteAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(_counters.Delete, _errors.Delete, _histograms.Delete, () => _repo.DeleteAsync(id, cancellationToken));
        }
    }

    public abstract class MetricsDocumentRepositoryBase
    {
        protected readonly Counters _counters;
        protected readonly ErrorCounters _errors;
        protected readonly Histograms _histograms;

        protected MetricsDocumentRepositoryBase(DocumentRepositoryMetricsRegistry metrics)
        {
            _counters = metrics.Counters;
            _errors = metrics.Counters.Errors;
            _histograms = metrics.Histograms;
        }

        protected T ExecuteAsync<T>(ICounter total, ICounter error, IHistogram histogram, Func<T> fn)
        {
            try
            {
                using (histogram.Time())
                    return fn();
            }
            catch (Exception)
            {
                error.Increment();
                throw;
            }
            finally
            {
                total.Increment();
            }
        }
    }

    public class MetricsPagedDocumentRepository<TId, TEntity> : MetricsDocumentRepositoryBase, IPagedDocumentRepository<TId, TEntity> where TEntity : new()
    {
        readonly IPagedDocumentRepository<TId, TEntity> _repo;

        public MetricsPagedDocumentRepository(IPagedDocumentRepository<TId, TEntity> repo, DocumentRepositoryMetricsRegistry metrics) : base(metrics)
        {
            _repo = repo;
        }

        public Task<IGetPagedResults<Document<TId, TEntity>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _repo.GetPagedAsync(query, cancellationToken);
        }
    }
}
