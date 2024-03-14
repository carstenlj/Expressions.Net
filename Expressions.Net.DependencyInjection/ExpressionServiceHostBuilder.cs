using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Expressions.Net.DependencyInjection
{
	public static class ExpressionServiceHostBuilder
	{
		public static IHostBuilder CreateDefault()
		{
			return Host.CreateDefaultBuilder().ConfigureServices(AddServices);
		}

		private static void AddServices(IServiceCollection services)
		{
			services.AddExpressionEngine();
		}
	}
}
