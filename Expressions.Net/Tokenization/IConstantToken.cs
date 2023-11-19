using Expressions.Net.Evaluation;
using System.Diagnostics.CodeAnalysis;

namespace Expressions.Net.Tokenization
{
	internal interface IConstantToken : IToken
	{
		bool TryGetValue([NotNullWhen(true)] out IValue? value);
	}
}
