using System;
using System.Linq;

namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class FunctionToken : IOperatorToken
	{
		public string FunctionName { get; }
		public bool TakesArguments { get; }
		public bool IsGlobalFunction { get; }
		public string Text { get; }
		public int StartIndex { get; }
		public int OperandCount { get; }
		public int Precedens => 12;

		public override string ToString() => $"{FunctionName}({(!TakesArguments ? string.Empty : "args")})";

		public FunctionToken(ReadOnlySpan<char> expression, int indexFrom, int indexTo, bool isGlobalFunction)
			: this(expression.Slice(indexFrom, indexTo - indexFrom).ToString(), indexFrom, isGlobalFunction) { }

		public FunctionToken(string token, int index, bool isGlobalFunction)
		{
			Text = token;
			FunctionName = Text.Trim('.', '(', ')');
			TakesArguments = Text.Last() != ')';
			IsGlobalFunction = isGlobalFunction;
			StartIndex = index;
			OperandCount = (TakesArguments ? 2 : 1) - (IsGlobalFunction ? 1 : 0);
		}

		internal static bool HasHigherPrecedensThan(IOperatorToken token1, IOperatorToken token2)
		{
			return token1.Precedens > token2.Precedens || (token1.Precedens == token2.Precedens && token1.StartIndex < token2.StartIndex);
		}

		public bool HasHigherPrecedensThan(IOperatorToken token)
		{
			return HasHigherPrecedensThan(this, token);
		}
	}
}
