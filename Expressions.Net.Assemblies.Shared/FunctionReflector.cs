using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Assemblies
{
	public static class FunctionReflector
	{
		public static void PupulateCacheWith(Type typeContainingFunctions, Dictionary<string, FunctionGroupDescriptor> cache)
		{
			var methods = typeContainingFunctions.GetMethods();
			foreach (var methodInfo in methods)
			{
				var functionsAttr = methodInfo.GetCustomAttributes<ExpressionFunctionAttribute>().ToList();

				foreach (var funcAttr in functionsAttr)
				{
					var expressionFuncs = FunctionDescriptor.Parse(funcAttr.Signature);
					foreach (var expressionFunc in expressionFuncs)
					{
						if (!cache.TryGetValue(expressionFunc.Alias, out var functionInfo))
						{
							functionInfo = new FunctionGroupDescriptor { Name = expressionFunc.Alias, MethodInfo = methodInfo, IsGlobal = expressionFunc.IsGlobal };
							cache.Add(expressionFunc.Alias, functionInfo);
						}

						if (functionInfo.MethodInfo != methodInfo)
							throw new InvalidOperationException($"Cannot declare diffrent methods with the same function alias '{expressionFunc.Alias}'");

						var argCount = expressionFunc.Args.Count();
						if (!functionInfo.Overloads.TryGetValue(argCount, out var overloads))
						{
							overloads = new List<FunctionSignatureDescriptor>();
							functionInfo.Overloads.Add(argCount, overloads);
						}

						overloads.Add(new FunctionSignatureDescriptor {
							Name = expressionFunc.Alias,
							IsGlobal = expressionFunc.IsGlobal,
							Args = expressionFunc.Args,
							ReturnType = expressionFunc.ReturnType
						});
					}
				}
			}
		}


	}
}
