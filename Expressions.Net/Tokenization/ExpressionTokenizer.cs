using Expressions.Net.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using static Expressions.Net.CharExtensions;
using static Expressions.Net.Chars;

namespace Expressions.Net.Tokenization
{
	public sealed class ExpressionTokenizer : IExpressionTokenizer
	{
		public static readonly IExpressionTokenizer Default = new ExpressionTokenizer(
			stringTokenizer: StringTokenizer.Default,
			keywordTokenizer: KeywordTokenizer.Default,
			operatorProvider: OperatorProvider.Default
		);

		internal const string True = "true";
		internal const string False = "false";

		private readonly IStringTokenizer _stringTokenizer;
		private readonly IKeywordTokenizer _keywordTokenizer;
		private readonly IOperatorProvider _operatorProvider;

		public ExpressionTokenizer(IStringTokenizer stringTokenizer, IKeywordTokenizer keywordTokenizer, IOperatorProvider operatorProvider)
		{
			_stringTokenizer = stringTokenizer;
			_keywordTokenizer = keywordTokenizer;
			_operatorProvider = operatorProvider;
		}

		public TokenCollectionInfix Tokenize(ReadOnlySpan<char> expression)
		{
			var result = new HashSet<Token>();
			var previousTokenWasOperator = false;
			var cursor = 0;

			while (cursor < expression.Length)
			{
				var token = ParseNextToken(expression, cursor, previousTokenWasOperator);
				if (token == null)
					break;

				result.Add(token);
				previousTokenWasOperator = token.TokenType == TokenType.Operator && !token.IsEndParenthesis();
				cursor = token.StartIndex + token.TokenText.Length;
			}

			return new TokenCollectionInfix(result.ToList());
		}

		private Token? ParseNextToken(ReadOnlySpan<char> expression, int cursor, bool previousTokenWasOperator)
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
				var tokenText = expression[startIndex..cursor].Trim().ToString();

				// Handle keywords
				if (_keywordTokenizer.TryTokenizeKeyword(tokenText, startIndex, @char, out var keywordToken))
					return keywordToken;

				// Handle global functions
				if (@char == OperatorConstants.ParenthesisBegin.Char0)
				{
					return expression[cursor + 1] == OperatorConstants.ParenthesisEnd.Char0 // Is function call without arguments?
						? Token.CreateFunction(expression.Slice(startIndex, ++cursor - startIndex + 1).Trim().ToString(), startIndex, true)
						: Token.CreateFunction(expression[startIndex..cursor].Trim().ToString(), startIndex, true);
				}

				// Return as a variable token
				return Token.CreateVariable(tokenText, startIndex, null);
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
					if (@char == Plus) return Token.CreateOperator(OperatorConstants.Add, startIndex);
					if (@char == Minus) return Token.CreateOperator(OperatorConstants.Subtract, startIndex);
				}

				return Token.CreateConstanNumber(expression.Slice(startIndex, cursor - startIndex).ToString(), startIndex);
			}

			// Check for operators
			if (_operatorProvider.TryGetArithmeticOperatorsStartingWith(@char, out var matchingOperators))
			{
				// List of operator starting with the current @char.
				foreach (var @operator in matchingOperators)
				{
					if ((!@operator.Char2.HasValue || cursor + 2 < expression.Length && @operator.Char2 == expression[cursor + 2]) &&
						(!@operator.Char1.HasValue || cursor + 1 < expression.Length && @operator.Char1 == expression[cursor + 1]))
					{
						return Token.CreateOperator(@operator, cursor);
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

		private static Token ParseDotPrefixedToken(ReadOnlySpan<char> expression, int startIndex, int cursor, char @char)
		{
			// Handle if last character of the expression
			if (cursor + 1 >= expression.Length)
				throw new Exception("Token '.' must be followed by a function call or property name");

			// Move cursor until encountering the first non-alphanumeric char
			while (++cursor < expression.Length && IsAlphaNumericOrUnderscore(@char = expression[cursor]))
				continue;

			// Special case: Handle non-zero-prefixed decimal notaton (eg '.42' over '0.42');
			if (IsNumberCharacter(expression[startIndex + 1]))
				return Token.CreateConstanNumber(expression[startIndex..cursor].ToString(), startIndex);

			// Handle sub-variable getter functions
			if (@char != OperatorConstants.ParenthesisBegin.Char0)
				return Token.CreateObjectAccessor(expression[startIndex..cursor].ToString(), startIndex);

			// Handle function call
			return expression[cursor + 1] == OperatorConstants.ParenthesisEnd.Char0 // Is function call without arguments?
				? Token.CreateFunction(expression.Slice(startIndex, ++cursor - startIndex + 1).ToString(), startIndex, false)
				: Token.CreateFunction(expression.Slice(startIndex, cursor - startIndex).ToString(), startIndex, false);
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