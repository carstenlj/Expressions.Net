namespace Expressions.Net.Tokenization
{
	public enum TokenType
	{
		ConstantString,
		ConstantNumber,
		ConstantBoolean,
		VariableToken,
		Operator,
		GlobalFunction,
		ObjectFunction,
		ObjectAccessor,
		Invalid,
	}

	public static class TokenTypeExtensions
	{
		public static bool IsAnyFunctionOrOperator(this TokenType @this)
		{
			switch (@this)
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

		public static bool IsGlobalOrObjectFunction(this TokenType @this)
		{
			switch (@this)
			{
				case TokenType.GlobalFunction:
				case TokenType.ObjectFunction:
					return true;
				default:
					return false;
			}
		}
	}
}
