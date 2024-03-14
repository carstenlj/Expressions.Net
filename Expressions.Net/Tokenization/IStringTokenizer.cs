using System;

namespace Expressions.Net.Tokenization
{
	public interface IStringTokenizer
	{
		Token ParseQuotedStringToken(ReadOnlySpan<char> expression, int cursor, char @char);
	}
}
