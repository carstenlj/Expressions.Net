using Expressions.Net.Evaluation;
using System.Collections.Generic;

namespace Expressions.Net
{
	public interface IExpressionCache
	{
		ExpressionDelegate GetOrAdd(string expression, IDictionary<string, IValueType>? schema = null);
	}


}
