using System.Linq;
using Expressions.Net.Assemblies;
using Expressions.Net.Conversion;

namespace Expressions.Net.Evaluation
{

	public class FunctionSignature
	{
		public string Name { get; set; }
		public bool IsGlobal { get; set; }
		public IValueType[] Args { get; set; }
		public IValueType ReturnType { get; set; }

		public static FunctionSignature FromInfo(FunctionSignatureDescriptor info)
		{
			return new FunctionSignature
			{
				Name = info.Name,
				IsGlobal = info.IsGlobal,
				Args = info.Args.Select(ValueTypeConverter.ConvertToValueType).ToArray(),
				ReturnType = ValueTypeConverter.ConvertToValueType(info.ReturnType)
			};
		}
	}

}
