using Xunit;

namespace Expressions.Net.Tests.IValueTypeTests
{
	[Trait("IValueType.ToString", "Facts")]
	public class IValueToStringTests : TestBase
	{
		[Fact(DisplayName = "StringType serializes to 'string'")]
		public void Test01()
		{
			Assert.Equal("string", ExpressionEngine.GetStringType().ToString());
		}

		[Fact(DisplayName = "BooleanType serializes to 'boolean'")]
		public void Test02()
		{
			Assert.Equal("boolean", ExpressionEngine.GetBooleanType().ToString());
		}

		[Fact(DisplayName = "NumberType serializes to 'number'")]
		public void Test03()
		{
			Assert.Equal("number", ExpressionEngine.GetNumberType().ToString());
		}

		[Fact(DisplayName = "ArrayType<StringType> serializes to 'array<string>'")]
		public void Test04()
		{
			Assert.Equal("array<string>", ExpressionEngine.GetArrayType(ExpressionEngine.GetStringType()).ToString());
		}

		[Fact(DisplayName = "ArrayType<NumberType> serializes to 'array<number>'")]
		public void Test05()
		{
			Assert.Equal("array<number>", ExpressionEngine.GetArrayType(ExpressionEngine.GetNumberType()).ToString());
		}

		[Fact(DisplayName = "ArrayType<BooleanType> serializes to 'array<boolean>'")]
		public void Test06()
		{
			Assert.Equal("array<boolean>", ExpressionEngine.GetArrayType(ExpressionEngine.GetBooleanType()).ToString());
		}
	}
}