using static Expressions.Net.Chars;

namespace Expressions.Net
{
	internal static class CharExtensions
	{
		public static bool IsNumberCharacter(char @char, bool includeSigns = false)
		{
			return @char > 47 && @char < 58 || (includeSigns && (@char == Plus || @char == Minus));
		}

		public static bool IsNumericCharacter(char @char, char decimalSeperator, char thousandSeperator, ref bool hasDecimalSep)
		{
			return @char > 47 && @char < 58   // 0-9
				|| (hasDecimalSep = (@char == decimalSeperator))
				|| (@char == thousandSeperator && hasDecimalSep)
				|| @char == Underscore;
		}

		public static bool IsAlphaNumericOrUnderscore(char @char)
		{
			return @char > 47 && @char < 58   // 0-9
				|| @char > 64 && @char < 91   // A-Z
				|| @char > 96 && @char < 123  // a-z
				|| @char == Underscore;
		}

		public static bool IsLetter(char @char)
		{
			return @char > 64 && @char < 91   // A-Z
				|| @char > 96 && @char < 123; // a-z
		}

		public static bool IsLetterOrUnderscore(char @char)
		{
			return @char > 64 && @char < 122 || @char == Underscore;
		}

		public static bool IsReadableCharacter(char @char)
		{
			return @char > 32 && @char < 127;
		}

	}
}
