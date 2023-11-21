using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization.ITokens;
using System.Diagnostics.CodeAnalysis;

namespace Expressions.Net.Tokenization
{
	public class KeywordTokenizer : IKeywordTokenizer
	{
		public const string False = "false";
		public const string True = "true";

		public static readonly KeywordTokenizer Default = new KeywordTokenizer(OperatorProvider.Default);

		private readonly IOperatorProvider _operatorProvider;

		public KeywordTokenizer(IOperatorProvider operatorProvider)
		{
			_operatorProvider = operatorProvider;
		}

		public bool TryTokenizeKeyword(string tokenText,  int startIndex, char nextChar, [NotNullWhen(true)] out IToken? token)
		{
			if (_operatorProvider.TryGetNonArithmeticOperator(tokenText, out var @operator))
				token = new OperatorToken(@operator,startIndex);

			else if (TryTokenizeCustomKeyword(tokenText, startIndex, nextChar, out token))
				return true;

			else if (tokenText.Equals(True, System.StringComparison.OrdinalIgnoreCase) || tokenText.Equals(False, System.StringComparison.OrdinalIgnoreCase))
				token = new ConstantBooleanToken(tokenText, startIndex);

			else
				token = null;

			return token != null;
		}

		protected virtual bool TryTokenizeCustomKeyword(string tokenText, int startIndex, char nextChar, [NotNullWhen(true)] out IToken? token)
		{
			return (token = null) != null;
		}
	}
}
