using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace TaleLearnCode.AzureFunctionHealthCheck
{

	internal static class Log
	{
		private static readonly Action<ILogger, Exception> _healthCheckProcessingBegin = LoggerMessage.Define(
				LogLevel.Debug,
				EventIds.HealthCheckProcessingBegin,
				"Running health checks");

		private static readonly Action<ILogger, double, HealthStatus, Exception> _healthCheckProcessingEnd = LoggerMessage.Define<double, HealthStatus>(
				LogLevel.Debug,
				EventIds.HealthCheckProcessingEnd,
				"Health check processing completed after {ElapsedMilliseconds}ms with combined status {HealthStatus}");

		private static readonly Action<ILogger, string, Exception> _healthCheckBegin = LoggerMessage.Define<string>(
				LogLevel.Debug,
				EventIds.HealthCheckBegin,
				"Running health check {HealthCheckName}");

		// These are separate so they can have different log levels
		private static readonly string HealthCheckEndText = "Health check {HealthCheckName} completed after {ElapsedMilliseconds}ms with status {HealthStatus} and '{HealthCheckDescription}'";

		private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndHealthy = LoggerMessage.Define<string, double, HealthStatus, string>(
				LogLevel.Debug,
				EventIds.HealthCheckEnd,
				HealthCheckEndText);

		private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndDegraded = LoggerMessage.Define<string, double, HealthStatus, string>(
				LogLevel.Warning,
				EventIds.HealthCheckEnd,
				HealthCheckEndText);

		private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndUnhealthy = LoggerMessage.Define<string, double, HealthStatus, string>(
				LogLevel.Error,
				EventIds.HealthCheckEnd,
				HealthCheckEndText);

		private static readonly Action<ILogger, string, double, HealthStatus, string, Exception> _healthCheckEndFailed = LoggerMessage.Define<string, double, HealthStatus, string>(
				LogLevel.Error,
				EventIds.HealthCheckEnd,
				HealthCheckEndText);

		private static readonly Action<ILogger, string, double, Exception> _healthCheckError = LoggerMessage.Define<string, double>(
				LogLevel.Error,
				EventIds.HealthCheckError,
				"Health check {HealthCheckName} threw an unhandled exception after {ElapsedMilliseconds}ms");

		public static void HealthCheckProcessingBegin(ILogger logger)
		{
			_healthCheckProcessingBegin(logger, null);
		}

		public static void HealthCheckProcessingEnd(ILogger logger, HealthStatus status, TimeSpan duration)
		{
			_healthCheckProcessingEnd(logger, duration.TotalMilliseconds, status, null);
		}

		public static void HealthCheckBegin(ILogger logger, HealthCheckRegistration registration)
		{
			_healthCheckBegin(logger, registration.Name, null);
		}

		public static void HealthCheckEnd(ILogger logger, HealthCheckRegistration registration, HealthReportEntry entry, TimeSpan duration)
		{
			switch (entry.Status)
			{
				case HealthStatus.Healthy:
					_healthCheckEndHealthy(logger, registration.Name, duration.TotalMilliseconds, entry.Status, entry.Description, null);
					break;

				case HealthStatus.Degraded:
					_healthCheckEndDegraded(logger, registration.Name, duration.TotalMilliseconds, entry.Status, entry.Description, null);
					break;

				case HealthStatus.Unhealthy:
					_healthCheckEndUnhealthy(logger, registration.Name, duration.TotalMilliseconds, entry.Status, entry.Description, null);
					break;
			}
		}

		public static void HealthCheckError(ILogger logger, HealthCheckRegistration registration, Exception exception, TimeSpan duration)
		{
			_healthCheckError(logger, registration.Name, duration.TotalMilliseconds, exception);
		}

		public static void HealthCheckData(ILogger logger, HealthCheckRegistration registration, HealthReportEntry entry)
		{
			if (entry.Data.Count > 0 && logger.IsEnabled(LogLevel.Debug))
			{
				logger.Log(
						LogLevel.Debug,
						EventIds.HealthCheckData,
						new HealthCheckDataLogValue(registration.Name, entry.Data),
						null,
						(state, ex) => state.ToString());
			}
		}
	}

}