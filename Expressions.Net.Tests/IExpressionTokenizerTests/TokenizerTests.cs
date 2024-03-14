using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Tokenization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
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
			Assert.Equal(new[] { Token.Var, Token.Prop, Token.FuncObj }, result);
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
		public const string Bool = nameof(TokenType.ConstantBoolean);
		public const string Num = nameof(TokenType.ConstantNumber);
		public const string Str = nameof(TokenType.ConstantString);
		public const string FuncObj = nameof(TokenType.ObjectFunction);
		public const string FuncGlobl = nameof(TokenType.GlobalFunction);
		public const string Prop = nameof(TokenType.ObjectAccessor);
		public const string Inv = nameof(TokenType.Invalid);
		public const string Op = nameof(TokenType.Operator);
		public const string Var = nameof(TokenType.VariableToken);
	}
}