using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValueTypes;
using System.Collections.Generic;

namespace Expressions.Net
{
	public interface IExpressionFactory
	{
		ExpressionDelegate CreateDelegate(string expression, IDictionary<string, IValueType>? schema = null);
		IVariables CreateVariables(object? values, IDictionary<string, IValueType>? schema);
	}

	public static class IExpressionFactoryExtensions
	{
		public static IValueType GetStringType(this IExpressionFactory factory) => StringType.Invariant;
		public static IValueType GetNumberType(this IExpressionFactory factory) => NumberType.Invariant;
		public static IValueType GetBooleanType(this IExpressionFactory factory) => BooleanType.Invariant;
		public static IValueType GetArrayType(this IExpressionFactory factory, IValueType itemType) => new ArrayType(itemType);
		public static IValueType GetObjectType(this IExpressionFactory factory, IDictionary<string, IValueType> schema) => new ObjectType(schema);
		public static IValueType GetAmbigousType(this IExpressionFactory factory, params IValueType[] types) => new AmbiguousValueType(types);
	}
}
