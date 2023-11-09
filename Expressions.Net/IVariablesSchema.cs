using Expressions.Net.Evaluation;

namespace Expressions.Net
{
	public interface IVariablesSchema
	{
		IValueType Lookup(string variableName);
	}
}
