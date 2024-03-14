using Expressions.Net.Tokenization;
using System.Linq;

namespace Expressions.Net.Tests
{
	internal static class TokenCollectionExtensions
	{
		public static string[] ToTypeNames(this TokenCollectionInfix tokens) => tokens.Select(x => x.TokenType.ToString()).ToArray();
	}
}