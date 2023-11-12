using Expressions.Net.Assemblies;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Evaluation
{

	public class FunctionMethodSignatures
	{
		public MethodInfo MethodInfo { get; }
		public IList<FunctionSignature> Signatures { get; } = new List<FunctionSignature>();

		public FunctionMethodSignatures(FunctionGroupDescriptor info)
		{
			MethodInfo = info.MethodInfo;
			Signatures = info.Signatures.Select(x => FunctionSignature.FromInfo(x)).ToList();
		}
	}

}
