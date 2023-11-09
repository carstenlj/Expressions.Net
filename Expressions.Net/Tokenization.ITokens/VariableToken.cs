using Expressions.Net.Evaluation;

namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class VariableToken : IToken
	{
		public string VariableName { get; }
		public string Text { get; }
		public int StartIndex { get; }
		public IValueType? KnownType { get; }

		public override string ToString() => VariableName;

		public VariableToken(string token, int index, IValueType? knownType)
		{
			Text = token;
			StartIndex = index;
			VariableName = Text;
			KnownType = knownType;
		}
	}
}
