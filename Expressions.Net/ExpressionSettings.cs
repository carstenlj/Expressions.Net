using Expressions.Net.Conversion;
using System;
using System.Globalization;

namespace Expressions.Net
{
	public class ExpressionSettings : IExpressionSettings
	{
		public static readonly IExpressionSettings Default = new ExpressionSettings(
			formatProvider: CultureInfo.InvariantCulture,
			unresolvableVariableHandling: MissingValueHandling.ReturnNull,
			invalidValueHandling: InvalidValueHandling.Return
		);

		public bool CachingDisabled { get; set; }
		public IFormatProvider FormatProvider { get; }
		public MissingValueHandling MissingValueHandling { get; }
		public InvalidValueHandling InvalidValueHandling { get; }

		public ExpressionSettings(IFormatProvider? formatProvider, MissingValueHandling? unresolvableVariableHandling, InvalidValueHandling? invalidValueHandling)
		{
			FormatProvider = formatProvider ?? CultureInfo.InvariantCulture;
			MissingValueHandling = unresolvableVariableHandling ?? MissingValueHandling.ReturnNull;
			InvalidValueHandling = invalidValueHandling ?? InvalidValueHandling.Return;
		}

		public static IExpressionSettings CreateDefault()
		{
			return new ExpressionSettings(
				formatProvider: CultureInfo.InvariantCulture,
				unresolvableVariableHandling: MissingValueHandling.ReturnNull,
				invalidValueHandling: InvalidValueHandling.Return
			);
		}
	}
}
