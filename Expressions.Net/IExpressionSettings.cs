using Expressions.Net.Conversion;
using System;

namespace Expressions.Net
{
	public interface IExpressionSettings
	{
		IFormatProvider FormatProvider { get; }
		MissingValueHandling MissingValueHandling { get; }
		InvalidValueHandling InvalidValueHandling { get; }
	}
}