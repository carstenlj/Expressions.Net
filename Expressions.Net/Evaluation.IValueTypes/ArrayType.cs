using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Collections.Generic;

namespace Expressions.Net.Evaluation.IValueTypes
{
    internal sealed class ArrayType : IValueType
	{
		internal static readonly ArrayType Any = new ArrayType(AmbiguousValueType.T);

		public ValueRootType RootType { get; } = ValueRootType.Array;
		public IValueType ItemType { get; }

		public ArrayType(IValueType itemType)
		{
			ItemType = itemType;
		}

		public IValue CreateValue(object? data, IValueConverter valueConverter) => new ArrayValue(valueConverter.ConvertToArray(data, ItemType), ItemType);
		public IValue CreateNullValue() => new ArrayValue(null, ItemType);
		public IValue CreateDefaultValue() => new ArrayValue(new List<IValue>(), ItemType);

		public override string ToString() => this.ToString(true);
		public string ToString(bool withType)
		{
			return withType ? $"array<{ItemType}>" : "array";
		}

		Type IValueType.ConvertToType(IValueTypeConverter typeConverter) => typeof(List<>).MakeGenericType(ItemType.ConvertToType(typeConverter));

		public override bool Equals(object? obj) => obj != null && obj is ArrayType type && type.ToString(true).GetHashCode() == GetHashCode();
		public override int GetHashCode() => ToString(true).GetHashCode();
	}
}
