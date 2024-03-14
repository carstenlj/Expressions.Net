using Expressions.Net.Compilation;
using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization;
using Microsoft.Extensions.DependencyInjection;

namespace Expressions.Net.DependencyInjection
{
	public static class ExpressionServiceCollectionExtensions
	{
		public static IServiceCollection AddExpressionEngine(this IServiceCollection services)
		{
			return services
				.AddSingleton(typeof(IExpressionSettings), ExpressionSettings.CreateDefault())
				.AddSingleton<IValueConverter, ValueConverter>()
				.AddSingleton<IValueTypeConverter, ValueTypeConverter>()
				.AddSingleton<IFunctionsProvider, FunctionsProvider>()
				.AddSingleton<IOperatorProvider, OperatorProvider>()
				.AddSingleton<IExpressionEngine, ExpressionEngine>()
				.AddSingleton<IExpressionCompiler, ExpressionILCompiler>()
				.AddSingleton<IExpressionTokenizer, ExpressionTokenizer>()
				.AddSingleton<IExpressionCache, ExpressionCache>()
				.AddSingleton<IStringTokenizer, StringTokenizer>()
				.AddSingleton<IKeywordTokenizer, KeywordTokenizer>();
		}
	}
}
