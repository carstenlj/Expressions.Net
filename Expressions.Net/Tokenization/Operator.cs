namespace Expressions.Net.Tokenization
{
	internal sealed partial class Operator
	{
		public char Char0 { get; }
		public char? Char1 { get; }
		public char? Char2 { get; }
		public byte Precedens { get; }
		public string Function { get; }

		public Operator(string? function, int precedens, char char0, char? char1 = null, char? char2 = null)
		{
			Char0 = char0;
			Char1 = char1;
			Char2 = char2;
			Precedens = (byte)precedens;
			Function = function ?? string.Empty;
 		}

		public override string ToString() => $"{Char0}{Char1}{Char2}";
		public override bool Equals(object? other) => other is Operator otherOp && Char0 == otherOp.Char0 && Char1 == otherOp.Char1 && Char2 == otherOp.Char2;
		public override int GetHashCode() => ToString().GetHashCode();
	}
}
