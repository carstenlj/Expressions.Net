using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Expressions.Net.Tokenization.ITokens
{
	public sealed class ConstantStringToken : IConstantToken
	{
		public string Text { get; }
		public string EscapedText { get; }
		public int StartIndex { get; }

		public ConstantStringToken(ReadOnlySpan<char> expression, int index, int length, int[]? escapedIndices = null, Func<char,char>? charNormalizer = null)
		{
			Text = expression.Slice(index, length).ToString();
			StartIndex = index;
			EscapedText = GetEscapedString(expression, index, length, escapedIndices, charNormalizer);
		}

		public static string GetEscapedString(ReadOnlySpan<char> expression, int index, int length, int[]? escapedIndices = null, Func<char, char>? normalizer = null)
		{
			var valueArray = new char[length - 2 - (escapedIndices?.Length ?? 0)];
			index++;
			for (var i = 0; i < valueArray.Length; i++)
			{
				if (escapedIndices?.Contains(index) ?? false)
					index++;

				valueArray[i] = normalizer?.Invoke(expression[index]) ?? expression[index];
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
