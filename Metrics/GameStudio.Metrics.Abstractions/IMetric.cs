namespace GameStudio.Metrics
{
	public interface IMetric
	{
		string Name { get; }
		string[] Labels { get; }
		string Description { get; }
	}

	public interface IValue<out T>
	{
		T Value { get; }
	}
}