using Expressions.Net.Evaluation;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Expressions.Net.Reflection
{
    public static class FunctionReflector
	{
		public static Dictionary<string, MethodInfo> GetOperatorMethodInfo(Type typeContainingFunctions)
		{
			var result = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
			foreach (var methodInfo in typeContainingFunctions.GetMethods())
			{
				var attr = methodInfo.GetCustomAttribute<OperatorFunctionAttribute>();
				if (attr != null)
					result.Add(attr.Operator, methodInfo);
			}

			return result;
		}

		public static void PopulateOperatorCacheWith(Type typeContainingFunctions, Dictionary<string, MethodInfo> cache)
		{
			foreach (var methodInfo in typeContainingFunctions.GetMethods())
			{
				var attr = methodInfo.GetCustomAttribute<OperatorFunctionAttribute>();
				if (attr != null)
					cache.Add(attr.Operator, methodInfo);
			}
		}

		public static void PopulateDictionaryWithFunctionDescriptors(Type typeContainingFunctions, Dictionary<string, FunctionSignatureGroup> cache)
		{
			foreach (var methodInfo in typeContainingFunctions.GetMethods())
			{
				foreach (var functionDescriptor in GetFunctionDescriptors(methodInfo))
				{
					PopulateFunctionCacheWith(functionDescriptor, methodInfo, cache);
				}
			}
		}

		internal static void PopulateFunctionCacheWith(ExpressionFunctionSignature functionDescriptor, MethodInfo methodInfo, Dictionary<string, FunctionSignatureGroup> cache)
		{
			if (!cache.TryGetValue(functionDescriptor.Alias, out var functionGroup))
			{
				functionGroup = new FunctionSignatureGroup(methodInfo);
				cache.Add(functionDescriptor.Alias, functionGroup);
			}

			if (functionGroup.MethodInfo != methodInfo)
				throw new InvalidOperationException($"Cannot declare diffrent methods with the same function alias '{functionDescriptor.Alias}'");

			functionGroup.Signatures.Add(FunctionSignature.Create(functionDescriptor));
		}

		internal static IEnumerable<ExpressionFunctionSignature> GetFunctionDescriptors(MethodInfo methodInfo)
		{
			foreach (var funcAttr in methodInfo.GetCustomAttributes<ExpressionFunctionAttribute>())
				yield return ExpressionFunctionSignature.Parse(funcAttr.Signature);
		}
	}
}
