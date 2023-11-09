using System;
using System.Collections;
using System.Linq;

namespace Expressions.Net.Conversion
{
	internal static class TypeExtensions
	{
		public static bool IsNumericType(this Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		public static bool IsEnumerable(this Type type, out Type itemType)
		{
			itemType = typeof(void);

			if (type == typeof(string))
				return false;

			if (type.IsArray)
			{
				itemType = type.GetElementType();
				return true;
			}
				

			if (typeof(IEnumerable).IsAssignableFrom(type) && type.GenericTypeArguments.Length == 1)
			{
				itemType = type.GenericTypeArguments.First();
				return true;
			}

			return false;
		}
	}
}
