using System;

namespace GameStudio.Metrics.Prometheus
{
	public class PrometheusMetricsFactory : IMetricsFactory
	{
		public ICounter Counter(string name, string description, params string[] labels)
		{
			if (name == null || string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

			return new Counter(name, description, labels);
		}

		public ICounter Counter(ICounter parent, string description, params string[] labels)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			var counter = parent as Counter;
			if (counter == null) throw new ArgumentException("Not a prometheus counter", nameof(parent));

			return new ChildCounter(counter, counter.Name, description, labels);
		}
		
		public IGauge Gauge(string name, string description, params string[] labels)
		{
			if (name == null || string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

			return new Gauge(name,description,labels);
		}

		public IGauge Gauge(IGauge parent, string description, params string[] labels)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			var gauge = parent as Gauge;
			if (gauge == null) throw new ArgumentException("Not a prometheus gauge", nameof(parent));

			return new ChildGauge(gauge, parent.Name, description, labels);
		}

		public IHistogram Histogram(string name, string description, double[] buckets, params string[] labels)
		{
			if (name == null || string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

			return new Histogram(name,description, buckets, labels);
		}

		public IHistogram Histogram(IHistogram parent, string description, params string[] labels)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			var histogram = parent as Histogram;
			if (histogram == null) throw new ArgumentException("Not a prometheus histogram", nameof(parent));

			return new ChildHistogram(histogram, description, labels);
		}
	}
}
