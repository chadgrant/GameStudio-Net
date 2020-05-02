using System;
using PrometheusNet = global::Prometheus;

namespace GameStudio.Metrics.Prometheus
{
	public class Histogram : IHistogram
	{
		internal PrometheusNet.Histogram PrometheusHistogram { get; }

		public string Name { get; }
		public string Description { get; }
		public string[] Labels => PrometheusHistogram.LabelNames;

		public Histogram(string name, string description, double[] buckets, params string[] labels)
		{
			var histoconfig = new PrometheusNet.HistogramConfiguration
			{
				Buckets = buckets,
				LabelNames = labels
			};

			histoconfig.Buckets = buckets;
			PrometheusHistogram = PrometheusNet.Metrics.CreateHistogram(name, description, histoconfig);
			Name = name;
			Description = description;
		}
		
		public void Observe(double val) => PrometheusHistogram.Observe(val);

		public IDisposable Time()
		{
			return new DisposableTimer(Observe);
		}
	}

	public class ChildHistogram : IHistogram
	{
		internal PrometheusNet.Histogram.Child Child { get; }

		public string Name { get; }
		public string[] Labels { get; }
		public string Description { get; }

		public ChildHistogram(Histogram parent, string description, params string[] labels)
		{
			Child = parent.PrometheusHistogram.WithLabels(labels);
			Name = parent.Name;
			Description = description;
			Labels = labels;
		}

		public void Observe(double val) => Child.Observe(val);

		public IDisposable Time()
		{
			return new DisposableTimer(Observe);
		}
	}
}
