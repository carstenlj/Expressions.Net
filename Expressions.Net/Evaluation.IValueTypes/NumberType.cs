using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.IValueTypes
{
	internal sealed class NumberType : IValueType
	{
		public static readonly NumberType Invariant = new NumberType();
		public ValueRootType RootType { get; } = ValueRootType.Number;

		public IValue CreateValue(object? data, IValueConverter valueConverter) => new NumberValue(valueConverter.ConvertToNumber(data));
		public IValue CreateNullValue() => new NumberValue(null);
		public IValue CreateDefaultValue() => new NumberValue(0d);

		public override string ToString() => "number";

		Type IValueType.ConvertToType(IValueTypeConverter _) => typeof(double);

		public override bool Equals(object? obj) => obj != null && obj is NumberType;
		public override int GetHashCode() => ToString().GetHashCode();
	}
}
