using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Expressions.Net.Evaluation
{
	public interface IValue
	{
		IValueType Type { get; }
		object? Data { get; }
	}

	public static class IValueExtensions
	{
		public static bool TryGetAsString(this IValue obj, [NotNullWhen(true)] out string? value) => (value = obj.Data as string) != null;
		public static bool TryGetAsNumber(this IValue obj, [NotNullWhen(true)] out double? value) => (value = obj.Data as double?) != null;
		public static bool TryGetAsBoolean(this IValue obj, [NotNullWhen(true)] out bool? value) => (value = obj.Data as bool?) != null;
		public static bool TryGetAsArray(this IValue obj, [NotNullWhen(true)] out IList<IValue>? value) => (value = obj.Data as IList<IValue>) != null;
		public static bool TryGetAsObject(this IValue obj, [NotNullWhen(true)] out IDictionary<string, IValue>? value) => (value = obj.Data as IDictionary<string, IValue>) != null;

		public static bool TryGetNumberOrBooleanAsNumber(this IValue obj, [NotNullWhen(true)] out double? value)
		{
			if (obj.IsNumber())
				value = obj.AsNumber();
			else if (obj.IsBoolean())
				value = obj.AsBoolean() ? 1d : 0d;
			else
				value = null;

			return value != null;
		}

		public static bool TryGetNumberOrBooleanAsBoolean(this IValue obj, [NotNullWhen(true)] out bool? value)
		{
			if (obj.IsNumber())
				value = obj.AsNumber() > 0;
			else if (obj.IsBoolean())
				value = obj.AsBoolean();
			else
				value = null;

			return value != null;
		}

		public static bool IsString(this IValue obj) => obj.Type.RootType == ValueRootType.String;
		public static bool IsNumber(this IValue obj) => obj.Type.RootType == ValueRootType.Number;
		public static bool IsBoolean(this IValue obj) => obj.Type.RootType == ValueRootType.Boolean;
		public static bool IsNumberOrBoolean(this IValue obj) => obj.Type.RootType == ValueRootType.Number || obj.Type.RootType == ValueRootType.Boolean;
		public static bool IsArray(this IValue obj) => obj.Type.RootType == ValueRootType.Array;
		public static bool IsObject(this IValue obj) => obj.Type.RootType == ValueRootType.Object;

		public static double AsNumber(this IValue value) => value.Data is double tvalue ? tvalue : throw IncorrectTypeException(value, "double");
		public static string AsString(this IValue value) => value.Data is string tvalue ? tvalue : throw IncorrectTypeException(value, "string");
		public static bool AsBoolean(this IValue value) => value.Data is bool tvalue ? tvalue : throw IncorrectTypeException(value, "bool");
		public static IList<IValue> AsArray(this IValue value) => value.Data is IList<IValue> tvalue ? tvalue : throw IncorrectTypeException(value, "IList<IValue>");
		public static IDictionary<string, IValue> AsObject(this IValue value) => value.Data is IDictionary<string, IValue> tvalue ? tvalue : throw IncorrectTypeException(value, "IDictionary<string,IValue>");

		private static Exception IncorrectTypeException(IValue value, string targetType) => new InvalidOperationException($"Cannot retrieve the value '{value?.ToString()}' as {targetType} because its type is {value?.Data?.GetType().Name} ({value?.Type.ToString()})");
	}
}
