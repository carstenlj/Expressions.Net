using Expressions.Net.Evaluation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Conversion
{
	public sealed class ValueConverter : IValueConverter
	{
		public static readonly IValueConverter Default = new ValueConverter(
			typeConverter: new ValueTypeConverter(),
			settings: ExpressionSettings.Default
		);

		private IExpressionSettings Settings { get; }
		public IValueTypeConverter TypeConverter { get; }

		public ValueConverter(IExpressionSettings settings, IValueTypeConverter typeConverter)
		{
			Settings = settings;
			TypeConverter = typeConverter;
		}

		public string? ConvertToString(object? value)
		{
			if (value is string strValue)
				return strValue;

			return Convert.ToString(value, Settings.FormatProvider);
		}

		public bool? ConvertToBoolean(object? value)
		{
			if (value is bool boolValue)
				return boolValue;

			return value == null
				? (bool?)null
				: Convert.ToBoolean(value, Settings.FormatProvider);
		}

		public double? ConvertToNumber(object? value)
		{
			if (value is double doubleValue)
				return doubleValue;

			return value == null
				? (double?)null
				: Convert.ToDouble(value, Settings.FormatProvider);
		}

		public DateTime? ConvertToDateTime(object? value)
		{
			if (value is DateTime dateTimeValue)
				return dateTimeValue;

			return value == null
				? (DateTime?)null
				: DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal);
		}

		public IList<IValue>? ConvertToArray(object? value, IValueType itemType)
		{
			var result = new List<IValue>();
			foreach(var item in AsEnumerable(value))
				result.Add(itemType.CreateValue(item,this));

			return result;
		}

		public IDictionary<string, IValue>? ConvertToDictionary(object? value, IDictionary<string, IValueType>? targetSchema)
		{
			if (value == null)
				return new Dictionary<string, IValue>();

			if (value is IDictionary<string, object> objDictionary)
				return ConvertDictionaryToValueDictionary(objDictionary, targetSchema);

			if (value is IDictionary<string, object?> nullableObjDictionary)
				return ConvertDictionaryToValueDictionary(nullableObjDictionary, targetSchema);

			if (value is IDictionary<string, string> stringDictionary)
				return ConvertDictionaryToValueDictionary(stringDictionary, targetSchema);

			return ConvertObjectToValueDictionary(value, targetSchema);
		}

		private IDictionary<string, IValue> ConvertObjectToValueDictionary(object value, IDictionary<string, IValueType>? schema = null)
		{
			var result = new Dictionary<string, IValue>();
			var props = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (var prop in props)
			{
				var targetTypeEntry = schema?.SingleOrDefault(x => x.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
				var targetType = (targetTypeEntry.HasValue && targetTypeEntry.Value.Value != null) ? targetTypeEntry.Value.Value : TypeConverter.ConvertToValueType(prop.PropertyType);
				
				var propValue = prop.GetValue(value);
				if (propValue == null)
				{
					result.Add(prop.Name, Settings.MissingValueHandling.GetValue(targetType, prop.Name));
					continue;
				}
				
				result.Add(prop.Name, targetType.CreateValue(propValue, this));
			}

			return result;
		}

		private IDictionary<string, IValue> ConvertDictionaryToValueDictionary<T>(IEnumerable<KeyValuePair<string, T>> dictionary, IDictionary<string, IValueType>? schema)
		{
			var result = new Dictionary<string, IValue>();
			 
			foreach (var prop in dictionary)
			{
				var targetType = schema?.SingleOrDefault(x => x.Key.Equals(prop.Key, StringComparison.OrdinalIgnoreCase)).Value;
				if (targetType == null && prop.Value != null)
					targetType = TypeConverter.ConvertToValueType(prop.Value.GetType());

				if (prop.Value == null)
				{
					result.Add(prop.Key, Settings.MissingValueHandling.GetValue(targetType, prop.Key));
					continue;
				}

				result.Add(prop.Key, targetType.CreateValue(prop.Value, this));
			}

			return result;
		}

		private static IEnumerable<object> AsEnumerable(object? value)
		{
			if (!(value?.GetType().IsEnumerable(out var itemType) ?? false))
			{
				yield return value;
			}
			else
			{
				foreach (object item in (IEnumerable)value)
					yield return item;
			}
		}

	}
}
