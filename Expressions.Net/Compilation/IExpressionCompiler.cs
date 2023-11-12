using Expressions.Net.Evaluation;
using System.Collections.Generic;

namespace Expressions.Net.Tokenization
{
	public interface IExpressionCompiler
	{
		ExpressionDelegate Compile(TokenCollectionPostfix tokens, IDictionary<string, IValueType>? schema);
	}
}
