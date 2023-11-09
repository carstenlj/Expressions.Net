namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class InvalidToken : IToken
	{
		public string Text { get; }
		public int StartIndex { get; }

		public InvalidToken(string text, int index)
		{
			Text = text;
			StartIndex = index;
		}
	}
}
