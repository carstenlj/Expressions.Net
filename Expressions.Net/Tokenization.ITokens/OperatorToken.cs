namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class OperatorToken : IOperatorToken
	{
		public Operator Operator { get; }
		public int StartIndex { get; }
		public string Text => Operator.ToString();
		public int Precedens => Operator.Precedens;
		public int OperandCount => Operator.OperandCount;
		public string FunctionName => Operator.ToString();

		public override string ToString() => Operator.ToString();

		public OperatorToken(Operator @operator, int index)
		{
			StartIndex = index;
			Operator = @operator;
		}

		public bool HasHigherPrecedensThan(IOperatorToken token)
		{
			return Precedens > token.Precedens || (OperandCount != 1 && Precedens == token.Precedens && StartIndex < token.StartIndex);
		}
	}
}
