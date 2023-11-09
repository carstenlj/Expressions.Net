using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;

namespace Expressions.Net.Conversion
{
	public enum MissingValueHandling
	{
		ReturnNull,
		ReturnDefault,
		ReturnInvalid
	}

	internal static class MissingValueHandlingExenstions
	{
		public static IValue GetValue(this MissingValueHandling value, IValueType valueType, string? propertyName)
		{
			return value switch
			{
				MissingValueHandling.ReturnNull => valueType.CreateNullValue(),
				MissingValueHandling.ReturnDefault => valueType.CreateDefaultValue(),
				MissingValueHandling.ReturnInvalid => CreateInvalidValue(valueType, propertyName),
				_ => CreateInvalidValue(valueType, propertyName),
			};
		}

		private static IValue CreateInvalidValue(IValueType valueType, string? propertyName)
		{
			return propertyName != null
				? InvalidValue.CannotResolveProperty(propertyName, valueType)
				: InvalidValue.CannotResolveValue(valueType);
		}
	}
}
