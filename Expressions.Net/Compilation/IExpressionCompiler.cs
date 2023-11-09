using System;

namespace Expressions.Net.Tokenization
{
	public interface IExpressionCompiler
	{
		ExpressionDelegate Compile(TokenCollectionPostfix tokens);
	}
}
