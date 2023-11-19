using Expressions.Net.Tokenization.ITokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	internal sealed class Tokenizer : IExpressionTokenizer
	{
		internal const char Backslash = '\\';
		internal const char SingleQuote = '\'';
		internal const char SingleQuoteLeft = (char)8216;
		internal const char SingleQuoteRight = (char)8217;
		internal const char DoubleQuote = '"';
		internal const char DoubleQuoteOpening = '“';
		internal const char DoubleQuoteClosing = '”';
		internal const char Underscore = '_';
		internal const char Hypen = '-';
		internal const char EnDash = '–';
		internal const char EmDash = '—';
		internal const char Minus2 = (char)8722;
		internal const char Minus = (char)45;
		internal const char Plus = '+';
		internal const char ObjAccessor = '.';
		internal const char DecimalPoint = '.';
		internal const char DecimalSep = ',';
		internal const string True = "true";
		internal const string False = "false";


		private readonly IStringTokenizer _stringTokenizer;

		public Tokenizer(IStringTokenizer stringTokenizer)
		{
			_stringTokenizer = stringTokenizer;
		}

		public TokenCollectionInfix Tokenize(ReadOnlySpan<char> expression)
		{
			var result = new HashSet<IToken>();
			var previousTokenWasOperator = false;
			var cursor = 0;

			while (cursor < expression.Length)
			{
				var token = ParseNextToken(expression, cursor, previousTokenWasOperator);
				if (token == null)
					break;

				result.Add(token);
				previousTokenWasOperator = token is OperatorToken operatorToken && !operatorToken.IsEndParenthesis();
				cursor = token.StartIndex + token.Text.Length;
			}

			return new TokenCollectionInfix(result.ToList());
		}

		private IToken? ParseNextToken(ReadOnlySpan<char> expression, int cursor, bool previousTokenWasOperator)
		{
			// The current character assumed to be the beginning of a new token
			var @char = NormalizeCharacter(expression[cursor]);

			// Forward the cursor until we have a readable character
			if (!NextReadableCharacter(expression, ref cursor, ref @char))
				return null;

			// Handle string tokens
			if (@char == DoubleQuote || @char == SingleQuote)
				return _stringTokenizer.ParseQuotedStringToken(expression, cursor, @char);

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

				// NOTE: At this point @char will be the next readable character not included in the current tokenText

				// TODO: 
				// We're currently materializing the tokenText here. Consider if it is worth delaying.
				// Although it should be kept in mind that the constructors of the Function, Variable and Constant token will materialize
				// In order to truly gain the full benefit from avoiding materializing here, all the token constructor should be changed to not do so themselves.
				var tokenText = expression.Slice(startIndex, cursor - startIndex).Trim().ToString();

				// TODO: All keywords (non-method and non-variable symbols) should be resolved centrally.
				// Here we simply do a lookup to determine if the token text represents a registered keyword, and then return the keyword token
				// Handle boolean constants
				if (tokenText.Equals(True, StringComparison.InvariantCultureIgnoreCase) || tokenText.Equals(False, StringComparison.InvariantCultureIgnoreCase))
					return new ConstantBooleanToken(tokenText, startIndex);

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
				return new VariableToken(tokenText, startIndex, null);
			}

			// Check for numeric sequences
			// TODO: A  number sequence should only be able to start  with a single sign
			if (IsNumberCharacter(@char, previousTokenWasOperator))
			{
				var hasDecimalSep = false;
				while (++cursor < expression.Length && IsNumericCharacter(expression[cursor], DecimalPoint, DecimalSep, ref hasDecimalSep))
					continue;

				// Handle that last character being an seperator
				if (cursor - 1 < expression.Length && (expression[cursor - 1] == DecimalPoint || expression[cursor - 1] == DecimalSep))
					cursor--;

				if (cursor - startIndex == 1)
				{
					if (@char == Plus)
						return new OperatorToken(Operator.Add, startIndex);

					if (@char == Minus)
						return new OperatorToken(Operator.Subtract, startIndex);
				}

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

		private static bool IsNumberCharacter(char @char, bool includeSigns = false)
		{
			return @char > 47 && @char < 58 || (includeSigns && (@char == Plus || @char == Minus));
		}

		private static bool IsNumericCharacter(char @char, char decimalSeperator, char thousandSeperator, ref bool hasDecimalSep)
		{
			return @char > 47 && @char < 58   // 0-9
				|| (hasDecimalSep = (@char == decimalSeperator))
				|| (@char == thousandSeperator && hasDecimalSep)
				|| @char == Underscore;
		}

		private static bool IsAlphaNumericOrUnderscore(char @char)
		{
			return @char > 47 && @char < 58   // 0-9
				|| @char > 64 && @char < 91   // A-Z
				|| @char > 96 && @char < 123  // a-z
				|| @char == Underscore;
		}

		private static bool IsLetterOrUnderscore(char @char)
		{
			return @char > 64 && @char < 122 || @char == Underscore;
		}

		private static bool IsReadableCharacter(char @char)
		{
			return @char > 32 && @char < 127;
		}

		private static char NormalizeCharacter(char @char)
		{
			// TODO: Make this a settings: Properly formatted data should not be using “ and ” as string quotation marks
			if (@char == DoubleQuoteOpening || @char == DoubleQuoteClosing)
				return DoubleQuote;

			if (@char == Hypen || @char == EnDash || @char == EmDash || @char == Minus2)
				return Minus;

			if (@char == SingleQuoteRight || @char == SingleQuoteLeft)
				return SingleQuote;

			return @char;
		}
	}
}