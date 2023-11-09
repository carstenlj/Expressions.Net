using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation.IValues
{
	//internal sealed class BooleanValue : ValueBase
	//{
	//	internal BooleanValue(bool? val)
	//		: base(BooleanType.Invariant, val) { }
	//}

	internal readonly struct BooleanValue : IValue
	{
		private readonly bool _value;
		private readonly bool _hasValue;

		public IValueType Type => BooleanType.Invariant;
		public object? Data => _hasValue ? (object?)_value : null;

		public BooleanValue(bool? val)
		{
			_value = val ?? false;
			_hasValue = val.HasValue;
		}

		public override string ToString() => _hasValue ? _value.ToString() : ValueBase.NullString;
	}
}
