using Expressions.Net.Assemblies;
using Expressions.Net.Evaluation.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Expressions.Net.Evaluation.IFunctionsProvider;

namespace Expressions.Net.Evaluation
{
	public class FunctionsProvider : IFunctionsProvider
	{

		public static readonly Dictionary<string, FunctionGroup> FunctionsCache;

		static FunctionsProvider()
		{
			var cache = new Dictionary<string, FunctionGroupDescriptor>();
			FunctionReflector.PupulateCacheWith(typeof(OperatorFunctions), cache);
			FunctionReflector.PupulateCacheWith(typeof(CommonFunctions), cache);
			FunctionReflector.PupulateCacheWith(typeof(NumericFunctions), cache);
			FunctionReflector.PupulateCacheWith(typeof(ObjectFunctions), cache);

			FunctionsCache = cache.ToDictionary(x => x.Key, x => new FunctionGroup(x.Value));
		}

		//static void PupulateCacheWith(Type typeContainingFunctions)
		//{
		//	foreach (var methodInfo in typeContainingFunctions.GetMethods())
		//	{
		//		var functionsAttr = methodInfo.GetCustomAttributes<ExpressionFunctionAttribute>();

		//		foreach (var funcAttr in functionsAttr)
		//		{
		//			var expressionFuncs = ExpressionFunction.Parse(funcAttr.Signature);
		//			foreach (var expressionFunc in expressionFuncs)
		//			{
		//				if (!FunctionsCache.TryGetValue(expressionFunc.Alias, out var functionInfo))
		//				{
		//					functionInfo = new FunctionGroupTypedInfo(methodInfo, expressionFunc.IsGlobal);
		//					FunctionsCache.Add(expressionFunc.Alias, functionInfo);
		//				}

		//				if (functionInfo.MethodInfo != methodInfo)
		//					throw new InvalidOperationException($"Cannot declare diffrent methods with the same function alias '{expressionFunc.Alias}'");

		//				var argCount = expressionFunc.Args.Count();
		//				if (!functionInfo.Overloads.TryGetValue(argCount, out var overloads))
		//				{
		//					overloads = new List<FunctionOverloadTypedInfo>();
		//					functionInfo.Overloads.Add(argCount, overloads);
		//				}

		//				overloads.Add(new FunctionOverloadTypedInfo
		//				{
		//					Args = expressionFunc.Args,
		//					ReturnType = expressionFunc.ReturnType
		//				});
		//			}

		//		}
		//	}
		//}

		public LookupFunctionInfoResult LookupFunctionInfo(string fuctionName, params IValueType[] argTypes)
		{
			if (!FunctionsCache.TryGetValue(fuctionName, out var functionInfo))
				return LookupFunctionInfoResult.FunctionDoesNotExist();

			if (!functionInfo.Overloads.TryGetValue(argTypes.Length, out var functionOverloads))
				return LookupFunctionInfoResult.FunctionDoesNotSupportArgCount();

			var validOverloads = GetValidOverloads(functionOverloads, argTypes).ToList();

			if (!validOverloads.Any())
				return LookupFunctionInfoResult.FunctionDoesNotSupportArgTypes();

			return LookupFunctionInfoResult.Exists(functionInfo.MethodInfo, validOverloads);
		}

		private IEnumerable<FunctionSignature> GetValidOverloads(IEnumerable<FunctionSignature> functionOverloads, IValueType[] argTypes)
		{
			foreach (var overload in functionOverloads)
			{
				var isValid = true;
				for (var i = 0; i < argTypes.Length; i++)
				{
					var overloadArg = overload.Args[i];
					isValid &= (argTypes[i].CouldBe(overloadArg));
				}

				if (isValid)
					yield return overload;
			}

		}
	}


}
