using System;

namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class GetterFunctionToken : IOperatorToken
	{
		public string FunctionName { get; }
		public string PropertyName { get; }
		public string Text { get; }
		public int StartIndex { get; }
		public int Precedens => 12;
		public int OperandCount => 2;

		public override string ToString() => $"{FunctionName}():{PropertyName}";

		public GetterFunctionToken(ReadOnlySpan<char> expression, int index, int length)
		{
			Text = expression.Slice(index, length).ToString();
			FunctionName = "Get";
			PropertyName = Text.Trim('.');
			StartIndex = index;
		}

		public bool HasHigherPrecedensThan(IOperatorToken token)
		{
			return FunctionToken.HasHigherPrecedensThan(this, token);
		}
	}
}
