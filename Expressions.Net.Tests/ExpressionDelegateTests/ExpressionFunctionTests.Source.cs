using Expressions.Net.Evaluation;
using System.Collections;

namespace Expressions.Net.Tests.ExpressionFunctionTests
{
	/// <summary>
	/// All test cases for the <see cref="ExpressionDelegate"/> theory
	/// </summary>
	internal class ExpressionFunctionTestSource : IEnumerable<object[]>
	{
		protected virtual FunctionsProvider FunctionsProvider => FunctionsProvider.Default;

		public record TestCase(string Expression, IValueType Returns);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<object[]> GetEnumerator()
		{
			foreach (var testCase in FunctionsProvider.FunctionsCache
				.SelectMany(x => x.Value.Signatures)
				.SelectMany(GetAllTestCasesForSignature)
				.Select(x => new object[] { x.Expression, x.Returns.ToString() }))
			{
				yield return testCase;
			}
		}

		public static IEnumerable<TestCase> GetAllTestCasesForSignature(FunctionSignature signature)
		{
			if (signature.Name == "Switch" || signature.Name == "Switch1")
				yield break;

			var argCombinations = GetDistinctiveArgCombinations(signature, 0);

			foreach (var combination in argCombinations)
			{
				if (combination.Any(x => x.IsAnyArrayItem() || x.IsAnyArray()))
				{
					foreach (var testCase in CreateArrayTestCases(signature, combination))
					{
						yield return testCase;
					}
				}
				else
				{
					yield return CreateTestCase(signature.Name, signature.IsGlobal, combination, signature.ReturnType);
				}
			}

		}

		private static IEnumerable<TestCase> CreateArrayTestCases(FunctionSignature signature, IEnumerable<IValueType> args)
		{
			var testCases = new List<TestCase>();
			var arrayItemTypes = new IValueType[] {
				IValueType.GetStringType(),
				IValueType.GetNumberType(),
				IValueType.GetBooleanType(),
				IValueType.GetDateTimeType(),
				IValueType.GetAnyObjectType()
			};

			foreach (var arrayItemType in arrayItemTypes)
			{
				testCases.Add(CreateTestCase(signature.Name, signature.IsGlobal, ReplaceTValueTypes(arrayItemType, args), signature.ReturnType.IsAnyArrayItem() ? arrayItemType : signature.ReturnType));
			}

			return testCases;
		}

		private static IEnumerable<IValueType> ReplaceTValueTypes(IValueType nonAmbigousType, IEnumerable<IValueType> args)
		{
			foreach (var arg in args)
			{
				if (arg.IsAnyArray())
					yield return IValueType.GetArrayType(nonAmbigousType);
				else if (arg.IsAnyArrayItem())
					yield return nonAmbigousType;
				else
					yield return arg;
			}
		}

		public static TestCase CreateTestCase(string functionName, bool isGlobal, IEnumerable<IValueType> args, IValueType returnType)
		{
			return isGlobal
				? new TestCase($"{functionName}({string.Join(", ", args.Select(GetTokenString))})", returnType)
				: new TestCase($"{GetTokenString(args.First())}.{functionName}({string.Join(", ", args.Skip(1).Select(GetTokenString))})", returnType);
		}

		private static string GetTokenString(IValueType type)
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
					if (arrayItemType.RootType == ValueRootType.DateTime)
						return "dateTimeArrayVar";

					throw new NotSupportedException($"Test cases with arrays of type '{arrayItemType}' is not supported");
				}
			}


			if (type.RootType == ValueRootType.Object)
				return "objVar";

			if (type.RootType == ValueRootType.DateTime)
				return "dateVar";

			return "any";
		}

		public static List<List<IValueType>> GetDistinctiveArgCombinations(FunctionSignature signature, int currentIndex = 0)
		{
			var result = new List<List<IValueType>>();

			if (currentIndex == signature.Args.Length)
			{
				result.Add(new List<IValueType>());
				return result;
			}

			var combinationsFromNextIndex = GetDistinctiveArgCombinations(signature, currentIndex + 1);
			var types = GetPossibleTypes(signature.Args[currentIndex]);
			foreach (var arrayElement in types)
			{
				foreach (var combination in combinationsFromNextIndex)
				{
					var currentCombination = new List<IValueType> { arrayElement }.Concat(combination).ToList();
					result.Add(currentCombination);
				}
			}

			return result;
		}

		public static IEnumerable<IValueType> GetPossibleTypes(IValueType valueType)
		{
			var types = valueType.GetPossibleTypes() ?? new IValueType[] { valueType.IsAnyArrayItem() ? IValueType.GetAmbigousItemType() : IValueType.GetAmbigousType() };
			var result = new List<IValueType>();

			foreach (var type in types)
			{
				if (type.IsAnyArrayItem())
				{
					result.Add(IValueType.GetAmbigousItemType());
				}
				else if (type.IsAny())
				{
					result.AddRange(new IValueType[] {
				IValueType.GetStringType(),
				IValueType.GetNumberType(),
				IValueType.GetBooleanType(),
				IValueType.GetDateTimeType(),
				IValueType.GetAnyArrayType(),
				IValueType.GetAnyObjectType()
			});
				}
				else if (type.IsAnyArray())
				{
					result.Add(IValueType.GetAnyArrayType());
				}
				else
				{
					result.Add(type);
				}
			}

			return result.Distinct();
		}




	}

	internal static class IValueTypeExtensions
	{
		public static bool IsAny(this IValueType type) => type.ToString() == "any";
		public static bool IsAnyArray(this IValueType type) => type.ToString() == "array<T>";
		public static bool IsAnyArrayItem(this IValueType type) => type.ToString() == "T";
	}

}