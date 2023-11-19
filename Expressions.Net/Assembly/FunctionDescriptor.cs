using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Assemblies
{
	internal sealed class FunctionDescriptor
	{
		public string Alias { get; }
		public string[] Args { get; }
		public int RequiredArgsCount { get; }
		public int NullArgCount { get; }
		public string ReturnType { get; }
		public bool IsGlobal { get; }

		private FunctionDescriptor(string alias, string[] args, string returnType, bool isGlobal)
		{
			Alias = alias;
			Args = args.Where(x => !(x is null)).Select(x => x.Trim('?')).ToArray();
			RequiredArgsCount = args.Count(x => !x.EndsWith('?'));
			NullArgCount = args.Count(x => x is null);
			ReturnType = returnType;
			IsGlobal = isGlobal;
		}

		public static FunctionDescriptor Parse(ReadOnlySpan<char> signature)
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
				if (@char == ':')
				{
					returnIndex = (short)i;
					break;
				}
			}

			var arg0 = dotIndex < 0 ? null : signature[..dotIndex];
			var startIdx = dotIndex < 0 ? 0 : dotIndex + 1;
			var methodName = signature[startIdx..paramsStartIndex];
			var index = paramsStartIndex + 1;
			var returnType = signature.Slice(returnIndex + 1, signature.Length - returnIndex - 1);
			var args = new List<string>();

			if (arg0.Length > 0)
				args.Add(arg0.ToString());

			// NOTE: Optmization potential. Poor performing approach
			args.AddRange(signature.Slice(paramsStartIndex + 1, paramsEndIndex - paramsStartIndex - 1).ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));


			return new FunctionDescriptor(
				alias: methodName.ToString(),
				args: args.ToArray(),
				returnType: returnType.ToString(),
				isGlobal: dotIndex < 0
			);
		}
	}
}
