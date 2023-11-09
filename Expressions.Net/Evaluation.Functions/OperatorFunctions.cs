using Expressions.Net.Assemblies;
using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.Functions
{
	public static class OperatorFunctions
	{
		[OperatorFunction("+")]
		[ExpressionFunction("Sum(number, number):number")]
		[ExpressionFunction("string.Concat(any):string")]
		[ExpressionFunction("number.Add(number|boolean):number")]
		[ExpressionFunction("boolean.Add(number|boolean):number")]
		[ExpressionFunction("array<T>.Add(T):array<T>")]
		[ExpressionFunction("array<T>.Union(array<T>):array<T>")]
		[ExpressionFunction("object.Union(object):object")]
		public static IValue Add(this IValue arg0, IValue arg1)
		{
			// If BOTH are either numbers OR boolean, convert both values to numbers and do addition
			if (arg0.TryGetNumberOrBooleanAsNumber(out var number0) && arg1.TryGetNumberOrBooleanAsNumber(out var number1))
				return new NumberValue(number0 + number1);

			// If BOTH are arrays AND they have the same item type, do union
			if (arg0.Type.TryGetArrayItemType(out var arg0ItemType) && arg0.Type.TryGetArrayItemType(out var arg1ItemType) && arg1ItemType == arg0ItemType)
				return arg0.Union(arg1);

			// If arg0 is Array and arg1 is of arg0's ItemType
			if (arg0.IsArray() && arg1.Type == arg0ItemType)
				return arg0.Union(arg1);

			// If ANY is a string, convert both values to string and perform concatination
			if (arg0.IsString() || arg1.IsString())
				return new StringValue(arg0.Data?.ToString() + arg1.Data?.ToString());

			// If BOTH are object do union
			if (arg0.IsObject() && arg1.IsObject())
				return arg0.Union(arg1);

			return InvalidValue.FunctionNotSupportedForArgs(nameof(Add), arg0, arg1);
		}

		[OperatorFunction("&&")]
		[ExpressionFunction("boolean.And(boolean|number):boolean")]
		[ExpressionFunction("number.And(boolean|number):boolean")]
		public static IValue And(this IValue arg0, IValue arg1)
		{
			if (arg0.TryGetNumberOrBooleanAsBoolean(out var bool0) && arg1.TryGetNumberOrBooleanAsBoolean(out var bool1))
				return new BooleanValue(bool0.Value && bool1.Value);

			return InvalidValue.FunctionNotSupportedForArgs(nameof(Add), arg0, arg1);
		}

		public static IValue Coalesce(this IValue arg0, IValue arg1) => throw new NotImplementedException();

		[OperatorFunction("/")]
		[ExpressionFunction("number.Divide(number):number")]
		public static IValue Divide(this IValue arg0, IValue arg1)
		{
			if (arg0.IsNumber() && arg1.IsNumber())
				return new NumberValue(arg0.AsNumber() / arg1.AsNumber());

			return InvalidValue.FunctionNotSupportedForArgs(nameof(Add), arg0, arg1);
		}

		[OperatorFunction("==")]
		[ExpressionFunction("any.Equals(any):boolean")]
		public static IValue Equal(this IValue arg0, IValue arg1)
		{
			return new BooleanValue(arg0 == arg1);
		}

		public static IValue GreaterThan(this IValue arg0, IValue arg1) => throw new NotImplementedException();
		public static IValue GreaterThanOrEqual(this IValue arg0, IValue arg1) => throw new NotImplementedException();
		public static IValue LessThan(this IValue arg0, IValue arg1) => throw new NotImplementedException();
		public static IValue LessThanOrEqual(this IValue arg0, IValue arg1) => throw new NotImplementedException();
		public static IValue Modulus(this IValue arg0, IValue arg1) => throw new NotImplementedException();

		[OperatorFunction("*")]
		[ExpressionFunction("number.Multiply(number|boolean):number")]
		[ExpressionFunction("boolean.Multiply(number|boolean):number")]
		public static IValue Multiply(this IValue arg0, IValue arg1)
		{
			if (arg0.TryGetNumberOrBooleanAsNumber(out var arg0Number) && arg1.TryGetNumberOrBooleanAsNumber(out var arg1Number))
				return new NumberValue(arg0Number * arg1Number);

			return InvalidValue.FunctionNotSupportedForArgs(nameof(Add), arg0);
		}

		[OperatorFunction("!")]
		[ExpressionFunction("boolean.Not():boolean")]
		public static IValue Not(this IValue arg0)
		{
			if (arg0.TryGetAsBoolean(out var value))
				return new BooleanValue(!value);

			return InvalidValue.FunctionNotSupportedForArgs(nameof(Add), arg0);
		}

		public static IValue NotEqual(this IValue arg0, IValue arg1) => throw new NotImplementedException();
		public static IValue Or(this IValue arg0, IValue arg1) => throw new NotImplementedException();
		public static IValue Subtract(this IValue arg0, IValue arg1) => throw new NotImplementedException();
	}
}
