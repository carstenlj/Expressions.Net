using Expressions.Net.Assemblies;
using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Evaluation
{
	internal class OperatorProvider : IOperatorProvider
	{
		public static readonly OperatorProvider Default = new OperatorProvider();

		protected virtual bool UseCoreOperators => true;

		private IDictionary<string, Operator> _keywordLookup;
		private IDictionary<char, Operator[]> arithmeticLookup;
		private IDictionary<string, MethodInfo> _methodInfos;

		public OperatorProvider()
		{
			// TODO: Ensure operators cannot start with "_". This is because the tokenizer will lookup keywords that have alphanum or underscore
			var customOperators = CustomOperators();

			_methodInfos = UseCoreOperators ? FunctionReflector.GetOperatorMethodInfo(typeof(OperatorFunctions)) : new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
			foreach (var customOperator in customOperators)
				_methodInfos.Add(customOperator.ToString(), customOperator.MethodInfo);

			var allOperators = UseCoreOperators ? new List<Operator>(Operator.AllCoreOperators) :  new List<Operator>();
			allOperators.AddRange(customOperators);

			// Group all non arithmeic operators so they can be efficiently looked up as keywords
			_keywordLookup = allOperators.Where(x => CharExtensions.IsLetter(x.Char0))
				.ToDictionary(x => x.ToString(), x => x, StringComparer.OrdinalIgnoreCase);
			
			// Group all arithmetic operator so they can be lookup efficiently by their first character
			arithmeticLookup = allOperators
				.Where(x => !CharExtensions.IsLetter(x.Char0))
				.GroupBy(x => x.Char0)
				.ToDictionary(
					keySelector: x => x.Key,
					elementSelector: x => x.OrderBy(x => !x.Char2.HasValue).ThenBy(x => !x.Char1.HasValue).ToArray());
		}

		protected virtual IEnumerable<Operator> CustomOperators() => new List<Operator>();

		public MethodInfo? LookupOperatorMethodInfo(string @operator)
		{
			if (_methodInfos.TryGetValue(@operator, out var methodInfo))
				return methodInfo;

			return null;
		}

		public bool TryGetNonArithmeticOperator(string operatorText, [NotNullWhen(true)] out Operator? @operator)
		{
			if (_keywordLookup.TryGetValue(operatorText, out var lookupResult))
				@operator = lookupResult;
			else
				@operator = null;

			return @operator != null;
		}

		public bool TryGetArithmeticOperatorsStartingWith(char char0, [NotNullWhen(true)] out Operator[]? matchingOperators)
		{
			if (arithmeticLookup.TryGetValue(char0, out var lookupResult))
				matchingOperators = lookupResult;
			else
				matchingOperators = null;

			return matchingOperators != null;
		}
	}
}
