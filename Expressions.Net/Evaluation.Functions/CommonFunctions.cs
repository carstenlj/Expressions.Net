using Expressions.Net.Assemblies;
using Expressions.Net.Evaluation.IValues;
using System.Linq;

namespace Expressions.Net.Evaluation.Functions
{
	public static class CommonFunctions
	{
		[ExpressionFunction("string|number|boolean.Length():number")]
		[ExpressionFunction("array<T>.Length():number")]
		[ExpressionFunction("object.Length():number")]
		public static IValue Length(this IValue arg0)
		{
			if (arg0.IsArray())
				return new NumberValue(arg0.AsArray()?.Count ?? 0);

			if (arg0.IsObject())
				return new NumberValue(arg0.AsObject()?.Keys.Count ?? 0);

			return new NumberValue(arg0.Data?.ToString()?.Length ?? 0);
		}

		[ExpressionFunction("any.ToString():string")]
		public static IValue ToString(this IValue arg0)
		{
			return new StringValue(arg0.ToString());
		}

		public static IValue Union(this IValue arg0, IValue arg1)
		{
			if (arg0.Type.TryGetArrayItemType(out var arrType0))
			{
				// Union two arrays
				if (arg1.Type.TryGetArrayItemType(out var arrType1))
				{
					if (arrType0 != arrType1 || (arrType0.RootType == ValueRootType.Object && arg1.Type.RootType == ValueRootType.Object))
						return InvalidValue.CannotUnionTwoArraysWithDifrentItemType(arg0, arg1);

					return new ArrayValue(arg0.AsArray().Union(arg1.AsArray()).ToList(), arrType0);
				}

				// Append arg1 (scalar) to arg0 (Array)
				if (arrType0 == arg1.Type || (arrType0.RootType == ValueRootType.Object && arg1.Type.RootType == ValueRootType.Object))
				{
					var arr0 = arg0.AsArray();
					arr0.Add(arg1);
					return new ArrayValue(arr0.ToList(), arrType0); 
				}
			}

			// Union two objects
			if (arg0.TryGetAsObject(out var obj0) && arg1.TryGetAsObject(out var obj1))
			{
				return new ObjectValue(obj0.Union(obj1).ToDictionary(x => x.Key, x => x.Value));
			}

			return InvalidValue.FunctionNotSupportedForArgs(nameof(Union), arg0, arg1);
		}
	}
}
