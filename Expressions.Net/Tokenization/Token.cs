using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using System.Globalization;
using System.Linq;
using System;

namespace Expressions.Net.Tokenization
{
	public sealed class Token
	{
		public readonly TokenType TokenType;
		public readonly int StartIndex;
		public readonly string TokenText;
		public readonly IValue? ConstantValue;
		public readonly IValueType? VariableKnownType;
		public readonly int OperatorHashCode;
		public readonly string FunctionName;
		public readonly int FunctionOperandCount;
		public readonly int FunctionPrecedens;
		public readonly bool FunctionTakesArguments;
		public readonly bool FunctionIsGlobalFunction;
		public readonly string GetArgument;

		public Token(
			TokenType tokenType,
			int startIndex,
			string tokenText,
			IValue? constantValue,
			IValueType? variableKnownType,
			int @operatorHashCode,
			string functionName,
			int functionOperandCount,
			int functionPrecedens,
			bool functionTakesArguments,
			bool functionIsGlobalFunction,
			string getArgument)
		{
			TokenType = tokenType;
			StartIndex = startIndex;
			TokenText = tokenText;
			ConstantValue = constantValue;
			VariableKnownType = variableKnownType;
			OperatorHashCode = @operatorHashCode;
			FunctionName = functionName;
			FunctionOperandCount = functionOperandCount;
			FunctionPrecedens = functionPrecedens;
			FunctionTakesArguments = functionTakesArguments;
			FunctionIsGlobalFunction = functionIsGlobalFunction;
			GetArgument = getArgument;
		}

		public override string ToString()
		{
			return TokenText;
		}

		public bool HasHigherPrecedensThan(Token token)
		{
			if (TokenType == TokenType.Operator)
				return FunctionPrecedens > token.FunctionPrecedens || (FunctionOperandCount != 1 && FunctionPrecedens == token.FunctionPrecedens && StartIndex < token.StartIndex);

			return FunctionPrecedens > token.FunctionPrecedens || (FunctionPrecedens == token.FunctionPrecedens && StartIndex < token.StartIndex);
		}

		public bool IsBeginParenthesis()
		{
			return OperatorHashCode == 544887;
		}

		public bool IsEndParenthesis()
		{
			return OperatorHashCode == 545848;
		}

		public bool IsArgumentSeperator()
		{
			return OperatorHashCode == 548731;
		}

		public bool IsOperatorType()
		{
			switch (TokenType)
			{
				case TokenType.Operator:
				case TokenType.GlobalFunction:
				case TokenType.ObjectFunction:
				case TokenType.ObjectAccessor:
					return true;
				default:
					return false;
			}
		}

		public bool IsOperandType()
		{
			switch (TokenType)
			{
				case TokenType.ConstantString:
				case TokenType.ConstantNumber:
				case TokenType.ConstantBoolean:
				case TokenType.VariableToken:
					return true;
				default:
					return false;
			}
		}

		public static Token CreateOperator(Operator @operator, int startIndex) => new Token(
			tokenType: TokenType.Operator,
			startIndex: startIndex,
			tokenText: @operator.StringValue,
			constantValue: null,
			variableKnownType: null,
			operatorHashCode: @operator.HashCode,
			functionName: @operator.StringValue,
			functionOperandCount: @operator.OperandCount,
			functionPrecedens: @operator.Precedens,
			functionTakesArguments: true,
			functionIsGlobalFunction: false,
			getArgument: String.Empty
		);

		public static Token CreateFunction(string tokenText, int startIndex, bool isGlobalFunction) => new Token(
			tokenType: isGlobalFunction ? TokenType.GlobalFunction : TokenType.ObjectFunction,
			startIndex: startIndex,
			tokenText: tokenText,
			constantValue: null,
			variableKnownType: null,
			operatorHashCode: 0,
			functionName: tokenText.Trim('.', '(', ')'),
			functionOperandCount: (EndWithStartParenthesis(tokenText) ? 2 : 1) - (isGlobalFunction ? 1 : 0),
			functionPrecedens: 12,
			functionTakesArguments: EndWithStartParenthesis(tokenText),
			functionIsGlobalFunction: isGlobalFunction,
			getArgument: String.Empty
		);

		public static Token CreateObjectAccessor(string tokenText, int startIndex) => new Token(
			tokenType: TokenType.ObjectAccessor,
			startIndex: startIndex,
			tokenText: tokenText,
			constantValue: null,
			variableKnownType: null,
			operatorHashCode: 0,
			functionName: "Get",
			functionOperandCount: 2,
			functionPrecedens: 12,
			functionTakesArguments: true,
			functionIsGlobalFunction: false,
			getArgument: tokenText.Trim('.')
		);

		public static Token CreateVariable(string tokenText, int startIndex, IValueType? knownType) => new Token(
			tokenType: TokenType.VariableToken,
			startIndex: startIndex,
			tokenText: tokenText,
			constantValue: null,
			variableKnownType: knownType,
			operatorHashCode: 0,
			functionName: String.Empty,
			functionOperandCount: -1,
			functionPrecedens: -1,
			functionTakesArguments: false,
			functionIsGlobalFunction: false,
			getArgument: tokenText
		);

		public static Token CreateConstantString(ReadOnlySpan<char> expression, int startIndex, int tokenLength, int[]? escapedIndices = null, Func<char, char>? charNormalizer = null) => new Token(
			tokenType: TokenType.ConstantString,
			startIndex: startIndex,
			tokenText: expression.Slice(startIndex, tokenLength).ToString(),
			constantValue: new StringValue(GetEscapedString(expression, startIndex, tokenLength, escapedIndices, charNormalizer)),
			variableKnownType: null,
			operatorHashCode: 0,
			functionName: String.Empty,
			functionOperandCount: -1,
			functionPrecedens: -1,
			functionTakesArguments: false,
			functionIsGlobalFunction: false,
			getArgument: String.Empty
		);

		public static Token CreateConstanNumber(string tokenText, int startIndex) => new Token(
			tokenType: TokenType.ConstantNumber,
			startIndex: startIndex,
			tokenText: tokenText,
			constantValue: double.TryParse(tokenText, NumberStyles.Number, CultureInfo.InvariantCulture, out var number) ? new NumberValue(number) : null as IValue,
			variableKnownType: null,
			operatorHashCode: 0,
			functionName: String.Empty,
			functionOperandCount: -1,
			functionPrecedens: -1,
			functionTakesArguments: false,
			functionIsGlobalFunction: false,
			getArgument: String.Empty
		);

		public static Token CreateConstanBoolean(string tokenText, int startIndex) => new Token(
			tokenType: TokenType.ConstantBoolean,
			startIndex: startIndex,
			tokenText: tokenText,
			constantValue: bool.TryParse(tokenText, out var @bool) ? new BooleanValue(@bool) : null as IValue,
			variableKnownType: null,
			operatorHashCode: 0,
			functionName: String.Empty,
			functionOperandCount: -1,
			functionPrecedens: -1,
			functionTakesArguments: false,
			functionIsGlobalFunction: false,
			getArgument: String.Empty
		);

		public static Token CreateInvalid(string tokenText, int startIndex) => new Token(
			tokenType: TokenType.Invalid,
			startIndex: startIndex,
			tokenText: tokenText,
			constantValue: null,
			variableKnownType: null,
			operatorHashCode: 0,
			functionName: String.Empty,
			functionOperandCount: -1,
			functionPrecedens: -1,
			functionTakesArguments: false,
			functionIsGlobalFunction: false,
			getArgument: String.Empty
		);

		private static bool EndWithStartParenthesis(string tokenText)
		{
			return tokenText.Last() != ')';
		}

		private static string GetEscapedString(ReadOnlySpan<char> expression, int index, int length, int[]? escapedIndices = null, Func<char, char>? normalizer = null)
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
	}
}
