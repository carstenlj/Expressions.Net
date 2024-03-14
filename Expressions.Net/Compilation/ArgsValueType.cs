using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using System;
using System.Collections.Generic;

namespace Expressions.Net.Compilation
{
	internal sealed class ArgsValueType : IValueType
	{
		public ValueRootType RootType { get; } = ValueRootType.Invalid;
		public IEnumerable<IValueType> ArgumentTypes { get; }

		public ArgsValueType(IValueType[] types)
		{
			ArgumentTypes = GetTypes(types);
		}

		public static IEnumerable<IValueType> GetTypes(IValueType[] types)
		{
			foreach (var type in types)
			{
				if (type is ArgsValueType args)
				{
					foreach (var subType in args.ArgumentTypes)
						yield return subType;
				}
				else
				{
					yield return type;
				}
			}
		}

		public IValue CreateDefaultValue() => throw new NotSupportedException();
		public IValue CreateNullValue() => throw new NotSupportedException();
		public IValue CreateValue(object? data, IValueConverter valueConverter) => throw new NotSupportedException();
		Type IValueType.ConvertToType(IValueTypeConverter typeConverter) => throw new NotSupportedException();

	}
}
