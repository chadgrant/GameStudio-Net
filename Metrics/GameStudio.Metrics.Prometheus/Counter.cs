using PrometheusNet = global::Prometheus;

namespace GameStudio.Metrics.Prometheus
{
	public class Counter : ICounter
	{
		internal PrometheusNet.Counter PrometheusCounter { get; }

		public string Name => PrometheusCounter.Name;
		public string Description => PrometheusCounter.Help;
		public string[] Labels => PrometheusCounter.LabelNames;
		public double Value => PrometheusCounter.Value;

		public Counter(string name, string description, params string[] labels)
		{
			PrometheusCounter = PrometheusNet.Metrics.CreateCounter(name, description, labels);
		}

		public void Increment(double val = 1) => PrometheusCounter.Inc(val);
	}

	public class ChildCounter : ICounter
	{
		internal PrometheusNet.Counter.Child Child { get; }

		public string Name { get; }
		public string Description { get; }
		public string[] Labels { get; }
		public double Value => Child.Value;

		public ChildCounter(Counter parent, string name, string description, string[] labels)
		{
			Child = parent.PrometheusCounter.WithLabels(labels);

			Name = name;
			Description = description;
			Labels = labels;
		}

		public void Increment(double val = 1) => Child.Inc(val);
	}
}
