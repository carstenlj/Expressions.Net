using Expressions.Net.Compilation;
using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Net
{
	public class ExpressionCache : IExpressionCache
	{
		public static readonly IExpressionCache Default = new ExpressionCache(
			expressionCompiler:ExpressionILCompiler.Default,
			expressionTokenizer: ExpressionTokenizer.Default
		);

		private readonly ConcurrentDictionary<string, ExpressionDelegate> _cache = new ConcurrentDictionary<string, ExpressionDelegate>();
		private readonly IExpressionCompiler _expressionCompiler;
		private readonly IExpressionTokenizer _expressionTokenizer;

		public ExpressionCache(IExpressionCompiler expressionCompiler, IExpressionTokenizer expressionTokenizer)
		{
			_expressionCompiler = expressionCompiler;
			_expressionTokenizer = expressionTokenizer;
		}

		public ExpressionDelegate GetOrAdd(string expression, IDictionary<string, IValueType>? schema = null)
		{
			var key = expression + (schema == null ? string.Empty : CreateSchemaHash(schema));
			if (_cache.TryGetValue(key, out var expressionDelegate))
				return expressionDelegate;

			_cache.AddOrUpdate(key, Compile(expression, schema), (key, value) => Compile(expression, schema));
			return _cache[expression];
		}

		private string CreateSchemaHash(IDictionary<string, IValueType> schema)
		{
			var stringBuilder = new StringBuilder();
			foreach(var item in schema)
			{
				stringBuilder.Append($"{item.Key};{item.Value}");
			}

			return $";{stringBuilder.ToString().GetHashCode()}";
		}

		private ExpressionDelegate Compile(string expression, IDictionary<string, IValueType>? schema = null)
		{
			return _expressionCompiler.Compile(_expressionTokenizer.TokenizeToPostfix(expression), schema) ?? throw new InvalidOperationException();
		}
	}
}
