using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class ConstantNumberToken : IConstantToken
	{
		public ConstantTokenType Type => ConstantTokenType.Number;
		public string Text { get; }
		public int StartIndex { get; }

		public ConstantNumberToken(ReadOnlySpan<char> expression, int index, int length)
			: this(expression.Slice(index, length).ToString(), index) { }

		public ConstantNumberToken(string text, int index)
		{
			Text = text;
			StartIndex = index;
		}

		public override string ToString() => Text.Trim();

		public bool TryGetValue([NotNullWhen(true)] out IValue? value)
		{
			value = double.TryParse(Text, out var number)
				? new NumberValue(number)
				: null as IValue;

			return value != null;
		}
	}
}
