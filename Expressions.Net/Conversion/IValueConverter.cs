using Expressions.Net.Evaluation;
using System;
using System.Collections.Generic;

namespace Expressions.Net.Conversion
{
	public interface IValueConverter
	{
		string? ConvertToString(object? data);
		double? ConvertToNumber(object? data);
		DateTime? ConvertToDateTime(object? data);
		bool? ConvertToBoolean(object? data);
		IList<IValue>? ConvertToArray(object? data, IValueType itemType);
		IDictionary<string,IValue>? ConvertToDictionary(object? data, IDictionary<string,IValueType>? schema = null);
	}
}
