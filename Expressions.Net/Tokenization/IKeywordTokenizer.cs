using System.Diagnostics.CodeAnalysis;

namespace Expressions.Net.Tokenization
{
	public interface IKeywordTokenizer
	{
		bool TryTokenizeKeyword(string tokenText, int startIndex, char nextChar, [NotNullWhen(true)] out Token? token);
	}
}
