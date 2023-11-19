using System.Collections.Generic;
using System.Reflection;

namespace Expressions.Net.Assemblies
{
	public sealed class FunctionGroupDescriptor
	{
		public string Name { get; set; }
		public MethodInfo MethodInfo { get; set; }
		public IList<FunctionSignatureDescriptor> Signatures { get; set; } = new List<FunctionSignatureDescriptor>();

		public FunctionGroupDescriptor(string name, MethodInfo methodInfo)
		{
			Name = name;
			MethodInfo = methodInfo;
		}
	}
}
