using Expressions.Net.Compilation;
using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Generic;

namespace Expressions.Net.DependencyInjection
{
	public static class ExpressionServiceImplementations
	{
		public static IExpressionSettings GetDefaultValueConverterSettings() => ExpressionSettings.CreateDefault();

		public static IDictionary<Type, Type> GetDefaults() => new Dictionary<Type, Type> {
			{ typeof(IValueConverter), typeof(ValueConverter) },
			{ typeof(IValueTypeConverter), typeof(ValueTypeConverter) },
			{ typeof(IFunctionsProvider), typeof(FunctionsProvider) },
			{ typeof(IExpressionFactory), typeof(ExpressionFactory) },
			{ typeof(IExpressionCompiler), typeof(ExpressionCompiler) },
			{ typeof(IExpressionTokenizer), typeof(Tokenizer) },
			{ typeof(IStringTokenizer), typeof(StringTokenizer) }
		};
	}
}
