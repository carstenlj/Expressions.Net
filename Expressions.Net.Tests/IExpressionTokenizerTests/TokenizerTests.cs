using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.Functions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Reflection;
using Xunit;

namespace Expressions.Net.Tests.IExpressionTokenizerTests
{
	[Trait("IExpressionTokenizer", "Facts")]
	public class ExpressionTokenizerTests : TestBase
	{

		[Fact(DisplayName = "Can tokenize variables with getter functions")]
		public void TokenizeTest01()
		{
			var result = TokenizeTypeNames("myvar.myprop");
			Assert.Equal(new[] { Token.Var, Token.Prop }, result);
		}

		[Fact(DisplayName = "Can tokenize variables with function calls")]
		public void TokenizeTest02()
		{
			var result = TokenizeTypeNames("myvar.myprop.Length()");
			Assert.Equal(new[] { Token.Var, Token.Prop, Token.Func }, result);
		}

		//[Fact(DisplayName = "Can tokenize function with whitespace before arguments")]
		//public void TokenizeTest02()
		//{
		//	var result = TokenizeTypeNames("myvar.func ()");
		//	Assert.Equal(new string[] { Token.Var, Token.Func }, result);
		//}


	}

	public class TokenizerTestData : IEnumerable<object[]>
	{
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return Item("1 + 2 * 3", Token.Num, Token.Op, Token.Num, Token.Op, Token.Num);
			yield return Item("myvar.myprop + 1", Token.Var, Token.Prop, Token.Op, Token.Num);
		}

		private static object[] Item(string expr, params string[] expectedResult)
		{
			return new object[] { expr, expectedResult };
		}
	}

	public static class Token
	{
		public const string Bool = "ConstantBooleanToken";
		public const string Num = "ConstantNumberToken";
		public const string Str = "ConstantStringToken";
		public const string Func = "FunctionToken";
		public const string Prop = "GetterFunctionToken";
		public const string Inv = "InvalidToken";
		public const string Op = "OperatorToken";
		public const string Var = "VariableToken";
	}
}