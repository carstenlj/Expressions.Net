using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.IValueTypes
{
	internal sealed class InvalidType : IValueType
	{
		public static readonly InvalidType Invariant = new InvalidType();
		public ValueRootType RootType { get; } = ValueRootType.Invalid;
		
		public IValue CreateValue(object? data, IValueConverter valueConverter) => new InvalidValue(data);
		public IValue CreateNullValue() => new InvalidValue(null);
		public IValue CreateDefaultValue() => new InvalidValue("Default");

		public override string ToString() => "invalid";

		Type IValueType.ConvertToType(IValueTypeConverter _) => typeof(void);

		public override bool Equals(object? obj) => obj != null && obj is InvalidType;
		public override int GetHashCode() => ToString().GetHashCode();
	}
}
