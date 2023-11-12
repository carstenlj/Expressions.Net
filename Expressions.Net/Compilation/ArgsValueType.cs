using Expressions.Net.Conversion;
using Expressions.Net.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Compilation
{
	internal class ArgsValueType : IValueType
	{
		public ValueRootType RootType => ValueRootType.Invalid;
		public List<IValueType> ArgumentTypes { get; set; }

		public ArgsValueType(IValueType[] types)
		{
			var types0 = GetTypes(types[0]);
			var types1 = GetTypes(types[1]);
			types0.AddRange(types1);

			ArgumentTypes = new List<IValueType>(types0);
		}

		public IValue CreateDefaultValue() => throw new NotSupportedException();
		public IValue CreateNullValue() => throw new NotSupportedException();
		public IValue CreateValue(object? data, IValueConverter valueConverter) => throw new NotSupportedException();
		Type IValueType.ConvertToType(IValueTypeConverter typeConverter) => throw new NotSupportedException();
		public static List<IValueType> GetTypes(IValueType type) => type is ArgsValueType args ? args.ArgumentTypes : new List<IValueType> { type };
	}
}
