using System.Collections.Generic;
using System.Reflection;

namespace Expressions.Net.Assemblies
{
	public sealed class FunctionGroupDescriptor
	{
		public string Name { get; set; }
		public MethodInfo MethodInfo { get; set; }
		public bool IsGlobal { get; set; }
		public Dictionary<int, List<FunctionSignatureDescriptor>> Overloads { get; set; } = new Dictionary<int, List<FunctionSignatureDescriptor>>();
	}
}
