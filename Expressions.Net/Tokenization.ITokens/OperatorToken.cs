namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class OperatorToken : IOperatorToken
	{
		public Operator Operator { get; }
		public string Text { get; }
		public int StartIndex { get; }
		public int OperandCount { get; }
		public string FunctionName => Operator.Function;
		public int Precedens => Operator.Precedens;

		public override string ToString() => Operator.ToString();

		public OperatorToken(Operator @operator, int index)
		{
			Text = @operator.ToString();
			StartIndex = index;
			Operator = @operator;
			OperandCount = @operator.Char0 == '!' && !@operator.Char1.HasValue ? 1 : 2;
		}

		public bool HasHigherPrecedensThan(IOperatorToken token)
		{
			return Precedens > token.Precedens || (OperandCount != 1 && Precedens == token.Precedens && StartIndex < token.StartIndex);
		}
	}
}
