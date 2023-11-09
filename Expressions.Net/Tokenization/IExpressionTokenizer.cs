using System;

namespace Expressions.Net.Tokenization
{
	public interface IExpressionTokenizer
	{
		TokenCollectionInfix Tokenize(ReadOnlySpan<char> expression);
	}

	public static class IExpressionTokenzierExtensions
	{
		public static TokenCollectionPostfix TokenizeToPostfix(this IExpressionTokenizer tokenizer, ReadOnlySpan<char> expression)
		{
			return TokenCollectionPostfix.FromInfixOrder(tokenizer.Tokenize(expression));
		}
	}
}
