using Expressions.Net.Evaluation;
using System;
using System.Collections;
using Xunit;

namespace Expressions.Net.Tests.ExpressionFunctionTests
{
	/// <summary>
	/// All test cases for the <see cref="ExpressionDelegate"/> theory
	/// </summary>
	internal class ExpressionFunctionTestSource : IEnumerable<object[]>
	{
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<object[]> GetEnumerator()
		{
			var tests = GetDistinctiveTestScenarios().Select(CreateTestCase).ToList();
			foreach (var test in tests)
			{
				yield return test;
			}
		}

		private static IList<FunctionSignature> GetDistinctiveTestScenarios()
		{
			var result = new Dictionary<string, List<FunctionSignature>>();
			foreach (var item in FunctionsProvider.FunctionsCache)
			{
				var allOverloads = item.Value.Overloads.SelectMany(x => x.Value).ToArray();
				var allDistinctOverloads = new List<FunctionSignature>();
				result.Add(item.Key, allDistinctOverloads);

				foreach (var functionSignature in allOverloads)
				{
					allDistinctOverloads.AddRange(GetDistinctiveSignatures(functionSignature));
				}

			}

			return result.SelectMany(x => x.Value).ToList();
		}

		private object[] CreateTestCase(FunctionSignature func)
		{
			var prefix = func.IsGlobal ? string.Empty : $"{GetTokenString(func.Args[0])}.";
			var argString = func.Args.Length > 1 ? string.Join(',', func.Args.Skip(func.IsGlobal ? 0 : 1).Select(GetTokenString)) : string.Empty;


			return new object[] { $"{prefix}{func.Name}({argString})", func.ReturnType };
		}

		private static IEnumerable<FunctionSignature> GetDistinctiveSignatures(FunctionSignature functionSignature)
		{
			var anySubstitutions = new IValueType[] {
				IValueType.GetStringType(),
				IValueType.GetNumberType(),
				IValueType.GetBooleanType(),
				IValueType.GetAnyArrayType(),
				IValueType.GetAnyObjectType()
			};

			var arraySubstitutions = new IValueType[] {
				IValueType.GetArrayType(IValueType.GetStringType()),
				IValueType.GetArrayType(IValueType.GetNumberType()),
				IValueType.GetArrayType(IValueType.GetBooleanType()),
				//IValueType.GetArrayType(IValueType.GetArrayType(IValueType.GetAnyObjectType())),
				IValueType.GetArrayType(IValueType.GetAnyObjectType())
			};

			var args = CreateDistinctiveArgPermutations(functionSignature.Args, "any", anySubstitutions);

			foreach (var sig1 in args)
			{
				if (sig1.Any(x => x.IsAnyArray()))
				{
					foreach (var arrayType in arraySubstitutions)
					{
						yield return new FunctionSignature
						{
							Name = functionSignature.Name,
							Args = sig1.Select(x => x.SubstituteT(arrayType)).ToArray(),
							ReturnType = functionSignature.ReturnType.SubstituteT(arrayType)
						};
					}
				}
				else
				{
					yield return new FunctionSignature
					{
						Name = functionSignature.Name,
						Args = sig1,
						ReturnType = functionSignature.ReturnType
					};
				}
			}
		}

		private static List<IValueType[]> CreateDistinctiveArgPermutations(IEnumerable<IValueType> inputArgs, string substitute, IValueType[] availableSubstitutions)
		{
			var result = new List<IValueType[]>();
			var substituteCount = inputArgs.Count(item => item.ToString().Equals(substitute, StringComparison.OrdinalIgnoreCase));
			var combinations = (int)Math.Pow(availableSubstitutions.Length, substituteCount);

			for (var i = 0; i < combinations; i++)
			{
				var currentCombination = new List<IValueType>();
				var anyIndex = 0;
				for (var j = 0; j < inputArgs.Count(); j++)
				{
					var inputArg = inputArgs.ElementAt(j);
					if (!inputArg.ToString().Equals(substitute, StringComparison.OrdinalIgnoreCase))
					{
						currentCombination.Add(inputArg);
					}
					else
					{
						var valueIndex = (i / (int)Math.Pow(availableSubstitutions.Length, anyIndex)) % availableSubstitutions.Length;
						var valueType = availableSubstitutions[valueIndex];
						currentCombination.Add(valueType);
						anyIndex++;
					}
				}

				result.Add(currentCombination.ToArray());
			}

			return result;
		}

		private string GetTokenString(IValueType type)
		{
			if (type.RootType == ValueRootType.String)
				return $@"""myValue""";

			if (type.RootType == ValueRootType.Number)
				return 7.ToString();

			if (type.RootType == ValueRootType.Boolean)
				return "true";

			if (type.RootType == ValueRootType.Array)
			{
				if (type.TryGetArrayItemType(out var arrayItemType))
				{
					if (arrayItemType.RootType == ValueRootType.String)
						return "strArrayVar";
					if (arrayItemType.RootType == ValueRootType.Number)
						return "numArrayVar";
					if (arrayItemType.RootType == ValueRootType.Boolean)
						return "boolArrayVar";
					if (arrayItemType.RootType == ValueRootType.Object)
						return "objArrayVar";

					throw new NotSupportedException($"Test cases with arrays of type '{arrayItemType}' is not supported");
				}
			}
				

			if (type.RootType == ValueRootType.Object)
				return "objVar";

			return "any";
		}

	}

	internal static class IValueTypeExtensions
	{
		public static bool IsAny(this IValueType type) => type.ToString() == "any";
		public static bool IsAnyArray(this IValueType type) => type.ToString() == "array<T>";
		public static bool IsAnyArrayItem(this IValueType type) => type.ToString() == "T";

		public static IValueType SubstituteT(this IValueType type, IValueType substituteArrayType)
		{
			if (type.IsAnyArray())
				return substituteArrayType;

			if (type.IsAnyArrayItem())
				return substituteArrayType.TryGetArrayItemType(out var substituteItemType) ? substituteItemType : type;

			return type;
		}

	}
}