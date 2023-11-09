using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation.IValues
{
	//internal sealed class NumberValue : ValueBase
	//{
	//	public NumberValue(double? val)
	//		: base(NumberType.Invariant, val) { }
	//}

	internal readonly struct NumberValue : IValue
	{
		private readonly double _value;
		private readonly bool _hasValue;

		public IValueType Type => NumberType.Invariant;
		public object? Data => _hasValue ? (object?)_value : null;

		public NumberValue(double? val)
		{
			_value = val ?? 0;
			_hasValue = val.HasValue;
		}

		public override string ToString() => _hasValue ? _value.ToString() : ValueBase.NullString;
	}
}
