using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStudio.HealthChecks.Mongo
{
	public class MongoHealthCheck : IHealthCheck
	{
		readonly IOptionsMonitor<MongoHealthCheckOptions> _options;

		public MongoHealthCheck(IOptionsMonitor<MongoHealthCheckOptions> options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
		{
			var opt = _options.CurrentValue;
			var url = new MongoUrl(opt.ConnectionString);

			var t = PingAsync(url, cancellationToken);
			if (t.Wait((int)url.ConnectTimeout.TotalMilliseconds, cancellationToken) && t.Result != null)
				return Task.FromResult(HealthCheckResult.Healthy());

			return Task.FromResult(HealthCheckResult.Unhealthy());
		}

		Task<BsonDocument> PingAsync(MongoUrl url, CancellationToken cancellationToken = default(CancellationToken))
		{
			var client = new MongoClient(MongoClientSettings.FromUrl(url));
			var db = client.GetDatabase(url.DatabaseName);
			return db.RunCommandAsync((Command<BsonDocument>) "{ping:1}", cancellationToken: cancellationToken);			
		}
	}
}
