using Expressions.Net.Evaluation;
using Xunit;

namespace Expressions.Net.Tests.IValueConverterTests
{
	[Trait("IValueConverter", "Facts")]
	public class IValueConverterTests : TestBase
	{
		[Fact(DisplayName = "Can convert to string from boolean")]
		public void Test01()
		{
			Assert.Equal("False", Converter.ConvertToString(false));
		}

		[Fact(DisplayName = "Can convert to string from number")]
		public void Test02()
		{
			Assert.Equal("13.37", Converter.ConvertToString(13.37));
		}

		[Fact(DisplayName = "Can convert to IDictionary<string,IValue> from simple annonymous object")]
		public void Test03()
		{
			var result = Converter.ConvertToDictionary(new
			{
				id = 123,
				text = "hello hest",
				success = true
			});

			Assert.Equal(3, result?.Count);
			Assert.True(result?.ContainsKey("id"));
			Assert.True(result?.ContainsKey("text"));
			Assert.True(result?.ContainsKey("success"));

			Assert.True(result?["id"].Type.RootType == ValueRootType.Number);
			Assert.True(result?["text"].Type.RootType == ValueRootType.String);
			Assert.True(result?["success"].Type.RootType == ValueRootType.Boolean);

			Assert.Equal(123d, result?["id"].Data);
			Assert.Equal("hello hest", result?["text"].Data);
			Assert.Equal(true, result?["success"].Data);
		}

		[Fact(DisplayName = "Can convert to IDictionary<string,IValue> from nested annonymous objects")]
		public void Test04()
		{
			var result = Converter.ConvertToDictionary(new
			{
				child0 = new
				{
					child1 = new
					{
						name = "hest"
					}
				}
			});

			Assert.True(result?.ContainsKey("child0"));
			Assert.True(result?["child0"].Data is IDictionary<string, IValue>);

			var child0 = result?["child0"].Data as IDictionary<string, IValue>;
			Assert.True(child0?.ContainsKey("child1"));
			Assert.True(child0?["child1"].Data is IDictionary<string, IValue>);

			var child1 = child0?["child1"].Data as IDictionary<string, IValue>;
			Assert.True(child1?.ContainsKey("name"));
			Assert.Equal("hest", child1?["name"].Data);
		}

		[Fact(DisplayName = "Can convert to IDictionary<string,IValue> from IDictionary<string,object>")]
		public void Test05()
		{
			var result = Converter.ConvertToDictionary(new Dictionary<string, object>
			{
				{ "id", 123},
				{ "text", "hello hest"},
				{ "success", true},
			});

			Assert.Equal(3, result?.Count);
			Assert.True(result?.ContainsKey("id"));
			Assert.True(result?.ContainsKey("text"));
			Assert.True(result?.ContainsKey("success"));

			Assert.True(result?["id"].Type.RootType == ValueRootType.Number);
			Assert.True(result?["text"].Type.RootType == ValueRootType.String);
			Assert.True(result?["success"].Type.RootType == ValueRootType.Boolean);

			Assert.Equal(123d, result?["id"].Data);
			Assert.Equal("hello hest", result?["text"].Data);
			Assert.Equal(true, result?["success"].Data);
		}

		[Fact(DisplayName = "Can convert to IList<IValue> from string[]")]
		public void Test06()
		{
			var result = Converter.ConvertToArray(new string[] { "hello", "world" }, ExpresisonFactory.GetStringType());

			Assert.Equal(2, result?.Count);
			Assert.Equal(ValueRootType.String, result?[0].Type.RootType);
			Assert.Equal(ValueRootType.String, result?[1].Type.RootType);
			Assert.Equal("hello", result?[0].Data);
			Assert.Equal("world", result?[1].Data);
		}
	}
}