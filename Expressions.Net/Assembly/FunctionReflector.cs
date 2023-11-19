using Expressions.Net.Assembly;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Expressions.Net.Assemblies
{
    public static class FunctionReflector
	{
		public static void PopulateOperatorCacheWith(Type typeContainingFunctions, Dictionary<string, MethodInfo> cache)
		{
			foreach (var methodInfo in typeContainingFunctions.GetMethods())
			{
				var attr = methodInfo.GetCustomAttribute<OperatorFunctionAttribute>();
				if (attr != null)
					cache.Add(attr.Operator, methodInfo);
			}
		}

		public static void PopulateFunctionCacheWith(Type typeContainingFunctions, Dictionary<string, FunctionGroupDescriptor> cache)
		{
			foreach (var methodInfo in typeContainingFunctions.GetMethods())
			{
				foreach (var functionDescriptor in GetFunctionDescriptors(methodInfo))
				{
					PopulateFunctionCacheWith(functionDescriptor, methodInfo, cache);
				}
			}
		}

		internal static void PopulateFunctionCacheWith(FunctionDescriptor functionDescriptor, MethodInfo methodInfo, Dictionary<string, FunctionGroupDescriptor> cache)
		{
			if (!cache.TryGetValue(functionDescriptor.Alias, out var functionGroup))
			{
				functionGroup = new FunctionGroupDescriptor(functionDescriptor.Alias, methodInfo);
				cache.Add(functionDescriptor.Alias, functionGroup);
			}

			if (functionGroup.MethodInfo != methodInfo)
				throw new InvalidOperationException($"Cannot declare diffrent methods with the same function alias '{functionDescriptor.Alias}'");

			functionGroup.Signatures.Add(new FunctionSignatureDescriptor (
				name: functionDescriptor.Alias,
				isGlobal: functionDescriptor.IsGlobal,
				args: functionDescriptor.Args,
				requiredArgsCount: functionDescriptor.RequiredArgsCount,
				returnType: functionDescriptor.ReturnType
			));
		}

		internal static IEnumerable<FunctionDescriptor> GetFunctionDescriptors(MethodInfo methodInfo)
		{
			foreach (var funcAttr in methodInfo.GetCustomAttributes<ExpressionFunctionAttribute>())
				yield return FunctionDescriptor.Parse(funcAttr.Signature);
		}
	}
}
