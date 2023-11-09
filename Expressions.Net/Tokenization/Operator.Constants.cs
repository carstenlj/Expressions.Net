using Expressions.Net.Evaluation.Functions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	internal sealed partial class Operator
	{
		public static readonly Operator ObjectAccessor = new Operator(null, 12, '.');
		public static readonly Operator Not = new Operator(nameof(OperatorFunctions.Not), 10, '!');
		public static readonly Operator Divide = new Operator(nameof(OperatorFunctions.Divide), 8, '/');
		public static readonly Operator Modulus = new Operator(nameof(OperatorFunctions.Modulus), 8, '%');
		public static readonly Operator Multiply = new Operator(nameof(OperatorFunctions.Multiply), 8, '*');
		public static readonly Operator Add = new Operator(nameof(OperatorFunctions.Add), 7, '+');
		public static readonly Operator Subtract = new Operator(nameof(OperatorFunctions.Subtract), 7, '-');
		public static readonly Operator GreaterThan = new Operator(nameof(OperatorFunctions.GreaterThan), 6, '>');
		public static readonly Operator GreaterThanOrEqual = new Operator(nameof(OperatorFunctions.GreaterThanOrEqual), 6, '>', '=');
		public static readonly Operator LessThanOrEqual = new Operator(nameof(OperatorFunctions.LessThanOrEqual), 6, '<', '=');
		public static readonly Operator LessThan = new Operator(nameof(OperatorFunctions.LessThan), 6, '<');
		public static readonly Operator Equal = new Operator(nameof(OperatorFunctions.Equal), 5, '=', '=');
		public static readonly Operator NotEqual = new Operator(nameof(OperatorFunctions.NotEqual), 5, '!', '=');
		public static readonly Operator And = new Operator(nameof(OperatorFunctions.And), 4, '&', '&');
		public static readonly Operator Coalesce = new Operator(nameof(OperatorFunctions.Coalesce), 2, '?', '?');
		public static readonly Operator Or = new Operator(nameof(OperatorFunctions.Or), 3, '|', '|');
		public static readonly Operator ArgumentSeperator = new Operator(null, 1, ',');
		public static readonly Operator ParenthesisBegin = new Operator(null, 0, '(');
		public static readonly Operator ParenthesisEnd = new Operator(null, 0, ')');

		public static readonly Operator Equal2 = new Operator(nameof(OperatorFunctions.Equal), 5, '=');
		public static readonly Operator And2 = new Operator(nameof(OperatorFunctions.And), 4, 'A', 'N', 'D');
		public static readonly Operator Or2 = new Operator(nameof(OperatorFunctions.Or), 3, 'O', 'R');

		/// <summary>
		/// All operators sorted (somewhat) by commonality
		/// </summary>
		public static readonly Operator[] All = new Operator[] {
			ParenthesisBegin, ParenthesisEnd, Add, Not, Equal, NotEqual, And, Or, LessThan, GreaterThan, ObjectAccessor,
			GreaterThanOrEqual, LessThanOrEqual,Subtract,Multiply,Divide,ArgumentSeperator,Coalesce,Modulus,
			Equal2, And2, Or2

		};

		private static readonly IDictionary<char, Operator[]> StartingCharLookup = All
			.GroupBy(x => x.Char0)
			.ToDictionary(
				keySelector: x => x.Key, 
				elementSelector: x => x.OrderBy(x => !x.Char2.HasValue).ThenBy(x => !x.Char1.HasValue).ToArray(),
				comparer: CharComparer.IgnoreCase);

		public static bool TryGetOperatorsStartingWith(char @char, [NotNullWhen(true)] out Operator[]? matchingOperators)
		{
			matchingOperators = null;

			if (StartingCharLookup.TryGetValue(@char, out var lookupResult))
				matchingOperators = lookupResult;

			return matchingOperators != null;
		}

		private class CharComparer : IEqualityComparer<char>
		{
			public static readonly CharComparer IgnoreCase = new CharComparer(true);

			private readonly bool _ignoreCase;

			public CharComparer(bool ignoreCase)
			{
				this._ignoreCase = ignoreCase;
			}

			public bool Equals(char x, char y)
			{
				if (_ignoreCase)
				{
					if (x >= 97 && x <= 122)
						x = (char)(x - 32);

					if (y >= 97 && y <= 122)
						y = (char)(y - 32);
				}

				return x.Equals(y);
			}

			public int GetHashCode(char obj)
			{
				if (_ignoreCase)
				{
					if (obj >= 97 && obj <= 122)
						obj = (char)(obj - 32);
				}

				return obj.GetHashCode();
			}
		}
	}

}
