using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaleLearnCode.AzureFunctionHealthCheck
{

	public class DefaultHealthCheckService : HealthCheckService
	{

		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IOptions<HealthCheckServiceOptions> _options;
		private readonly ILogger<DefaultHealthCheckService> _logger;

		public DefaultHealthCheckService(
			IServiceScopeFactory serviceScopeFactory,
			IOptions<HealthCheckServiceOptions> options,
			ILogger<DefaultHealthCheckService> logger)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_logger = logger ?? throw new ArgumentNullException(nameof(options));
			ValidateRegistrations(_options.Value.Registrations);
		}

		/// <summary>
		/// Performs the health check.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public override async Task<HealthReport> CheckHealthAsync(
			Func<HealthCheckRegistration, bool> predicate,
			CancellationToken cancellationToken = default)
		{

			ICollection<HealthCheckRegistration> registrations = _options.Value.Registrations;

			using IServiceScope scope = _serviceScopeFactory.CreateScope();

			HealthCheckContext healthCheckContext = new();
			Dictionary<string, HealthReportEntry> entries = new(StringComparer.OrdinalIgnoreCase);

			ValueStopwatch totalTime = ValueStopwatch.StartNew();
			Log.HealthCheckProcessingBegin(_logger);

			foreach (HealthCheckRegistration registration in registrations)
			{
				if (predicate != null && !predicate(registration))
					continue;

				cancellationToken.ThrowIfCancellationRequested();

				IHealthCheck healthCheck = registration.Factory(scope.ServiceProvider);

				ValueStopwatch stopwatch = ValueStopwatch.StartNew();
				healthCheckContext.Registration = registration;

				Log.HealthCheckBegin(_logger, registration);

				HealthReportEntry entry;
				try
				{
					HealthCheckResult result = await healthCheck.CheckHealthAsync(healthCheckContext, cancellationToken);
					TimeSpan duration = stopwatch.Elapsed;

					entry = new HealthReportEntry(
						status: result.Status,
						description: result.Description,
						duration: duration,
						exception: result.Exception,
						data: result.Data);

					Log.HealthCheckEnd(_logger, registration, entry, duration);
					Log.HealthCheckData(_logger, registration, entry);
				}
				catch (Exception ex) when (ex as OperationCanceledException == null)
				{
					var duration = stopwatch.Elapsed;
					entry = new HealthReportEntry(
						status: HealthStatus.Unhealthy,
						description: ex.Message,
						duration: duration,
						exception: ex,
						data: null);

					Log.HealthCheckError(_logger, registration, ex, duration);
				}
				entries[registration.Name] = entry;
			}

			var totalElapsedTime = totalTime.Elapsed;
			var report = new HealthReport(entries, totalElapsedTime);
			Log.HealthCheckProcessingEnd(_logger, report.Status, totalElapsedTime);
			return report;

		}

		private static void ValidateRegistrations(IEnumerable<HealthCheckRegistration> registrations)
		{

			List<string> duplicateNames = registrations
				.GroupBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key)
				.ToList();

			if (duplicateNames.Any())
				throw new ArgumentException($"Duplicate health checks were registered with the name(s): {string.Join(", ", duplicateNames)}", nameof(registrations));

		}

	}

}