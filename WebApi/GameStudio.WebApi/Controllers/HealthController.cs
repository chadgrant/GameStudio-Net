using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStudio.WebApi
{
	[ApiVersionNeutral, Route("[controller]"), ApiController]
	public sealed class HealthController : ControllerBase
	{
		static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
		static readonly TimeSpan ReportStaleAfter = TimeSpan.FromSeconds(10);
		static HealthCheckReport _lastReport;

		readonly HealthCheckService _healthService;
		
		public HealthController(HealthCheckService healthService)
		{
			_healthService = healthService;
		}

		/// <summary>
		/// The "Good To Go" (GTG) returns a successful response in the case that the service is in an operational state and is able to receive traffic.
		/// This resource is used by load balancers and monitoring tools to determine if traffic should be routed to this service or not.
		/// Note that GTG is not used to determine if the service is healthy or not, only if it is able to receive traffic.
		/// A healthy instance may not be able to accept traffic due to the failure of critical downstream dependencies.
		/// Failure to respond within a predetermined timeout typically 2 seconds is also treated as a failure.
		/// </summary>
		/// <response code="200">A successful response with "OK" body and text/plain content type</response>
		/// <response code="500">failed response</response>
		[HttpGet("gtg")]
		[Produces("text/plain")]
		[ProducesResponse(HttpStatus.OK)]
		[ProducesResponse(HttpStatus.InternalServerError)]
		public ActionResult<string> GoodToGo()
		{
			return Ok("OK");
		}

		/// <summary>
		/// The "Service Canary" (ASG) returns a successful response in the case that the service is in a healthy state.
		/// If a service returns a failure response or fails to respond within a predefined timeout then the service can expect to be terminated and replaced.
		/// (Typically this resouce is used in auto-scaling group healthchecks.)
		/// </summary>
		/// <response code="200">A successful response with "OK" body and text/plain content type</response>
		/// <response code="500">failed response</response>
		[HttpGet("asg")]
		[Produces("text/plain")]
		[ProducesResponse(HttpStatus.OK)]
		[ProducesResponse(HttpStatus.InternalServerError)]
		public async Task<ActionResult<string>> ServiceCanaryAsync(CancellationToken cancel)
		{
			var report = await GetHealthReportAsync(cancel);
			if (report.Tests.All(r => r.Result.Equals("passed", StringComparison.OrdinalIgnoreCase) || r.Result.Equals("healthy", StringComparison.OrdinalIgnoreCase)))
				return Ok("OK");

			return StatusCode((int) HttpStatus.InternalServerError);
		}

		/// <summary>
		/// Returns the healthcheck report of the service.
		/// </summary>
		[HttpGet]
		[Produces("application/json")]
		[ProducesResponse(HttpStatus.OK)]
		[ProducesResponse(HttpStatus.InternalServerError)]
		public async Task<ActionResult<HealthCheckReport>> Get(CancellationToken cancel)
		{
			return Ok(await GetHealthReportAsync(cancel));
		}

		/// <summary>
		/// Gets a specific healthcheck result
		/// </summary>
		[HttpGet("{healthCheck}")]
		[Produces("application/json")]
		[ProducesResponse(HttpStatus.OK)]
		[ProducesResponse(HttpStatus.NotFound)]
		[ProducesResponse(HttpStatus.InternalServerError)]
		public async Task<ActionResult<HealthCheckResult>> Get([FromRoute] string healthCheck, CancellationToken cancel)
		{
			var results = (await _healthService.CheckHealthAsync(b => b.Name.Equals(healthCheck, StringComparison.OrdinalIgnoreCase),cancel));
			if (!results.Entries.ContainsKey(healthCheck))
				return NotFound($"Could not find health check: {healthCheck}");

			var result = results.Entries.First(e => e.Key.Equals(healthCheck, StringComparison.OrdinalIgnoreCase)).Value;

			return new HealthCheckResult
			{
				Name = healthCheck,
				DurationMilliseconds = (long) result.Duration.TotalMilliseconds,
				Result = result.Status.ToString(),
				TestedAt = TestedAtDate(result.Data)
			};
		}

		async Task<HealthCheckReport> GetHealthReportAsync(CancellationToken cancel = default(CancellationToken))
		{
			await Semaphore.WaitAsync(cancel);
			try
			{
				if (_lastReport == null || _lastReport.ReportAsOf <= DateTime.UtcNow.Subtract(ReportStaleAfter))
				{
					var stopwatch = new Stopwatch();
					stopwatch.Start();

					var results = await _healthService.CheckHealthAsync(cancel);

					_lastReport = new HealthCheckReport
					{
						ReportAsOf = DateTime.UtcNow,
						DurationMilliseconds = stopwatch.ElapsedMilliseconds,
						Tests = results.Entries.Select(r => new HealthCheckResult
						{
							Name = r.Key,
							DurationMilliseconds = (long) r.Value.Duration.TotalMilliseconds,
							Result = r.Value.Status.ToString(),
							TestedAt = TestedAtDate(r.Value.Data),
							Message = (r.Value.Exception != null) ? r.Value.Exception.ToString() : string.Empty,
						}).ToList()
					};
				}

				return _lastReport;
			}
			finally
			{
				Semaphore.Release();
			}
		}

		static DateTime TestedAtDate(IReadOnlyDictionary<string,object> data)
		{
			if (data.ContainsKey("testedAt"))
			{
				if (data["testedAt"] is DateTime time)
					return time;
			}

			return DateTime.UtcNow;
		}
	}
}
