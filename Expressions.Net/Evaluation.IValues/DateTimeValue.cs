using Expressions.Net.Evaluation.IValueTypes;
using System;
using System.Globalization;

namespace Expressions.Net.Evaluation.IValues
{
	internal readonly struct DateTimeValue : IValue
	{
		public static DateTimeValue UtcNow => new DateTimeValue(DateTime.UtcNow);

		public IValueType Type => DateTimeType.Invariant;
		public object? Data { get; }

		public DateTimeValue(DateTime? val)
		{
			Data = val;
		}

		public override string ToString()
		{
			return Data == null ? string.Empty : ((DateTime)Data).ToString(CultureInfo.InvariantCulture) ?? string.Empty;
		}
	}
}
