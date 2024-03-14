using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	public sealed class StringTokenizer : IStringTokenizer
	{
		public static readonly StringTokenizer Default = new StringTokenizer();

		private bool _useBackslashEscaping;

		public StringTokenizer()
		{
			// TODO: Settings
			// TODO: Support double quotes as escaping
			_useBackslashEscaping = true;
		}

		public Token ParseQuotedStringToken(ReadOnlySpan<char> expression, int cursor, char @char)
		{
			var quoteChar = @char;
			var cursorStart = cursor;
			var isEscaping = false;
			var escapedIndices = null as HashSet<int>;

			while (++cursor < expression.Length)
			{
				@char = expression[cursor];

				// Check if we're currently escaping a character
				if (isEscaping)
				{
					// Reset state for next iteration as we'll no longer be escaping
					isEscaping = false;

					// If the current character is eliglble for being escaped, then continue on
					if (@char == Chars.Backslash || @char == quoteChar)
						continue;

					throw new NotSupportedException($"Invalid escape sequence '{Chars.Backslash}{@char}'");
				}

				// Check if we should be escaping the next character
				if (_useBackslashEscaping && @char == Chars.Backslash)
				{
					isEscaping = true;

					// Add the cursor to the list of escaped indices 
					(escapedIndices ??= new HashSet<int>()).Add(cursor);
					continue;
				}

				// Break when we encounter the terminating unquoting char
				if (@char == quoteChar)
					break;
			}

			// Return the constant token
			return Token.CreateConstantString(expression, cursorStart, cursor - cursorStart + 1, escapedIndices?.ToArray());
		}
	}
}