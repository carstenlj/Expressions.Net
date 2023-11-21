using Expressions.Net.Tokenization;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Expressions.Net.Evaluation
{
	public interface IOperatorProvider
	{		
		MethodInfo? LookupOperatorMethodInfo(string @operator);
		bool TryGetNonArithmeticOperator(string operatorText, [NotNullWhen(true)] out Operator? @operator);
		bool TryGetArithmeticOperatorsStartingWith(char @char, [NotNullWhen(true)] out Operator[]? matchingOperators);
	}
}
