using Expressions.Net.Evaluation.IValueTypes;

namespace Expressions.Net.Evaluation.IValues
{
	internal readonly struct BooleanValue : IValue
	{
		public static readonly BooleanValue Empty = new BooleanValue(null);
		public static readonly BooleanValue False = new BooleanValue(false);
		public static readonly BooleanValue True = new BooleanValue(true);

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
