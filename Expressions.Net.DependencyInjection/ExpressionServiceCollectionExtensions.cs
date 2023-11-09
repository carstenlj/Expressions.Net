using Microsoft.Extensions.DependencyInjection;

namespace Expressions.Net.DependencyInjection
{
	public static class ExpressionServiceCollectionExtensions
	{
		public static IServiceCollection AddExpressions(this IServiceCollection services)
		{
			// Add settings
			services.AddSingleton(typeof(IExpressionSettings), ExpressionServiceImplementations.GetDefaultValueConverterSettings());

			// Add services
			var implementations = ExpressionServiceImplementations.GetDefaults();
			foreach (var implementation in implementations)
				services.AddSingleton(implementation.Key, implementation.Value);

			return services;
		}
	}
}
