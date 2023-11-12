using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressions.Net.Conversion
{
	internal sealed class ValueTypeConverter : IValueTypeConverter
	{
		private readonly ModuleBuilder ModuleBuilder = AssemblyBuilder
			.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run)
			.DefineDynamicModule("Expressions.Net.Conversion");

		public static IValueType ConvertToValueType(string type)
		{
			type = type.TrimEnd('?');

			if (type.Equals("string", StringComparison.OrdinalIgnoreCase))
				return StringType.Invariant;

			if (type.Equals("number", StringComparison.OrdinalIgnoreCase))
				return NumberType.Invariant;

			if (type.Equals("boolean", StringComparison.OrdinalIgnoreCase))
				return BooleanType.Invariant;

			if (type.Equals("datetime", StringComparison.OrdinalIgnoreCase))
				return DateTimeType.Invariant;

			if (type.Equals("array<T>", StringComparison.OrdinalIgnoreCase))
				return ArrayType.Any;

			if (type.Equals("object", StringComparison.OrdinalIgnoreCase))
				return ObjectType.Any;

			if (type.Equals("any", StringComparison.OrdinalIgnoreCase))
				return AmbiguousValueType.Any;

			if (type.Equals("T", StringComparison.OrdinalIgnoreCase))
				return AmbiguousValueType.T;

			var multiTypes = type.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(ConvertToValueType);
			if (multiTypes.Any())
				return AmbiguousValueType.CreateUnionType(multiTypes.ToArray());

			throw new NotSupportedException($"There's no conversion between the string '{type}' and an IValueType");
		}

		public  IValueType ConvertToValueType(Type type)
		{
			if (type == null)
				return InvalidType.Invariant;

			if (type == typeof(string))
				return StringType.Invariant;

			if (type == typeof(bool) || type == typeof(bool?))
				return BooleanType.Invariant;

			if (type.IsNumericType())
				return NumberType.Invariant;

			if (type == typeof(DateTime) || type == typeof(DateTime?))
				return DateTimeType.Invariant;

			if (type.IsEnumerable(out var itemType))
				return new ArrayType(ConvertToValueType(itemType));

			return new ObjectType(ConvertToSchema(type));
		}

		public Type CreateTypeFromSchema(IDictionary<string, IValueType> schema)
		{
			var typeName = "ExpressionObjectType_" + Guid.NewGuid();
			var typeAttribs = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;
			var typeBuilder = ModuleBuilder.DefineType(typeName, typeAttribs, null);

			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

			foreach (var field in schema)
				DefineTypeProperty(typeBuilder, field.Key, field.Value.ConvertToType(this));

			return typeBuilder.CreateType();
		}

		private IDictionary<string, IValueType> ConvertToSchema(Type type)
		{
			var result = new Dictionary<string, IValueType>();
			var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach(var prop in props)
				result.Add(prop.Name, ConvertToValueType(prop.PropertyType));

			return result;
		}

		private void DefineTypeProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
		{
			// Define property and underlying field
			var fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);
			var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

			// Emit property 'get_' method body
			var getMethodAttribs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
			var getPropMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}", getMethodAttribs, propertyType, Type.EmptyTypes);
			var getMethod_IL = getPropMethodBuilder.GetILGenerator();

			getMethod_IL.Emit(OpCodes.Ldarg_0);
			getMethod_IL.Emit(OpCodes.Ldfld, fieldBuilder);
			getMethod_IL.Emit(OpCodes.Ret);

			// Emit property 'set_' method body
			var setMethodAttribs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
			var setPropMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}", setMethodAttribs, null, new[] { propertyType });
			var setMethod_IL = setPropMethodBuilder.GetILGenerator();
			var labelModify = setMethod_IL.DefineLabel();
			var labelRet = setMethod_IL.DefineLabel();

			setMethod_IL.MarkLabel(labelModify);
			setMethod_IL.Emit(OpCodes.Ldarg_0);
			setMethod_IL.Emit(OpCodes.Ldarg_1);
			setMethod_IL.Emit(OpCodes.Stfld, fieldBuilder);
			setMethod_IL.Emit(OpCodes.Nop);
			setMethod_IL.MarkLabel(labelRet);
			setMethod_IL.Emit(OpCodes.Ret);

			// Assign property accessor method bodies
			propertyBuilder.SetGetMethod(getPropMethodBuilder);
			propertyBuilder.SetSetMethod(setPropMethodBuilder);
		}
	}
}
