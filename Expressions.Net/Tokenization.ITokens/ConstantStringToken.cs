using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Expressions.Net.Tokenization.ITokens
{
	internal sealed class ConstantStringToken : IConstantToken
	{
		public ConstantTokenType Type => ConstantTokenType.String;
		public string Text { get; }
		public string EscapedText { get; }
		public int StartIndex { get; }

		public ConstantStringToken(ReadOnlySpan<char> expression, int index, int length, int[]? escapedIndices)
		{
			Text = expression.Slice(index, length).ToString();
			StartIndex = index;
			EscapedText = GetEscapedString(expression, index, length, escapedIndices);
		}

		public static string GetEscapedString(ReadOnlySpan<char> expression, int index, int length, int[]? escapedIndices)
		{
			var valueArray = new char[length - 2 - (escapedIndices?.Length ?? 0)];
			index++;
			for (var i = 0; i < valueArray.Length; i++)
			{
				if (escapedIndices?.Contains(index) ?? false)
					index++;

				valueArray[i] = expression[index];
				index++;
			}

			return new string(valueArray);
		}

		public override string ToString()
		{
			return EscapedText;
		}

		public bool TryGetValue([NotNullWhen(true)] out IValue? value)
		{
			value = new StringValue(EscapedText);
			return !string.IsNullOrWhiteSpace(EscapedText);
		}

	}
}
