using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Net.Evaluation.IValueTypes
{
	internal sealed class ObjectType : IValueType
	{
		internal static readonly ObjectType Any = new ObjectType(new Dictionary<string, IValueType>(), true);

		public ValueRootType RootType { get; } = ValueRootType.Object;
		public IDictionary<string, IValueType> ObjectSchema { get; }
		public bool SchemaIsAny { get; }

		// TODO:	Introduce a IObjectSchema type with a .Loopup() calss
		//			This should be used by the IVariablesSchema.
		//			It could also be utilized to create an AmbigousObjectSchema with the Lookup method always returning a  AmbigousType

		public ObjectType(IDictionary<string, IValueType> schema) : this(schema, false) { }
		private ObjectType(IDictionary<string, IValueType> schema, bool schemaIsAny)
		{
			ObjectSchema = schema;
			SchemaIsAny = schemaIsAny;
		}

		public IValue CreateValue(object? data, IValueConverter valueConverter) => new ObjectValue(valueConverter.ConvertToDictionary(data, ObjectSchema), ObjectSchema);
		public IValue CreateNullValue() => new ObjectValue(null, ObjectSchema);
		public IValue CreateDefaultValue() => new ObjectValue(new Dictionary<string, IValue>(), ObjectSchema);

		public override bool Equals(object? obj) => obj != null && obj is ObjectType type && type.ToString(true).GetHashCode() == GetHashCode();
		public override int GetHashCode() => ToString(true).GetHashCode();

		public override string ToString() => this.ToString(false);
		public string ToString(bool withSchema)
		{
			if (!withSchema || SchemaIsAny)
				return "object";

			var i = 0;
			var schemaStringBuilder = new StringBuilder("object<");
			foreach (var item in ObjectSchema)
			{
				schemaStringBuilder.Append($"{item.Key}:{item.Value.ToString()}");
				if (i++ < ObjectSchema.Count - 1)
					schemaStringBuilder.Append($", ");
			}

			return schemaStringBuilder.Append(">").ToString();
		}

		Type IValueType.ConvertToType(IValueTypeConverter typeConverter) => typeConverter.CreateTypeFromSchema(ObjectSchema);
	}
}
