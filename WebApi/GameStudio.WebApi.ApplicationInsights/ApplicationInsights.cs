using System.Collections.Generic;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameStudio.ConfigurationConventions;
using GameStudio.WebApi.ApplicationInsights.Telemetry;

namespace GameStudio.WebApi.ApplicationInsights
{
	public class ApplicationInsights : IConfigureServices, IConfigure
	{
		const string HealthReadinessUri = "ready";
		const string HealthLivesnessUri = "live";

		bool _appInsightEnabled;

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			_appInsightEnabled = configuration.GetSection("applicationInsights") != null;
			if (_appInsightEnabled)
			{
				services.AddApplicationInsightsTelemetry(configuration);
				services.AddApplicationInsightsTelemetryProcessor<TelemetryProcessor>();
			}
		}

		public void Configure(IConfiguration configuration, IApplicationBuilder app, IHostingEnvironment env)
		{
			if (_appInsightEnabled)
			{
				var telctx = CreateTelemetryContext();
				var config = app.ApplicationServices.GetService<TelemetryConfiguration>();
				config.TelemetryProcessorChainBuilder.Use(next => new TelemetryProcessor(next) { _context = telctx });
				config.TelemetryProcessorChainBuilder.Build();
			}
		}

		internal static TelemetryContext CreateTelemetryContext()
		{
			var context = new TelemetryContext()
			{
				TelemetryConditions = new List<TelemetryCondition>()
				{
					//Condition-1
					new TelemetryCondition ()
					{
						Type = TelemetryType.TraceTelemetry,
						Condition = new Dictionary<LeftOperand, IEnumerable<string>>()
						{
							{ LeftOperand.Path,new List<string>(2){ HealthReadinessUri, HealthLivesnessUri} },
							{ LeftOperand.EndpointName,new List<string>(2){ HealthReadinessUri, HealthLivesnessUri} },
							{ LeftOperand.OperationName,new List<string>(2){ HealthReadinessUri, HealthLivesnessUri} },
						}
					},
					//Condition-2
				}
			};
			return context;
		}
	}
}
