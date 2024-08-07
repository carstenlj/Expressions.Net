﻿using Xunit;

namespace Expressions.Net.Tests.IValueTypeTests
{
	[Trait("IValueType.Equals", "Facts")]
	public class IValueTypeEqualityTests : TestBase
	{
		[Fact(DisplayName = "StringType only equals StringType")]
		public void Test01()
		{
			var type = ExpressionEngine.GetStringType();

			Assert.True(type.Equals(ExpressionEngine.GetStringType()));
			Assert.False(type.Equals(ExpressionEngine.GetBooleanType()));
			Assert.False(type.Equals(ExpressionEngine.GetNumberType()));
			Assert.False(type.Equals(ExpressionEngine.GetArrayType(type)));
		}

		[Fact(DisplayName = "NumberType only equals NumberType")]
		public void Test02()
		{
			var type = ExpressionEngine.GetNumberType();

			Assert.True(type.Equals(ExpressionEngine.GetNumberType()));
			Assert.False(type.Equals(ExpressionEngine.GetBooleanType()));
			Assert.False(type.Equals(ExpressionEngine.GetStringType()));
			Assert.False(type.Equals(ExpressionEngine.GetArrayType(type)));
		}

		[Fact(DisplayName = "BooleanType only equals BooleanType")]
		public void Test03()
		{
			var type = ExpressionEngine.GetBooleanType();

			Assert.True(type.Equals(ExpressionEngine.GetBooleanType()));
			Assert.False(type.Equals(ExpressionEngine.GetStringType()));
			Assert.False(type.Equals(ExpressionEngine.GetNumberType()));
			Assert.False(type.Equals(ExpressionEngine.GetArrayType(type)));
		}
	}
}