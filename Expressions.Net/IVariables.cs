using Expressions.Net.Evaluation;

namespace Expressions.Net
{
	public interface IVariables
	{
		string[] Keys { get; }
		IValue Lookup(string variableName);
	}
}
