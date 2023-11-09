using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation.IValues
{
	internal sealed class StringValue : ValueBase
	{
		public StringValue(string? val) 
			: base(StringType.Invariant, val) { }
	}
}
