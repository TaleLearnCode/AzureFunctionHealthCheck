using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaleLearnCode.AzureFunctionHealthCheck
{

	/// <summary>
	/// Provides extension methods for registering <see cref="HealthCheckService"/>.
	/// </summary>
	public static class HealthCheckServiceFunctionExtension
	{

		/// <summary>
		/// Adds the <see cref="HealthCheckService"/> to the container, using the provided delegates to register.
		/// </summary>
		/// <param name="services">The services.</param>
		public static IHealthChecksBuilder AddFunctionHealthChecks(this IServiceCollection services)
		{
			services.TryAddSingleton<HealthCheckService, DefaultHealthCheckService>();
			return new HealthChecksBuilder(services);
		}

	}
}
