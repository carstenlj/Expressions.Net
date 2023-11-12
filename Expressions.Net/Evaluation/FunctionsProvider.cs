using Expressions.Net.Assemblies;
using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Evaluation.Functions.Wud;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Evaluation
{
	public class FunctionsProvider : IFunctionsProvider
	{

		public static readonly Dictionary<string, FunctionMethodSignatures> FunctionsCache;

		static FunctionsProvider()
		{
			var cache = new Dictionary<string, FunctionGroupDescriptor>();
			FunctionReflector.PupulateCacheWith(typeof(OperatorFunctions), cache);
			FunctionReflector.PupulateCacheWith(typeof(CommonFunctions), cache);
			FunctionReflector.PupulateCacheWith(typeof(NumericFunctions), cache);
			FunctionReflector.PupulateCacheWith(typeof(ObjectFunctions), cache);

			FunctionsCache = cache.ToDictionary(x => x.Key, x => new FunctionMethodSignatures(x.Value), StringComparer.OrdinalIgnoreCase);
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

		//private IEnumerable<FunctionSignature> GetValidOverloads(IEnumerable<FunctionSignature> functionOverloads, IValueType[] argTypes)
		//{
		//	foreach (var overload in functionOverloads)
		//	{
		//		var isValid = true;
		//		for (var i = 0; i < argTypes.Length; i++)
		//		{
		//			var overloadArg = overload.Args[i];
		//			isValid &= (argTypes[i].CouldBe(overloadArg));
		//		}

		//		if (isValid)
		//			yield return overload;
		//	}
		//}
	}
}
