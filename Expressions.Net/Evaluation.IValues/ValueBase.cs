namespace Expressions.Net.Evaluation.IValues
{
	internal abstract class ValueBase : IValue
	{
		public const string NullString = "[null]";

		public IValueType Type { get; }
		public object? Data { get; }

		protected ValueBase(IValueType type, object? convertedValue)
		{
			Type = type;
			Data = convertedValue;
		}

		public override string ToString() => Data?.ToString() ?? NullString; 

	}
}
