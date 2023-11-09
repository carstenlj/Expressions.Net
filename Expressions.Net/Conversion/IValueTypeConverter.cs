using Expressions.Net.Evaluation;
using System;
using System.Collections.Generic;

namespace Expressions.Net.Conversion
{
	public interface IValueTypeConverter
	{
		IValueType ConvertToValueType(Type type);
		Type CreateTypeFromSchema(IDictionary<string, IValueType> schema);
	}
}
