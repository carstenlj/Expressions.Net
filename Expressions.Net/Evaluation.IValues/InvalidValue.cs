using Expressions.Net.Evaluation.IValueTypes;
using System.Linq;

namespace Expressions.Net.Evaluation.IValues
{
	internal sealed class InvalidValue : IValue
	{
		public IValueType Type { get; }
		public object? Data { get; }
		public string? ErrorMessage { get; }

		internal InvalidValue(object? message)
		{
			Type = new InvalidType();
			Data = null;
			ErrorMessage = message?.ToString();
		}

		internal static InvalidValue FunctionNotSupportedForArgs(string functionName, params IValue[] args) => new InvalidValue($"Function '{functionName}' not supported for arguments of type {ArgTypes(args)}");
		internal static InvalidValue CannotUnionTwoArraysWithDifrentItemType(params IValue[] args) => new InvalidValue($"Cannot union two arrays with diffrent item types ({ItemTypes(args)})");
		internal static InvalidValue CannotResolveProperty(string propertyName) => new InvalidValue($"Cannot resolve property '{propertyName}'");
		internal static InvalidValue CannotResolveProperty(string propertyName, IValueType type) => new InvalidValue($"Cannot resolve property '{propertyName}' of type '{type}'");
		internal static InvalidValue CannotResolveValue(IValueType type) => new InvalidValue($"Cannot resolve of type '{type}'");
		internal static InvalidValue InvalidExpressionFunction() => new InvalidValue($"Invalid expression function");
		internal static InvalidValue InvalidExpressionFunction(string message) => new InvalidValue($"Invalid expression function ({message})");
		internal static InvalidValue VariableNotDeclared(string variable) => new InvalidValue($"Variable '{variable}' not declared");
		internal static InvalidValue OperatorOrOperandMissing() => new InvalidValue($"Operator or operand missing");

	
		private static string ArgTypes(params IValue[] args) => string.Join(",", args.Select(x => $"'{x.Type.RootType.ToString().ToLower()}'"));
		private static string ItemTypes(params IValue[] args) => string.Join(",", args.Select(x => $"'{(x.Type as ArrayType)?.ItemType?.ToString().ToLower()}'"));
	}
}

