using System;

namespace GameStudio.Metrics.Null
{
    public class NullMetricsFactory : IMetricsFactory
    {
        static class Nulls
        {
            public static NullCounter Counter { get; } = new NullCounter();
            public static NullHistogram Histogram { get; } = new NullHistogram();
            public static NullGauge Gauge { get; } = new NullGauge();
        }

        public ICounter Counter(string name, string description, params string[] labels)
        {
            return Nulls.Counter;
        }

        public ICounter Counter(ICounter parent, string description, params string[] labels)
        {
            return Nulls.Counter;
        }

        public IGauge Gauge(string name, string description, params string[] labels)
        {
            return Nulls.Gauge;
        }

        public IGauge Gauge(IGauge parent, string description, params string[] labels)
        {
            return Nulls.Gauge;
        }

        public IHistogram Histogram(string name, string description, double[] buckets, params string[] labels)
        {
            return Nulls.Histogram;
        }

        public IHistogram Histogram(IHistogram parent, string description, params string[] labels)
        {
            return Nulls.Histogram;
        }
    }

    public class NullCounter : ICounter
    {
        public string Name { get; }
        public string[] Labels { get; }
        public string Description { get; }
        public double Value { get; }
        public void Increment(double val = 1)
        {
        }
    }

    public class NullHistogram : IHistogram
    {
        public string Name { get; }
        public string[] Labels { get; }
        public string Description { get; }
        public void Observe(double val)
        {
        }

        public IDisposable Time()
        {
            return new DisposableTimer(Observe);
        }
    }

    public class NullGauge : IGauge
    {
        public string Name { get; }
        public string[] Labels { get; }
        public string Description { get; }
        public double Value { get; }
        public void Increment(double val = 1)
        {
        }

        public void Decrement(double val = 1)
        {
        }

        public void Set(double val)
        {
        }
    }
}
