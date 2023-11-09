namespace Expressions.Net.Assemblies
{
	/// <summary>
	/// A typeless representation of the FunctionSignature class in the main assembly
	/// </summary>
	public sealed class FunctionSignatureDescriptor
	{
		public string Name { get; set; }
		public bool IsGlobal { get; set; }
		public string[] Args { get; set; }
		public string ReturnType { get; set; }
	}
}
