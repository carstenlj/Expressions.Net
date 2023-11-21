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
		public static readonly FunctionsProvider Default = new FunctionsProvider();
		public readonly Dictionary<string, FunctionMethodSignatures> FunctionsCache;

		protected virtual bool UseCoreFunctions => true;

		public FunctionsProvider()
		{
			var functionsCache = new Dictionary<string, FunctionGroupDescriptor>();

			if(UseCoreFunctions)
			{
				FunctionReflector.PopulateDictionaryWithFunctionDescriptors(typeof(OperatorFunctions), functionsCache);
				FunctionReflector.PopulateDictionaryWithFunctionDescriptors(typeof(CommonFunctions), functionsCache);
				FunctionReflector.PopulateDictionaryWithFunctionDescriptors(typeof(NumericFunctions), functionsCache);
				FunctionReflector.PopulateDictionaryWithFunctionDescriptors(typeof(ObjectFunctions), functionsCache);
			}
			
			var customFunctions = CustomFunctions();
			foreach(var function in customFunctions)
				functionsCache.Add(function.Key, function.Value);

			FunctionsCache = functionsCache.ToDictionary(x => x.Key, x => new FunctionMethodSignatures(x.Value), StringComparer.OrdinalIgnoreCase);
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

		protected virtual IDictionary<string,FunctionGroupDescriptor> CustomFunctions()
		{
			return new Dictionary<string, FunctionGroupDescriptor>();
		}

	}
}
