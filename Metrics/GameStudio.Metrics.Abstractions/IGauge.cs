namespace GameStudio.Metrics
{
	public interface IGauge : IMetric, IValue<double>
	{
		void Increment(double val = 1);
		void Decrement(double val = 1);
		void Set(double val);
	}
}