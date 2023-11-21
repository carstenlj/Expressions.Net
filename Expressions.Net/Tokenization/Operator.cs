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

		public Operator(MethodInfo? methodInfo, int precedens, char char0, char? char1 = null, char? char2 = null)
		{
			MethodInfo = methodInfo;
			Precedens = (byte)precedens;
			OperandCount  = (char0 == '!' && !char1.HasValue) ? 1 : 2;
			Char0 = char0;
			Char1 = char1;
			Char2 = char2;
		}

		public override string ToString() => $"{Char0}{Char1}{Char2}";
		public override bool Equals(object? other) => other is Operator otherOp && Char0 == otherOp.Char0 && Char1 == otherOp.Char1 && Char2 == otherOp.Char2;
		public override int GetHashCode() => ToString().GetHashCode();
	}
}
