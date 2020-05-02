using System;

namespace GameStudio.Metrics
{
	public interface IHistogram : IMetric
	{
		void Observe(double val);

		IDisposable Time();
	}
}