using Expressions.Net.Compilation;
using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Generic;

namespace Expressions.Net
{
	public sealed class ExpressionFactory : IExpressionFactory
	{
		public static readonly IExpressionFactory Default = CreateDefault();

		private IExpressionCompiler ExpressionCompiler { get; }
		private IExpressionTokenizer ExpressionTokenizer { get; }
		private IValueConverter ValueConverter { get; }

		public ExpressionFactory(IExpressionCompiler expressionCompiler,  IExpressionTokenizer expressionTokenizer, IValueConverter valueConverter)
		{
			ExpressionCompiler = expressionCompiler;
			ExpressionTokenizer = expressionTokenizer;
			ValueConverter = valueConverter;
		}

		public ExpressionDelegate CreateDelegate(string expression, IDictionary<string,IValueType>? schema = null)
		{
			return ExpressionCompiler.Compile(ExpressionTokenizer.TokenizeToPostfix(expression), schema);
		}

		public IVariables CreateVariables(object? data, IDictionary<string, IValueType>? schema)
		{
			return new Variables(ValueConverter.ConvertToDictionary(data, schema) ?? new Dictionary<string, IValue>(StringComparer.OrdinalIgnoreCase));
		}

		public static IExpressionFactory CreateDefault()
		{
			return new ExpressionFactory(
				expressionCompiler: new ExpressionCompiler(
					operatorProvider: OperatorProvider.Default,
					functionsProvider: new FunctionsProvider()
				),
				expressionTokenizer: new Tokenizer(
					stringTokenizer: StringTokenizer.Default,
					keywordTokenizer: KeywordTokenizer.Default,
					operatorProvider: OperatorProvider.Default
				),
				valueConverter: new ValueConverter(
					typeConverter: new ValueTypeConverter(),
					settings: ExpressionSettings.CreateDefault()
				)
			);
		}
	}
}
