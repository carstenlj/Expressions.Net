using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation.IValues
{
	public sealed class StringValue : ValueBase
	{
		public static readonly StringValue Empty = new StringValue(string.Empty);
		public static readonly StringValue Null = new StringValue(null);

		public StringValue(string? val) 
			: base(StringType.Invariant, val) { }
	}
}
