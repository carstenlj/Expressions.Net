using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation
{
    public class LookupFunctionInfoResult
	{
		public LookupStatus Status { get; }

		public MethodInfo? MethodInfo { get; }
		public IValueType? ReturnType { get; }
		public bool Success => (Status == LookupStatus.ExistsSingle || Status == LookupStatus.ExistAmbigous) && MethodInfo != null && ReturnType != null;
		public int NullArgCount { get; }

		private LookupFunctionInfoResult(LookupStatus status, MethodInfo? methodInfo, IValueType? returnType, int nullArgCount)
		{
			Status = status;
			MethodInfo = methodInfo;
			ReturnType = returnType;
			NullArgCount = nullArgCount;
		}

		public static LookupFunctionInfoResult Exists(MethodInfo methodInfo, IEnumerable<FunctionSignature> validSignatures, int nullArgCount)
		{
			if (validSignatures.Count() == 1)
				return new LookupFunctionInfoResult(LookupStatus.ExistsSingle, methodInfo, validSignatures.Single().ReturnType, nullArgCount);

			var returnType = AmbiguousValueType.CreateUnionType(validSignatures.Select(x => x.ReturnType).ToArray());
			return new LookupFunctionInfoResult(LookupStatus.ExistAmbigous, methodInfo, returnType, nullArgCount);
		}

		public static LookupFunctionInfoResult FunctionDoesNotExist() => new LookupFunctionInfoResult(LookupStatus.FunctionDoesNotExist, null, null, 0);
		public static LookupFunctionInfoResult FunctionDoesNotSupportArgs() => new LookupFunctionInfoResult(LookupStatus.FunctionDoesNotSupportArgs, null, null, 0);

	}
}
