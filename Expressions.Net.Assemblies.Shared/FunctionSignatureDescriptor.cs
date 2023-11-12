namespace Expressions.Net.Assemblies
{
	/// <summary>
	/// A typeless representation of the FunctionSignature class in the main assembly
	/// </summary>
	public sealed class FunctionSignatureDescriptor
	{
		public string Name { get;  }
		public bool IsGlobal { get; }
		public string[] Args { get; }
		public int RequiredArgsCount { get;  }
		public string ReturnType { get;  }

		public FunctionSignatureDescriptor(string name, bool isGlobal, string[] args, int requiredArgsCount, string returnType)

		{
			Name = name;
			IsGlobal = isGlobal;
			Args = args;
			RequiredArgsCount = requiredArgsCount;
			ReturnType = returnType;
		}
	}
}
