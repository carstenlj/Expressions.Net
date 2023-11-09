using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValueTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Expressions.Net.Evaluation
{
    public interface IValueType
	{
		ValueRootType RootType { get; }
		IValue CreateNullValue();
		IValue CreateDefaultValue();
		IValue CreateValue(object? data, IValueConverter valueConverter);
		internal Type ConvertToType(IValueTypeConverter typeConverter);

		public static IValueType GetStringType() => StringType.Invariant;
		public static IValueType GetNumberType() => NumberType.Invariant;
		public static IValueType GetBooleanType() => BooleanType.Invariant;
		public static IValueType GetAnyArrayType() => ArrayType.Any;
		public static IValueType GetAnyObjectType() => ObjectType.Any;
		public static IValueType GetArrayType(IValueType itemType) => new ArrayType(itemType);
		public static IValueType GetObjectType(IDictionary<string, IValueType> schema) => new ObjectType(schema);
		public static IValueType GetAmbigousType() => AmbiguousValueType.Any;
		public static IValueType GetAmbigousType(params IValueType[] types) => types.Length == 1 ? types.Single() : AmbiguousValueType.CreateUnionType(types);
	}

	public static class IValueTypeExtensions
	{
		public static IValueType[]? GetPossibleTypes(this IValueType type)
		{
			return type is AmbiguousValueType ambiguousValueType ? ambiguousValueType.PossibleTypes : new IValueType[] { type };
		}

		public static bool TryGetArrayItemType(this IValueType type, [NotNullWhen(true)] out IValueType? arrayItemType)
		{
			return (arrayItemType = type is ArrayType arrayType ? arrayType.ItemType : null) != null;
		}

		public static bool TryGetObjectSchema(this IValueType type, [NotNullWhen(true)] out IDictionary<string, IValueType>? objectSchema)
		{
			return (objectSchema = type is ObjectType objType ? objType.ObjectSchema : null) != null;
		}

		internal static bool TryGetAmbigousItemType(this IValueType type, [NotNullWhen(true)] out AmbiguousValueType? itemType)
		{
			return (itemType = type is ArrayType arrType && arrType.ItemType is AmbiguousValueType ambItemType ? ambItemType : null) != null;
		}

		public static bool IsAnyObject(this IValueType type)
		{
			return type is ObjectType objectType && objectType.SchemaIsAny;
		}

		public static bool CouldBe(this IValueType type, IValueType otherType)
		{
			if (type is AmbiguousValueType ambiguousType1)
				return ambiguousType1.CouldBe(otherType);

			if (otherType is AmbiguousValueType ambiguousType2)
				return ambiguousType2.CouldBe(type);

			if (type.TryGetArrayItemType(out var arrItemType0) && otherType.TryGetArrayItemType(out var arrItemType1))
				return arrItemType0.CouldBe(arrItemType1);

			if (type.RootType == ValueRootType.Object && otherType.RootType == ValueRootType.Object)
				return true;

			return type == otherType;
		}
	}
}