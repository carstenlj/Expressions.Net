using Xunit;

namespace Expressions.Net.Tests.IValueTypeTests
{
    [Trait("IValueType.Equals", "Facts")]
    public class IValueTypeEqualityTests : TestBase
    {
        [Fact(DisplayName = "StringType only equals StringType")]
        public void Test01()
        {
            var type = ExpresisonFactory.GetStringType();

            Assert.True(type.Equals(ExpresisonFactory.GetStringType()));
            Assert.False(type.Equals(ExpresisonFactory.GetBooleanType()));
            Assert.False(type.Equals(ExpresisonFactory.GetNumberType()));
            Assert.False(type.Equals(ExpresisonFactory.GetArrayType(type)));
        }

        [Fact(DisplayName = "NumberType only equals NumberType")]
        public void Test02()
        {
            var type = ExpresisonFactory.GetNumberType();

            Assert.True(type.Equals(ExpresisonFactory.GetNumberType()));
            Assert.False(type.Equals(ExpresisonFactory.GetBooleanType()));
            Assert.False(type.Equals(ExpresisonFactory.GetStringType()));
            Assert.False(type.Equals(ExpresisonFactory.GetArrayType(type)));
        }

        [Fact(DisplayName = "BooleanType only equals BooleanType")]
        public void Test03()
        {
            var type = ExpresisonFactory.GetBooleanType();

            Assert.True(type.Equals(ExpresisonFactory.GetBooleanType()));
            Assert.False(type.Equals(ExpresisonFactory.GetStringType()));
            Assert.False(type.Equals(ExpresisonFactory.GetNumberType()));
            Assert.False(type.Equals(ExpresisonFactory.GetArrayType(type)));
        }
    }
}