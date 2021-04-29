using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace TaleLearnCode.AzureFunctionHealthCheck
{

	/// <summary>
	/// A builder used to register health checks.
	/// </summary>
	/// <seealso cref="IHealthChecksBuilder" />
	public class HealthChecksBuilder : IHealthChecksBuilder
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="HealthChecksBuilder"/> class.
		/// </summary>
		/// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> into which <see cref="T:Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck" /> instances should be registered.</param>
		public HealthChecksBuilder(IServiceCollection services)
		{
			Services = services;
		}

		/// <summary>
		/// Gets the <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> into which <see cref="T:Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck" /> instances should be registered.
		/// </summary>
		public IServiceCollection Services { get; }

		/// <summary>
		/// Adds a <see cref="T:Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckRegistration" /> for a health check.
		/// </summary>
		/// <param name="registration">The <see cref="T:Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckRegistration" />.</param>
		/// <returns>An initialized <see cref="HealthChecksBuilder"/></returns>
		/// <exception cref="ArgumentNullException">registration</exception>
		public IHealthChecksBuilder Add(HealthCheckRegistration registration)
		{
			if (registration == default) throw new ArgumentNullException(nameof(registration));
			Services.Configure<HealthCheckServiceOptions>(options =>
			{
				options.Registrations.Add(registration);
			});
			return this;
		}

	}

}