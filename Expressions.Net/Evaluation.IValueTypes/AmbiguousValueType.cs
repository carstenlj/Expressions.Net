using Expressions.Net.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Expressions.Net.Tests")]

namespace Expressions.Net.Evaluation.IValueTypes
{

	internal sealed class AmbiguousValueType : IValueType
	{
		public static readonly AmbiguousValueType Any = new AmbiguousValueType(false, null);
		public static readonly AmbiguousValueType T = new AmbiguousValueType(true, null);

		public ValueRootType RootType => ValueRootType.Invalid;
		public IValueType[]? PossibleTypes { get; private set; }
		public bool IsAny => PossibleTypes == null;
		private bool IsItemType { get; }

		public AmbiguousValueType(params IValueType[] possibleTypes) : this(false, possibleTypes) { }
		private AmbiguousValueType(bool isItemType, params IValueType[]? possibleTypes)
		{
			IsItemType = isItemType;
			PossibleTypes = possibleTypes;
		}

		public static IValueType CreateUnionType(params IValueType[] types)
		{
			var unionTypes = new List<IValueType>();
			foreach (var type in types)
			{
				if (type is AmbiguousValueType ambiguous)
				{
					if (ambiguous.IsAny)
						return Any;

					unionTypes.AddRange(ambiguous.PossibleTypes);
				}
				else
				{
					unionTypes.Add(type);
				}
			}

			var distincUnionTypes = unionTypes.Distinct().ToArray();
			if (distincUnionTypes.Count() == 1)
				return unionTypes.First();

			return new AmbiguousValueType(distincUnionTypes);
		}

		// NOTE: Good candidate for unit test
		public bool CouldBe(IValueType type)
		{
			if (IsAny)
				return true;

			var ambigousType = type as AmbiguousValueType;

			if (!(ambigousType is null) && ambigousType.IsAny)
				return true;

			if (RootType == ValueRootType.Array || type.RootType == ValueRootType.Array)
				return IsItemType && IsAny || (!(ambigousType is null) && ambigousType.IsItemType && ambigousType.IsAny);

			if (!(ambigousType is null))
				return ambigousType.PossibleTypes.Intersect(PossibleTypes).Any();

			return PossibleTypes.Contains(type);
		}

		public override string ToString()
		{
			if (IsAny)
				return IsItemType ? "T" : "any";

			return string.Join('|', PossibleTypes.Select(x => x.ToString()));
		}

		#region Unsupported operations
		public IValue CreateDefaultValue() => throw new InvalidOperationException($"Cannot create an {nameof(IValue)} of an {nameof(AmbiguousValueType)}");
		public IValue CreateNullValue() => throw new InvalidOperationException($"Cannot create an {nameof(IValue)} of an {nameof(AmbiguousValueType)}");
		public IValue CreateValue(object? data, IValueConverter valueConverter) => throw new InvalidOperationException($"Cannot create an {nameof(IValue)} of an {nameof(AmbiguousValueType)}");
		Type IValueType.ConvertToType(IValueTypeConverter typeConverter) => throw new InvalidOperationException($"Cannot create a Type from an {nameof(AmbiguousValueType)}");
		#endregion
	}
}
