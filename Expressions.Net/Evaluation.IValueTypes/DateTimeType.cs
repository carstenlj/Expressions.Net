using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.IValueTypes
{
	internal sealed class DateTimeType : IValueType
	{
		public static readonly DateTimeType Invariant = new DateTimeType();
		public ValueRootType RootType { get; } = ValueRootType.DateTime;

		public IValue CreateValue(object? data, IValueConverter valueConverter) => new DateTimeValue(valueConverter.ConvertToDateTime(data));
		public IValue CreateDefaultValue() => new DateTimeValue(DateTime.MinValue);
		public IValue CreateNullValue() => new DateTimeValue(null);
		
		Type IValueType.ConvertToType(IValueTypeConverter _) => typeof(DateTime);

		public override string ToString() => "datetime";
	}
}
