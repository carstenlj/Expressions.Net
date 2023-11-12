using Expressions.Net.Evaluation.IValueTypes;
using System;

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
	}
}
