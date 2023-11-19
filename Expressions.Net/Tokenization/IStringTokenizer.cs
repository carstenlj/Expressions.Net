using Expressions.Net.Tokenization.ITokens;
using System;

namespace Expressions.Net.Tokenization
{
	public interface IStringTokenizer
	{
		ConstantStringToken ParseQuotedStringToken(ReadOnlySpan<char> expression, int cursor, char @char);
	}
}
