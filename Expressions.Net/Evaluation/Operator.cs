using System.Reflection;

namespace Expressions.Net.Tokenization
{
	public sealed partial class Operator
	{
		public MethodInfo? MethodInfo { get; set; }
		public byte Precedens { get; }
		public int OperandCount { get; }
		public char Char0 { get; }
		public char? Char1 { get; }
		public char? Char2 { get; }
		public string StringValue { get; }
		public int HashCode { get; }

		public Operator(MethodInfo? methodInfo, int precedens, char char0, char? char1 = null, char? char2 = null, int operandCount = 2)
		{
			MethodInfo = methodInfo;
			Precedens = (byte)precedens;
			OperandCount = operandCount;
			Char0 = char0;
			Char1 = char1;
			Char2 = char2;
			StringValue = $"{Char0}{Char1}{Char2}";
			HashCode = GetHashCode(char0, char1, char2);
		}

		public override string ToString() => StringValue;
		public override int GetHashCode() => HashCode;
		public override bool Equals(object? other) => other is Operator otherOp && HashCode == otherOp.HashCode;

		public static int GetHashCode(char char0, char? char1, char? char2) => ((17 * 31 + (int)Normalize(char0)!) * 31 + (int)(Normalize(char1) ?? 0)) * 31 + (int)(Normalize(char2) ?? 0);
		private static char? Normalize(char? @char) => @char is null ? null : (@char >= 97 && @char <= 122 ? (char)(@char - 32) : @char);
	}
}
