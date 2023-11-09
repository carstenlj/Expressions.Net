using Expressions.Net.Conversion;
using Expressions.Net.DependencyInjection;
using Expressions.Net.Tokenization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Expressions.Net.Tests
{
	public abstract class TestBase
	{
		protected static IHost Host { get; } = ExpressionServiceHostBuilder.CreateDefault().Build();

		protected IExpressionFactory ExpresisonFactory => Host.Services.GetRequiredService<IExpressionFactory>();
		protected IExpressionTokenizer Tokenizer => Host.Services.GetRequiredService<IExpressionTokenizer>();
		protected IValueConverter Converter => Host.Services.GetRequiredService<IValueConverter>();

		protected static object[] Case(string expression, object equals) => new object[] { expression, equals };

		protected string[] TokenizeTypeNames(string expression)
		{
			return Tokenizer.Tokenize(expression).ToTypeNames();
		}

		protected static object TestVariablesData { get; } = new
		{
			id = 1337,
			text = "hello world",
			success = true,
			traits = new string[] {
				"test",
				"awesome"
			},
			item = new
			{
				id = 420,
				name = "yolo max",
				success = true
			},
			strArrayVar = new string[] {
				"myValue1",
				"myValue2"
			},
			objArrayVar = new object[] {
				new { id = 123, text = "Hello world" }
			},
			boolArrayVar = new bool[] {
				true, false
			},
			numArrayVar = new double[] {
				1d, 2d, 3.5d
			},
			objVar = new
			{
				id = 1337,
				name = "Hello world",
				success = true
			}
		};
	}
}