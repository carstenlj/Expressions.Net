using Expressions.Net.Evaluation.IValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Evaluation.IValues
{
	internal sealed class ObjectValue : ValueBase, IVariables
	{
		public string[] Keys => Type.TryGetObjectSchema(out var schema) ? schema.Keys.ToArray() : Array.Empty<string>();
		public IDictionary<string, IValueType> Schema { get; }

		private IDictionary<string, IValue>? TypedData { get; }

		internal ObjectValue(IDictionary<string, IValue>? val) : this(val, new Dictionary<string, IValueType>()) { }
		internal ObjectValue(IDictionary<string, IValue>? val, IDictionary<string, IValueType> schema)
			: base(new ObjectType(schema), val) 
		{
			TypedData = val;
			Schema = schema;
		}

		public IValue Lookup(string variableName)
		{
			if (TypedData?.TryGetValue(variableName, out var result) ?? false)
				return result;

			return InvalidValue.CannotResolveProperty(variableName);
		}
	}
}
