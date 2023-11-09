namespace Expressions.Net.Tokenization
{
	public interface IToken
	{
		string Text { get; }
		int StartIndex { get; }
	}
}
