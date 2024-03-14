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

		protected virtual IExpressionEngine ExpressionEngine => Host.Services.GetRequiredService<IExpressionEngine>();
		protected virtual IExpressionTokenizer Tokenizer => Host.Services.GetRequiredService<IExpressionTokenizer>();
		protected virtual IValueConverter Converter => Host.Services.GetRequiredService<IValueConverter>();

		protected static object[] Case(string expression, object equals) => new object[] { expression, equals };

		protected string[] TokenizeTypeNames(string expression)
		{
			return Tokenizer.Tokenize(expression).ToTypeNames();
		}
	}
}