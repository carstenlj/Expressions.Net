using Expressions.Net.Assemblies;
using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Evaluation.Functions.Wud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Evaluation
{
	public class FunctionsProvider : IFunctionsProvider
	{
		public static readonly Dictionary<string, FunctionMethodSignatures> FunctionsCache;
		public static readonly Dictionary<string, MethodInfo> OperatorCache;

		static FunctionsProvider()
		{
			var functionsCache = new Dictionary<string, FunctionGroupDescriptor>();
			var operatorCache= new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

			FunctionReflector.PopulateOperatorCacheWith(typeof(OperatorFunctions), operatorCache);
			FunctionReflector.PopulateFunctionCacheWith(typeof(OperatorFunctions), functionsCache);
			FunctionReflector.PopulateFunctionCacheWith(typeof(CommonFunctions), functionsCache);
			FunctionReflector.PopulateFunctionCacheWith(typeof(NumericFunctions), functionsCache);
			FunctionReflector.PopulateFunctionCacheWith(typeof(ObjectFunctions), functionsCache);


			FunctionsCache = functionsCache.ToDictionary(x => x.Key, x => new FunctionMethodSignatures(x.Value), StringComparer.OrdinalIgnoreCase);
			OperatorCache = operatorCache;
		}

		public LookupFunctionInfoResult LookupFunctionInfo(string fuctionName, params IValueType[] argTypes)
		{
			if (!FunctionsCache.TryGetValue(fuctionName, out var functionInfo))
				return LookupFunctionInfoResult.FunctionDoesNotExist();

			var validSignatures = functionInfo.Signatures.Where(x => x.SupportsArgTypes(argTypes)).ToArray();
			if (!validSignatures.Any())
				return LookupFunctionInfoResult.FunctionDoesNotSupportArgs();

			var nullArgCount = validSignatures.Max(x => x.GetNullArgCount(argTypes.Length));
			return LookupFunctionInfoResult.Exists(functionInfo.MethodInfo, validSignatures, nullArgCount);
		}

		public MethodInfo? LookupOperatorMethodInfo(string @operator)
		{
			if (OperatorCache.TryGetValue(@operator, out var methodInfo))
				return methodInfo;

			return null;
		}
	}
}
