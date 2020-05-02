using System;
using System.Diagnostics;

namespace GameStudio.Metrics
{
	public class DisposableTimer : IDisposable
	{
		readonly Action<double> _observe;
		readonly Stopwatch _stopwatch = new Stopwatch();

		public DisposableTimer(Action<double> observe)
		{
			_observe = observe;
			_stopwatch.Start();
		}

		public void Dispose()
		{
			_stopwatch.Stop();
			_observe(_stopwatch.Elapsed.TotalSeconds);
		}
	}
}
