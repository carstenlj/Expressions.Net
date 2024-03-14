using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Evaluation
{
	public class OperatorProvider : IOperatorProvider
	{
		public static readonly OperatorProvider Default = new OperatorProvider();

		protected virtual bool UseCoreOperators => true;

		private IDictionary<string, Operator> _keywordLookup;
		private IDictionary<char, Operator[]> arithmeticLookup;

		public OperatorProvider()
		{
			// TODO: Ensure operators cannot start with "_". This is because the tokenizer will lookup keywords that have alphanum or underscore
			var customOperators = CustomOperators();

			var allOperators = UseCoreOperators ? new List<Operator>(OperatorConstants.AllCoreOperators) :  new List<Operator>();
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

		public MethodInfo? LookupOperatorMethodInfo(int hashcode)
		{
			return hashcode switch
			{
				// Not
				538160 => OperatorMethodInfo.Not,

				// Divide
				551614 => OperatorMethodInfo.Divide,

				//Modulus
				542004 => OperatorMethodInfo.Modulus,

				// Multiply
				546809 => OperatorMethodInfo.Multiply,

				// Add
				547770 => OperatorMethodInfo.Add,

				// Subtract
				549692 => OperatorMethodInfo.Subtract,

				// GreaterThan
				566029 => OperatorMethodInfo.GreaterThan,

				// GreaterThanOrEqual
				567920 => OperatorMethodInfo.GreaterThanOrEqual,

				// LessThanOrEqual:
				565998 => OperatorMethodInfo.LessThanOrEqual,

				// LessThan
				564107 => OperatorMethodInfo.LessThan,

				// Equal
				566959 => OperatorMethodInfo.Equal,

				// NotEqual
				540051 => OperatorMethodInfo.NotEqual,

				// And
				544143 => OperatorMethodInfo.And,

				// Coalesce
				568943 => OperatorMethodInfo.Coalesce,

				// Or
				629455 => OperatorMethodInfo.Or,

				// Other operators
				_ => LookupCustomOperatorMethodInfo(hashcode),
			};
		}

		protected virtual MethodInfo? LookupCustomOperatorMethodInfo(int hashcode)
		{
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

		private bool Test(char char0, ReadOnlySpan<char> expression, int cursor, [NotNullWhen(true)] out Operator? result)
		{
			result = null;
			if (!arithmeticLookup.TryGetValue(char0, out var lookupResult))
				return false;

			foreach (var @operator in lookupResult)
			{
				if ((!@operator.Char2.HasValue || cursor + 2 < expression.Length && @operator.Char2 == expression[cursor + 2]) &&
					(!@operator.Char1.HasValue || cursor + 1 < expression.Length && @operator.Char1 == expression[cursor + 1]))
				{
					result = @operator;
				}
			}

			return result != null;
		}
	}
}
