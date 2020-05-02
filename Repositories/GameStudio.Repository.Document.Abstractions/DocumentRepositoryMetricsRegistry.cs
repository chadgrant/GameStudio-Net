using GameStudio.Metrics;

namespace GameStudio.Repository.Document
{
    static class Ops
    {
        internal const string GetPaged = "get_paged";
        internal const string Get = "get";
        internal const string Add = "add";
        internal const string Update = "update";
        internal const string Upsert = "upsert";
        internal const string Delete = "delete";
        internal const string Retry = "retry";
    }

    public abstract class DocumentRepositoryMetricsRegistry
    {
        protected DocumentRepositoryMetricsRegistry(IMetricsFactory factory, string repositoryName)
        {
            Counters = new Counters(factory, repositoryName);
            Histograms = new Histograms(factory, repositoryName);
        }

        public Counters Counters { get; }
        public Histograms Histograms { get; }
    }

    public class Counters
    {
        public Counters(IMetricsFactory factory, string repositoryName)
        {
            All = factory.Counter($"{repositoryName}_document_repository", $"counts all {repositoryName} document repository calls", "operation");

            GetPaged = factory.Counter(All, "counts get paged calls", Ops.GetPaged);
            Get = factory.Counter(All, "counts get calls", Ops.Get);
            Add = factory.Counter(All, "counts add operations", Ops.Add);
            Update = factory.Counter(All, "counts update operations", Ops.Update);
            Upsert = factory.Counter(All, "counts upsert operations", Ops.Upsert);
            Delete = factory.Counter(All, "counts delete operations", Ops.Delete);

            Errors = new ErrorCounters(factory, repositoryName);
        }

        public ICounter All { get; }
        public ICounter GetPaged { get; }
        public ICounter Get { get; }
        public ICounter Add { get; }
        public ICounter Update { get; }
        public ICounter Upsert { get; }
        public ICounter Delete { get; }
        public ErrorCounters Errors { get; }
    }

    public class ErrorCounters
    {
        public ErrorCounters(IMetricsFactory factory, string repositoryName)
        {
            All = factory.Counter($"{repositoryName}_document_repository_err", $"counts all errors to {repositoryName} document repository", "operation");

            GetPaged = factory.Counter(All, "counts errors to get paged", Ops.GetPaged);
            Get = factory.Counter(All, "counts errors to get", Ops.Get);
            Add = factory.Counter(All, "counts errors to add", Ops.Add);
            Update = factory.Counter(All, "counts errors update", Ops.Update);
            Upsert = factory.Counter(All, "counts errors upsert", Ops.Upsert);
            Delete = factory.Counter(All, "counts errors to delete", Ops.Delete);
            Retry = factory.Counter(All, "counts repo retries", Ops.Retry);
        }

        public ICounter All { get; }
        public ICounter GetPaged { get; }
        public ICounter Get { get; }
        public ICounter Add { get; }
        public ICounter Update { get; }
        public ICounter Upsert { get; }

        public ICounter Delete { get; }
        public ICounter Retry { get; }
    }

    public class Histograms
    {
        static readonly double[] Buckets = {
            0,
            0.1,
            0.2,
            0.4,
            0.6,
            0.8,
            1,
            2,
            3,
            5
        };

        public Histograms(IMetricsFactory factory, string repositoryName)
        {
            All = factory.Histogram($"{repositoryName}_document_repository_call_duration", $"measures the duration of {repositoryName} document repository calls", Buckets, "operation");

            GetPaged = factory.Histogram(All, "times calls to get paged", Ops.GetPaged);
            Get = factory.Histogram(All, "times calls to get", Ops.Get);
            Add = factory.Histogram(All, "times calls to add", Ops.Add);
            Update = factory.Histogram(All, "times calls to update", Ops.Update);
            Upsert = factory.Histogram(All, "times calls to upsert", Ops.Upsert);
            Delete = factory.Histogram(All, "times calls to delete", Ops.Delete);
        }

        public IHistogram All { get; }
        public IHistogram GetPaged { get; }
        public IHistogram Get { get; }
        public IHistogram Add { get; }
        public IHistogram Update { get; }
        public IHistogram Upsert { get; }
        public IHistogram Delete { get; }
    }
}
