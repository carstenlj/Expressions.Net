using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValueTypes;
using Expressions.Net.Tokenization;
using System.Collections.Generic;

namespace Expressions.Net
{
	public interface IExpressionEngine
	{
		//IExpressionTokenizer Tokenizer { get; }
		//IExpressionCompiler Compiler{ get; }
		ExpressionDelegate CompileExpressionToDelegate(string expression, IDictionary<string, IValueType>? schema = null);
		IVariables CreateVariables(object? values, IDictionary<string, IValueType>? schema = null);
	}

	public static class IExpressionFactoryExtensions
	{
		public static IValueType GetStringType(this IExpressionEngine factory) => StringType.Invariant;
		public static IValueType GetNumberType(this IExpressionEngine factory) => NumberType.Invariant;
		public static IValueType GetBooleanType(this IExpressionEngine factory) => BooleanType.Invariant;
		public static IValueType GetArrayType(this IExpressionEngine factory, IValueType itemType) => new ArrayType(itemType);
		public static IValueType GetObjectType(this IExpressionEngine factory, IDictionary<string, IValueType> schema) => new ObjectType(schema);
		public static IValueType GetAmbigousType(this IExpressionEngine factory, params IValueType[] types) => new AmbiguousValueType(types);
	}
}
