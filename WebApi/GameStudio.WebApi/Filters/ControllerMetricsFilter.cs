using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using GameStudio.Metrics;

namespace GameStudio.WebApi
{
	public class ControllerMetricsFilter : IActionFilter
	{
		static readonly double[] Buckets = {
			0,
			0.1,
			0.2,
			0.4,
			0.6,
			0.8,
			1,
			2,
			3,
			5
		};

		const string MetricsStopwatch = "metrics_stopwatch";

		static readonly ConcurrentDictionary<string, ICounter> Counters = new ConcurrentDictionary<string, ICounter>(StringComparer.OrdinalIgnoreCase);
		static readonly ConcurrentDictionary<string, IHistogram> Histograms = new ConcurrentDictionary<string, IHistogram>(StringComparer.OrdinalIgnoreCase);

		readonly IMetricsFactory _factory;
		readonly ICounter _parentCounter;
		readonly IHistogram _parentHisto;
		
		public ControllerMetricsFilter(IMetricsFactory factory)
		{
			_factory = factory;

			_parentCounter = factory.Counter("mvc_controller_actions_total", "counts mvc requests", "code", "version", "controller", "action");
			_parentHisto = factory.Histogram("mvc_controller_actions_duration", "measures mvc request durations", Buckets, "version", "controller", "action");
		}
		
		public void OnActionExecuting(ActionExecutingContext context)
		{
			var watch = new Stopwatch();
			watch.Start();
			context.HttpContext.Items[MetricsStopwatch] = watch;
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			if (context.HttpContext.Items[MetricsStopwatch] is Stopwatch watch)
			{
				context.RouteData.Values.TryGetValue("version", out object version);
				context.RouteData.Values.TryGetValue("controller", out object controller);
				context.RouteData.Values.TryGetValue("action", out object action);
				
				UpdateMetrics(
					context.HttpContext.Response.StatusCode,
					version as string,
					controller as string,
					action as string,
					watch.Elapsed.TotalSeconds);
			}
		}

		void UpdateMetrics(int statusCode, string version, string controller, string action, double seconds)
		{
			if (_factory == null)
				return;

			if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
				return;

			if (string.IsNullOrEmpty(version))
				version = "1";

			//don't care about the statuscode dimension on histogram
			var ckey = $"{statusCode}-{version}-{controller}-{action}";
			var hkey = $"{version}-{controller}-{action}";

			action = action.ToLower().Replace("async", string.Empty);

			Counters.GetOrAdd(ckey, k => _factory.Counter(_parentCounter, string.Empty, statusCode.ToString(), version, controller.ToLower(), action)).Increment();
			Histograms.GetOrAdd(hkey, k => _factory.Histogram(_parentHisto, string.Empty, version, controller.ToLower(), action)).Observe(seconds);
		}
	}
}
