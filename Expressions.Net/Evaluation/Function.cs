using Expressions.Net.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Evaluation
{
	public class Function
	{
		public string Name { get; }
		public IValueType[] Args { get; }
		public IValueType ReturnType { get; }
		public bool IsGlobal { get; }

		private Function(string name, IValueType[] args, IValueType returnType, bool isGlobal)
		{
			Name = name;
			Args = args;
			ReturnType = returnType;
			IsGlobal = isGlobal;
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public static IEnumerable<Function> Parse(ReadOnlySpan<char> signature)
		{
			var dotIndex = -1;
			var paramsStartIndex = -1;
			var paramsEndIndex = -1;
			var returnIndex = -1;

			for (var i = 0; i < signature.Length; i++)
			{
				var @char = signature[i];
				if (@char == '.') dotIndex = (short)i;
				if (@char == '(') paramsStartIndex = (short)i;
				if (@char == ')') paramsEndIndex = (short)i;
				if (@char == ':') returnIndex = (short)i;
			}

			var arg0 = dotIndex < 0 ? null : signature[..dotIndex];
			var startIdx = dotIndex < 0 ? 0 : dotIndex + 1;
			var methodName = signature[startIdx..paramsStartIndex];
			var index = paramsStartIndex + 1;
			var args = new List<string>();
			if (arg0.Length > 0)
				args.Add(arg0.ToString());

			// NOTE: Optmization potential. Poor performing approach
			args.AddRange(signature.Slice(paramsStartIndex + 1, paramsEndIndex - paramsStartIndex - 1).ToString().Split(',',StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));

			var returnType = signature.Slice(returnIndex + 1, signature.Length - returnIndex - 1);
			var results = new List<Function>();
			var argSets = GetCombinations(args.ToArray());

			foreach(var argSet in argSets)
			{
				results.Add(new Function(
					name: methodName.ToString(),
					args: argSet.Select(ValueTypeConverter.ConvertToValueType).ToArray(),
					returnType: ValueTypeConverter.ConvertToValueType(returnType.ToString()),
					isGlobal: dotIndex < 0
				));
			}

			return results;
		}

		public static List<string[]> GetCombinations(string[] input)
		{
			List<string[]> result = new List<string[]>();

			// Convert each item in the input array to a string array by splitting it with the "|" character
			List<string[]> splitInput = input.Select(x => x.Split('|')).ToList();

			// Compute the total number of combinations
			int totalCombinations = splitInput.Aggregate(1, (acc, arr) => acc * arr.Length);

			// Generate each combination by selecting one item from each input string array
			for (int i = 0; i < totalCombinations; i++)
			{
				string[] combination = new string[splitInput.Count];

				for (int j = 0; j < splitInput.Count; j++)
				{
					int index = (i / GetDivisor(j, splitInput)) % splitInput[j].Length;
					combination[j] = splitInput[j][index];
				}



				result.Add(combination);
			}

			return result;
		}

		// Helper function to compute the divisor for each input array
		private static int GetDivisor(int index, List<string[]> input)
		{
			int divisor = 1;

			for (int i = index + 1; i < input.Count; i++)
			{
				divisor *= input[i].Length;
			}

			return divisor;
		}

		private static int Count(ReadOnlySpan<char> input, char @char)
		{
			var result = 0;
			foreach (var c in input)
			{
				if (c == @char)
					result++;
			}
			
			return result;
		}

		struct Indexes
		{
			public readonly short DotIndex;
			public readonly short ParamsStartIndex;
			public readonly short ParamsEndIndex;
			public readonly short ReturnIndex;

			public Indexes(ReadOnlySpan<char> signature)
			{
				DotIndex = -1;
				ParamsStartIndex = -1;
				ParamsEndIndex = -1;
				ReturnIndex = -1;

				for (var i = 0; i < signature.Length; i++)
				{
					var @char = signature[i];
					if (@char == '.') DotIndex = (short)i;
					if (@char == '(') ParamsStartIndex = (short)i;
					if (@char == ')') ParamsEndIndex = (short)i;
					if (@char == ':') ReturnIndex = (short)i;
				}
			}
		}
	}
}
