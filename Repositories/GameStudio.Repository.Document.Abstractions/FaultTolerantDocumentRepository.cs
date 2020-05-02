using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;

namespace GameStudio.Repository.Document.Abstractions
{
    public static class Extensions
    {
        public static IDocumentRepository<TId, TDocument> FaultTolerant<TId, TDocument>(this IDocumentRepository<TId, TDocument> repo, IOptions<FaultTolerantDocumentRepositoryOptions> options) where TDocument : new()
        {
            return new FaultTolerantDocumentRepository<TId, TDocument>(repo, options);
        }
    }

    public class FaultTolerantDocumentRepositoryOptions
    {
        public int Retries { get; set; } = 5;
        public int TimeoutStepMilliseconds { get; set; } = 500;
        public int ExceptionsBeforeBreaking { get; set; } = 15;
        public int DurationOfBreakMilliseconds { get; set; } = 5000;
    }

    public class FaultTolerantDocumentRepository<TId, TEntity> : IDocumentRepository<TId, TEntity>
        where TEntity : new()
    {
        protected readonly IDocumentRepository<TId, TEntity> DecoratedRepo;
        protected readonly IAsyncPolicy RetryPolicy;

        public FaultTolerantDocumentRepository(IDocumentRepository<TId,TEntity> decoratedRepo, IOptions<FaultTolerantDocumentRepositoryOptions> options)
        {
            DecoratedRepo = decoratedRepo;

            // Retry Policy
            // We don't retry if the inner circuit-breaker
            // judges the underlying system is out of commission.
            //
            // Exponential Back off  1, 2, 4, 8, 16 etc...
            var waitAndRetry =
                Policy.Handle<TimeoutException>()
                    .Or<LockedException>()
                    .Or<DocumentServerException>()
                .WaitAndRetryAsync(options.Value.Retries,
                    attempt => TimeSpan.FromMilliseconds(options.Value.TimeoutStepMilliseconds * Math.Pow(2, attempt)),
                    (exception, waitDuration) =>
                    {
                        //TODO Log Errors
                    });

            var circuitBreaker = Policy.Handle<TimeoutException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: options.Value.ExceptionsBeforeBreaking,
                    durationOfBreak: TimeSpan.FromMilliseconds(options.Value.DurationOfBreakMilliseconds),
                    onBreak: (ex, breakDelay) =>
                    {
                        //TODO Log Errors / Metrics
                    },
                    onReset: () =>
                    {
                        //TODO Log Errors / Metrics
                    },
                    onHalfOpen: () =>
                    {
                        //TODO Log Errors / Metrics
                    }
                );

            RetryPolicy = Policy.WrapAsync(waitAndRetry, circuitBreaker);
        }
        
        public Task<Document<TId, TEntity>> UpsertAsync(TId id, Document<TId, TEntity> document, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RetryPolicy.ExecuteAsync(async () => await DecoratedRepo.UpsertAsync(id, document, cancellationToken));
        }

        public Task<Document<TId, TEntity>> GetAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RetryPolicy.ExecuteAsync(async () => await DecoratedRepo.GetAsync(id, cancellationToken));
        }

        public Task<Document<TId, TEntity>> DeleteAsync(TId id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RetryPolicy.ExecuteAsync(async () => await DecoratedRepo.DeleteAsync(id, cancellationToken));
        }
    }

    public class PagedFaultTorentDocumentRepository<TId, TEntity> :
        FaultTolerantDocumentRepository<TId, TEntity>, IPagedDocumentRepository<TId, TEntity>
        where TEntity : new()
    {
        public PagedFaultTorentDocumentRepository(IDocumentRepository<TId, TEntity> decoratedRepo, IOptions<FaultTolerantDocumentRepositoryOptions> options)
            : base(decoratedRepo, options)
        {
        }

        public Task<IGetPagedResults<Document<TId, TEntity>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return RetryPolicy.ExecuteAsync(async () => await ((IPagedDocumentRepository<TId, TEntity>)DecoratedRepo).GetPagedAsync(query, cancellationToken));
        }
    }

}
