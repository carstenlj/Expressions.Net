using Expressions.Net.Tokenization.ITokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	internal sealed class Tokenizer : IExpressionTokenizer
	{
		private const char EscapeChar = '\\';
		private const char SingleQuote = '\'';
		private const char DoubleQuote = '"';
		private const char DoubleQuoteOpening = '“';
		private const char DoubleQuoteClosing = '”';
		private const char Hypen = '-';
		private const char EnDash = '–';
		private const char EmDash = '—';
		private const char Minus2 = (char)8722;
		private const char Minus = (char)45;
		private const char ObjAccessor = '.';
		private const char DecimalPoint = '.';
		private const char DecimalSep = ',';
		private const string True = "true";
		private const string False = "false";

		public TokenCollectionInfix Tokenize(ReadOnlySpan<char> expression)
		{
			var result = new HashSet<IToken>();
			var cursor = 0;
			while (cursor < expression.Length)
			{
				var token = ParseNextToken(expression, cursor);
				if (token == null)
					break;

				result.Add(token);
				cursor = token.StartIndex + token.Text.Length;
			}

			return new TokenCollectionInfix(result.ToList());
		}

		private static IToken? ParseNextToken(ReadOnlySpan<char> expression, int cursor)
		{
			// The current character assumed to be the beginning of a new token
			var @char = NormalizeCharacter(expression[cursor]);

			// Forward the cursor until we have a readable character
			if (!NextReadableCharacter(expression, ref cursor, ref @char))
				return null;

			// Handle string tokens
			if (@char == DoubleQuote || @char == SingleQuote)
				return ParseQuotedStringToken(expression, cursor, cursor, @char);

			var startIndex = cursor;

			// Handle functions and decimal declarations
			if (@char == ObjAccessor && (ObjAccessor == DecimalPoint))
				return ParseDotPrefixedToken(expression, startIndex, cursor, @char);

			// Check for symbols (variable names, function names and keywords)
			if (IsLetterOrUnderscore(@char))
			{
				// Keep advancing until not longer reading alpha-numerics or underscores
				while (++cursor < expression.Length && IsAlphaNumericOrUnderscore(@char = expression[cursor]))
					continue;

				// Skip to the next non-whitespace without moving the curosr
				var tempCursor = cursor;
				while (tempCursor < expression.Length && char.IsWhiteSpace(@char = expression[tempCursor++]))
					continue;

				// Handle global functions
				if (@char == Operator.ParenthesisBegin.Char0)
				{
					return expression[cursor + 1] == Operator.ParenthesisEnd.Char0 // Is function call without arguments?
						? new FunctionToken(expression.Slice(startIndex, ++cursor - startIndex + 1).Trim().ToString(), startIndex, true)
						: new FunctionToken(expression.Slice(startIndex, cursor - startIndex).Trim().ToString(), startIndex, true);
				}

				// Handle boolean constants
				var token = expression.Slice(startIndex, cursor - startIndex).Trim().ToString();
				if (token.Equals(True, StringComparison.InvariantCultureIgnoreCase) || token.Equals(False, StringComparison.InvariantCultureIgnoreCase))
					return new ConstantBooleanToken(token, startIndex);

				// Handle reserved keywords better
				// TODO: Generic implementation
				if (token.Equals("AND", StringComparison.OrdinalIgnoreCase))
					return new OperatorToken(Operator.And2, startIndex);

				if (token.Equals("OR", StringComparison.OrdinalIgnoreCase))
					return new OperatorToken(Operator.Or2, startIndex);

				// Return as a variable token
				return new VariableToken(token, startIndex, null);
			}

			// Check for numeric sequences
			if (IsNumberCharacter(@char))
			{
				var hasDecimalSep = false;
				while (++cursor < expression.Length && IsNumericCharacter(expression[cursor], DecimalPoint, DecimalSep, ref hasDecimalSep))
					continue;

				// Handle that last character being an seperator
				if (cursor - 1 < expression.Length && (expression[cursor - 1] == DecimalPoint || expression[cursor - 1] == DecimalSep))
					cursor--;

				return new ConstantNumberToken(expression, startIndex, cursor - startIndex);
			}

			// Check for operators
			if (Operator.TryGetOperatorsStartingWith(@char, out var matchingOperators))
			{
				// List of operator starting with the current @char.
				foreach (var @operator in matchingOperators)
				{
					if ((!@operator.Char2.HasValue || cursor + 2 < expression.Length && @operator.Char2 == expression[cursor + 2]) &&
						(!@operator.Char1.HasValue || cursor + 1 < expression.Length && @operator.Char1 == expression[cursor + 1]))
					{
						return new OperatorToken(@operator, cursor);
					}
				}

				throw new Exception("Invalid operator");
			}

			throw new NotSupportedException();
		}

		private static bool NextReadableCharacter(ReadOnlySpan<char> expression, ref int cursor, ref char @char)
		{
			while (!IsReadableCharacter(@char))
			{
				// Advance the cursor and return null if all characters have been read
				if (++cursor >= expression.Length)
					return false;

				@char = NormalizeCharacter(expression[cursor]);
			}

			return true;
		}

		private static IToken ParseDotPrefixedToken(ReadOnlySpan<char> expression, int startIndex, int cursor, char @char)
		{
			// Handle if last character of the expression
			if (cursor + 1 >= expression.Length)
				throw new Exception("Token '.' must be followed by a function call or property name");

			// Move cursor until encountering the first non-alphanumeric char
			while (++cursor < expression.Length && IsAlphaNumericOrUnderscore(@char = expression[cursor]))
				continue;

			// Special case: Handle non-zero-prefixed decimal notaton (eg '.42' over '0.42');
			if (IsNumberCharacter(expression[startIndex + 1]))
				return new ConstantNumberToken(expression, startIndex, cursor - startIndex);

			// Handle sub-variable getter functions
			if (@char != Operator.ParenthesisBegin.Char0)
				return new GetterFunctionToken(expression, startIndex, cursor - startIndex);

			// Handle function call
			return expression[cursor + 1] == Operator.ParenthesisEnd.Char0 // Is function call without arguments?
				? new FunctionToken(expression.Slice(startIndex, ++cursor - startIndex + 1).ToString(), startIndex, false)
				: new FunctionToken(expression.Slice(startIndex, cursor - startIndex).ToString(), startIndex, false);
		}

		private static ConstantStringToken ParseQuotedStringToken(ReadOnlySpan<char> expression, int cursorStart, int cursor, char @char)
		{
			var quoteChar = @char;
			var isEscaping = false;
			var escapedIndices = null as HashSet<int>;

			while (++cursor < expression.Length)
			{
				@char = NormalizeCharacter(expression[cursor]);

				// Check if we're currently escaping a character
				if (isEscaping)
				{
					// Reset state for next iteration as we'll no longer be escaping
					isEscaping = false;

					// If the current character is eliglble for being escaped, then continue on
					if (@char == EscapeChar || @char == quoteChar)
						continue;

					// If the current character is not eligble for being escpaed, discard the escape character
					escapedIndices?.Remove(cursor - 1);

					// TODO: Should be a setting?
					//throw new NotSupportedException($"Invalid escape sequence '{EscapeChar}{@char}'");
				}

				// Check if we should be escaping the next character
				if (@char == EscapeChar)
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
			return new ConstantStringToken(expression, cursorStart, cursor - cursorStart + 1, escapedIndices?.ToArray());
		}

		private static bool IsNumberCharacter(char @char)
		{
			return @char > 47 && @char < 58;
		}

		private static bool IsNumericCharacter(char @char, char decimalSeperator, char thousandSeperator, ref bool hasDecimalSep)
		{
			return @char > 47 && @char < 58   // 0-9
				|| (hasDecimalSep = (@char == decimalSeperator))
				|| (@char == thousandSeperator && hasDecimalSep)
				|| @char == '_';
		}

		private static bool IsAlphaNumericOrUnderscore(char @char)
		{
			return @char > 47 && @char < 58   // 0-9
				|| @char > 64 && @char < 91   // A-Z
				|| @char > 96 && @char < 123  // a-z
				|| @char == '_';
		}

		private static bool IsLetterOrUnderscore(char @char)
		{
			return @char > 64 && @char < 122 || @char == '_';
		}

		private static bool IsReadableCharacter(char @char)
		{
			return @char > 32 && @char < 127;
		}

		private static char NormalizeCharacter(char @char)
		{
			if (@char == DoubleQuoteOpening || @char == DoubleQuoteClosing)
				return DoubleQuote;
				
			if (@char == Hypen || @char == EnDash || @char == EmDash || @char == Minus2)
				return Minus;
			
			return @char;
		}
	}
}