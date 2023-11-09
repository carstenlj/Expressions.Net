namespace Expressions.Net.Tokenization
{
	internal interface IOperatorToken : IToken
	{
		public int Precedens { get; }
		public string FunctionName { get; }
		public int OperandCount { get; }

		public bool HasHigherPrecedensThan(IOperatorToken token);
	}

	internal static class IOperatorTokenExtensions
	{
		public static bool IsBeginParenthesis(this IOperatorToken token)
		{
			return Operator.ParenthesisBegin.Char0.Equals(token.Text[0]);
		}

		public static bool IsEndParenthesis(this IOperatorToken token)
		{
			return Operator.ParenthesisEnd.Char0.Equals(token.Text[0]);
		}
	}
}
