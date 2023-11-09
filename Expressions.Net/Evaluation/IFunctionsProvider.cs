using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Expressions.Net.Assemblies;
using Expressions.Net.Conversion;
using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation
{
	public partial interface IFunctionsProvider
	{
		LookupFunctionInfoResult LookupFunctionInfo(string fuctionName, params IValueType[] argTypes);

		public class LookupFunctionInfoResult
		{
			public LookupStatus Status { get; }

			public MethodInfo? MethodInfo { get; }
			public IValueType? ReturnType { get; }
			public bool Success => (Status == LookupStatus.ExistsSingle || Status == LookupStatus.ExistAmbigous) && MethodInfo != null && ReturnType != null;

			private LookupFunctionInfoResult(LookupStatus status, MethodInfo? methodInfo, IValueType? returnType)
			{
				Status = status;
				MethodInfo = methodInfo;
				ReturnType = returnType;
			}

			public static LookupFunctionInfoResult Exists(MethodInfo methodInfo, IEnumerable<FunctionSignature> functionOverloads)
			{
				if (functionOverloads.Count() == 1)
					return new LookupFunctionInfoResult(LookupStatus.ExistsSingle, methodInfo, functionOverloads.Single().ReturnType);

				var returnType = AmbiguousValueType.CreateUnionType(functionOverloads.Select(x => x.ReturnType).ToArray());
				return new LookupFunctionInfoResult(LookupStatus.ExistAmbigous, methodInfo, returnType);
			}

			public static LookupFunctionInfoResult FunctionDoesNotExist() => new LookupFunctionInfoResult(LookupStatus.FunctionDoesNotExist, null, null);
			public static LookupFunctionInfoResult FunctionDoesNotSupportArgCount() => new LookupFunctionInfoResult(LookupStatus.FunctionDoesNotSupportArgCount, null, null);
			public static LookupFunctionInfoResult FunctionDoesNotSupportArgTypes() => new LookupFunctionInfoResult(LookupStatus.FunctionDoesNotSupportArgTypes, null, null);

		}
	}
}
