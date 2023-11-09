using Expressions.Net.Assemblies;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Evaluation
{

	public class FunctionGroup
	{
		public MethodInfo MethodInfo { get; set; }
		public bool IsGlobal { get; set; }
		public Dictionary<int, List<FunctionSignature>> Overloads { get; set; } = new Dictionary<int, List<FunctionSignature>>();

		public FunctionGroup(FunctionGroupDescriptor info)
		{
			MethodInfo = info.MethodInfo;
			IsGlobal = info.IsGlobal;
			Overloads = info.Overloads.ToDictionary(x => x.Key, x => x.Value.Select(FunctionSignature.FromInfo).ToList());
		}
	}

}
