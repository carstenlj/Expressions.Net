using Expressions.Net.Evaluation.IValueTypes;
using System.Collections.Generic;

namespace Expressions.Net.Evaluation.IValues
{
	internal sealed class ArrayValue : ValueBase
	{
		internal ArrayValue(IList<IValue>? value, IValueType itemType) 
			: base(new ArrayType(itemType), value) { }
	}
}
