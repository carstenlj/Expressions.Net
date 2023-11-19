using Expressions.Net.Evaluation;
using System.Collections.Generic;

namespace Expressions.Net
{
	public interface IVariables
	{
		string[] Keys { get; }
		IDictionary<string, IValueType> Schema { get; }
		IValue Lookup(string variableName);
	}
}
