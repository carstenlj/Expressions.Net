using Expressions.Net.Evaluation.IValues;
using System;

namespace Expressions.Net.Evaluation.Functions
{
	public static class ObjectFunctions
	{
		public static IValue Property(this IValue obj, string key)
		{
			return obj is IVariables vars 
				? vars.Lookup(key) 
				: InvalidValue.CannotResolveProperty(key);
		}
	}
}
