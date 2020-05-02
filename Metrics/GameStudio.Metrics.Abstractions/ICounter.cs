using GameStudio.Metrics;

namespace GameStudio.Metrics
{
	public interface ICounter : IMetric, IValue<double>
	{
		void Increment(double val = 1);
	}
}