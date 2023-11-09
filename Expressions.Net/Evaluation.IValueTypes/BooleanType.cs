using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.IValueTypes
{
	internal sealed class BooleanType : IValueType
	{
		public static readonly BooleanType Invariant = new BooleanType();
		public ValueRootType RootType { get; } = ValueRootType.Boolean;

		public IValue CreateValue(object? data, IValueConverter valueConverter) => new BooleanValue(valueConverter.ConvertToBoolean(data));
		public IValue CreateNullValue() => new BooleanValue(null);
		public IValue CreateDefaultValue() => new BooleanValue(false);

		public override string ToString() => "boolean";

		Type IValueType.ConvertToType(IValueTypeConverter _) => typeof(bool);

		public override bool Equals(object? obj) => obj != null && obj is BooleanType;
		public override int GetHashCode() => ToString().GetHashCode();
	}
}
