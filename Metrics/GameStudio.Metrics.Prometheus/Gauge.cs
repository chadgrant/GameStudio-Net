using PrometheusNet = global::Prometheus;

namespace GameStudio.Metrics.Prometheus
{
	public class Gauge : IGauge
	{
		internal PrometheusNet.Gauge PrometheusGauge { get; }

		public string Name => PrometheusGauge.Name;
		public string Description => PrometheusGauge.Help;
		public string[] Labels => PrometheusGauge.LabelNames;
		public double Value => PrometheusGauge.Value;

		public Gauge(string name, string description, params string[] labels)
		{
			PrometheusGauge = PrometheusNet.Metrics.CreateGauge(name,description,labels);
		}

		public void Increment(double val = 1) => PrometheusGauge.Inc(val);

		public void Decrement(double val = 1) => PrometheusGauge.Dec(val);

		public void Set(double val) => PrometheusGauge.Set(val);
	}

	public class ChildGauge : IGauge
	{
		internal PrometheusNet.Gauge.Child Child { get; }

		public string Name { get; }
		public string Description { get; }
		public string[] Labels { get; }
		public double Value => Child.Value;

		public ChildGauge(Gauge parent, string name, string description, string[] labels)
		{
			Child = parent.PrometheusGauge.WithLabels(labels);
			Labels = labels;
			Description = description;
			Name = name;
		}

		public void Increment(double val = 1) => Child.Inc(val);

		public void Decrement(double val = 1) => Child.Dec(val);

		public void Set(double val) => Child.Set(val);
	}
}
