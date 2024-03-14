using Expressions.Net.Compilation;
using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Expressions.Net
{
	public class ExpressionEngine : IExpressionEngine
	{
		public static readonly IExpressionEngine Default = CreateDefault();

		public IExpressionCompiler Compiler { get; }
		public IExpressionTokenizer Tokenizer { get; }

		private readonly ConcurrentDictionary<string, ExpressionDelegate> _cache = new ConcurrentDictionary<string, ExpressionDelegate>();
		private readonly IExpressionCache? _expressionCache;
		private readonly IValueConverter _valueConverter;

		public ExpressionEngine(IExpressionCompiler expressionCompiler, IExpressionTokenizer expressionTokenizer, IValueConverter valueConverter, IExpressionCache? expressionCache)
		{
			Compiler = expressionCompiler;
			Tokenizer = expressionTokenizer;
			_valueConverter = valueConverter;
			_expressionCache = expressionCache;
		}

		public ExpressionDelegate CompileExpressionToDelegate(string expression, IDictionary<string, IValueType>? schema = null)
		{
			return Compiler.Compile(Tokenizer.TokenizeToPostfix(expression), schema);
		}

		public IVariables CreateVariables(object? data, IDictionary<string, IValueType>? schema = null)
		{
			return new Variables(_valueConverter.ConvertToDictionary(data, schema) ?? new Dictionary<string, IValue>(StringComparer.OrdinalIgnoreCase));
		}

		public static IExpressionEngine CreateDefault()
		{
			return new ExpressionEngine(
				expressionCompiler: ExpressionILCompiler.Default,
				expressionTokenizer: ExpressionTokenizer.Default,
				valueConverter: ValueConverter.Default,
				expressionCache: ExpressionCache.Default
			);
		}
	}
}
