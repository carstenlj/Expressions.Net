using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using System.Diagnostics.CodeAnalysis;

namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class ConstantBooleanToken : IConstantToken
	{
		public string Text { get; }
		public int StartIndex { get; }

		public override string ToString() => Text.Trim().ToLower();

		public ConstantBooleanToken(string token, int index)
		{
			Text = token;
			StartIndex = index;
		}

		public bool TryGetValue([NotNullWhen(true)] out IValue? value)
		{
			value = bool.TryParse(Text, out var @bool)
				? new BooleanValue(@bool)
				: null as IValue;

			return value != null;
		}
	}
}
