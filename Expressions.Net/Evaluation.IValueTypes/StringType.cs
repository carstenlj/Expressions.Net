using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.IValueTypes
{
	internal sealed class StringType : IValueType
	{
		public static readonly StringType Invariant = new StringType();
		public ValueRootType RootType { get; } = ValueRootType.String;

		public IValue CreateValue(object? data, IValueConverter valueConverter) => new StringValue(valueConverter.ConvertToString(data));
		public IValue CreateNullValue() => new StringValue(null);
		public IValue CreateDefaultValue() => new StringValue(string.Empty);

		public override string ToString() => "string";
		
		Type IValueType.ConvertToType(IValueTypeConverter _) => typeof(string);

		public override bool Equals(object? obj) => obj != null && obj is StringType;
		public override int GetHashCode() => ToString().GetHashCode();
	}
}
