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
		public int RequiredArgsCount { get; set; }
		public IValueType ReturnType { get; set; }
		public int OptionalArgsCount => Args.Length - RequiredArgsCount;

		public static FunctionSignature FromInfo(FunctionSignatureDescriptor info)
		{
			return new FunctionSignature {
				Name = info.Name,
				IsGlobal = info.IsGlobal,
				Args = info.Args.Select(ValueTypeConverter.ConvertToValueType).ToArray(),
				RequiredArgsCount = info.RequiredArgsCount,
				ReturnType = ValueTypeConverter.ConvertToValueType(info.ReturnType)
			};
		}

		public bool SupportsArgTypes(IValueType[] args)
		{
			// Missing arguments
			if (args.Length < RequiredArgsCount)
				return false;

			// Too many arguments
			if (args.Length > this.Args.Length) 
				return false;

			for (int i = 0; i < args.Length; i++)
			{
				if (!args[i].CouldBe(this.Args[i]))
					return false;
			}

			return true;
		}

		public int GetNullArgCount(int argsSupplied)
		{
			return Args.Length - argsSupplied;
		}
	}

}
