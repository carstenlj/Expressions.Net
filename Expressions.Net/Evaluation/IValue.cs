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
		public static bool TryGetAsDateTime(this IValue obj, [NotNullWhen(true)] out DateTime? value) => (value = obj.Data as DateTime?) != null;
		public static bool TryGetAsBoolean(this IValue obj, [NotNullWhen(true)] out bool? value) => (value = obj.Data as bool?) != null;
		public static bool TryGetAsArray(this IValue obj, [NotNullWhen(true)] out IList<IValue>? value) => (value = obj.Data as IList<IValue>) != null;
		public static bool TryGetAsObject(this IValue obj, [NotNullWhen(true)] out IDictionary<string, IValue>? value) => (value = obj.Data as IDictionary<string, IValue>) != null;

		public static bool TryGetNumberOrBooleanAsNumber(this IValue obj, [NotNullWhen(true)] out double? value)
		{
			if (obj.IsNumber() && !obj.IsEmpty())
				value = obj.AsDouble();
			else if (obj.IsBoolean() && !obj.IsEmpty())
				value = obj.AsBoolean() ? 1d : 0d;
			else
				value = null;

			return value != null;
		}

		public static bool TryGetNumberOrBooleanAsBoolean(this IValue obj, [NotNullWhen(true)] out bool? value)
		{
			if (obj.IsNumber())
				value = obj.AsDouble() > 0;
			else if (obj.IsBoolean())
				value = obj.AsBoolean();
			else
				value = null;

			return value != null;
		}

		public static bool IsString(this IValue obj) => obj.Type.RootType == ValueRootType.String;
		public static bool IsNumber(this IValue obj) => obj.Type.RootType == ValueRootType.Number;
		public static bool IsBoolean(this IValue obj) => obj.Type.RootType == ValueRootType.Boolean;
		public static bool IsDateTime(this IValue obj) => obj.Type.RootType == ValueRootType.DateTime;
		public static bool IsTruthfulBoolean(this IValue obj) => obj.IsBoolean() && ((bool?)obj.Data) == true;
		public static bool IsNumberOrBoolean(this IValue obj) => obj.Type.RootType == ValueRootType.Number || obj.Type.RootType == ValueRootType.Boolean;
		public static bool IsArray(this IValue obj) => obj.Type.RootType == ValueRootType.Array;
		public static bool IsObject(this IValue obj) => obj.Type.RootType == ValueRootType.Object;

		public static bool IsEmpty(this IValue? obj) => (obj == null || obj.Data == null || obj.Data is string str && string.IsNullOrWhiteSpace(str));

		public static DateTime AsDateTime(this IValue value) => value.Data is DateTime tvalue ? tvalue : throw IncorrectTypeException(value, "datetime");
		public static int AsInteger(this IValue value) => value.Data is double tvalue ? (int)tvalue : throw IncorrectTypeException(value, "integer");
		public static double AsDouble(this IValue value) => value.Data is double tvalue ? tvalue : throw IncorrectTypeException(value, "double");
		public static decimal AsDecimal(this IValue value) => value.Data is double tvalue ? (decimal)tvalue: throw IncorrectTypeException(value, "decimal");
		public static string AsString(this IValue value) => value.Data is string tvalue ? tvalue : throw IncorrectTypeException(value, "string");
		public static char AsChar(this IValue value) => value.Data is string tvalue  ? tvalue[0] : throw IncorrectTypeException(value, "char");
		public static bool AsBoolean(this IValue value) => value.Data is bool tvalue ? tvalue : throw IncorrectTypeException(value, "bool");
		public static IList<IValue> AsArray(this IValue value) => value.Data is IList<IValue> tvalue ? tvalue : throw IncorrectTypeException(value, "IList<IValue>");
		public static IDictionary<string, IValue> AsObject(this IValue value) => value.Data is IDictionary<string, IValue> tvalue ? tvalue : throw IncorrectTypeException(value, "IDictionary<string,IValue>");

		public static string AsStringOrDefault(this IValue? value, string @default) => value?.Data is string tvalue ? tvalue : @default;
		public static int AsIntegerOrDefault(this IValue? value, int @default) => value?.Data is double tvalue ? (int)tvalue : @default;
		public static double AsDoubleOrDefault(this IValue? value, double @default) => value?.Data is double tvalue ? tvalue : @default;

		public static int ConvertToIntegerOrDefault(this IValue? value, int @default = 0) => (int)ConvertToDoubleOrDefault(value, @default);
		public static decimal ConvertToDecimalOrDefault(this IValue? value, decimal @default = 0) => (decimal)ConvertToDoubleOrDefault(value, (double)@default);
		public static double ConvertToDoubleOrDefault(this IValue? value, double @default = 0d)
		{
			if (value == null)
				return @default;

			if (value.IsNumber())
				return value.AsDouble();

			if (value.IsDateTime())
				return (int)value.AsDateTime().Subtract(new DateTime(1970, 1, 1)).TotalDays;

			if (int.TryParse(value.ToString(), out var result))
				return result;

			return @default;

		}
		public static bool ConvertToBoolOrDefault(this IValue? value, bool @default = false)
		{
			if (value == null)
				return @default;

			if (value?.IsBoolean() ?? @default)
				return (bool?)value?.Data ?? @default;

			if (value.IsNumber())
				return value.AsDouble() > 0;

			if (value.IsString() && bool.TryParse(value.ToString(), out var boolFromString))
				return boolFromString;

			if (value.IsDateTime())
				return value.AsDateTime() != DateTime.MinValue;

			return @default;

		}

		public static DateTime? ConvertToDateTimeOrNull(this IValue? value)
		{
			if (value.IsString() && DateTime.TryParse(value.ToString(), out var dateTimeFromString))
				return dateTimeFromString;

			else if (value.TryGetAsDateTime(out var datetimeValue))
				return datetimeValue.Value;

			return null;
		}

		private static Exception IncorrectTypeException(IValue value, string targetType) => new InvalidOperationException($"Cannot retrieve the value '{value?.ToString()}' as {targetType} because its type is {value?.Data?.GetType().Name} ({value?.Type.ToString()})");
	}
}
