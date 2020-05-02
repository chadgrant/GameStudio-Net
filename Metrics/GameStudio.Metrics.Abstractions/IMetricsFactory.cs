namespace GameStudio.Metrics
{
	public interface IMetricsFactory
	{
		ICounter Counter(string name, string description, params string[] labels);

		ICounter Counter(ICounter parent, string description, params string[] labels);

		IGauge Gauge(string name, string description, params string[] labels);

		IGauge Gauge(IGauge parent, string description, params string[] labels);

		IHistogram Histogram(string name, string description, double[] buckets, params string[] labels);

		IHistogram Histogram(IHistogram parent, string description, params string[] labels);
	}
}