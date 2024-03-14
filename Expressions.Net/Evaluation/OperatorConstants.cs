using Expressions.Net.Evaluation.Functions;

namespace Expressions.Net.Tokenization
{
	public static class OperatorConstants
	{
		public static readonly Operator ObjectAccessor = new Operator(null, 12, '.');
		public static readonly Operator Not = new Operator(OperatorMethodInfo.Not, 10, '!', null,null, 1);
		public static readonly Operator Divide = new Operator(OperatorMethodInfo.Divide, 8, '/');
		public static readonly Operator Modulus = new Operator(OperatorMethodInfo.Modulus, 8, '%');
		public static readonly Operator Multiply = new Operator(OperatorMethodInfo.Multiply, 8, '*');
		public static readonly Operator Add = new Operator(OperatorMethodInfo.Add, 7, '+');
		public static readonly Operator Subtract = new Operator(OperatorMethodInfo.Subtract, 7, '-');
		public static readonly Operator GreaterThan = new Operator(OperatorMethodInfo.GreaterThan, 6, '>');
		public static readonly Operator GreaterThanOrEqual = new Operator(OperatorMethodInfo.GreaterThanOrEqual, 6, '>', '=');
		public static readonly Operator LessThanOrEqual = new Operator(OperatorMethodInfo.LessThanOrEqual, 6, '<', '=');
		public static readonly Operator LessThan = new Operator(OperatorMethodInfo.LessThan, 6, '<');
		public static readonly Operator Equal = new Operator(OperatorMethodInfo.Equal, 5, '=', '=');
		public static readonly Operator NotEqual = new Operator(OperatorMethodInfo.NotEqual, 5, '!', '=');
		public static readonly Operator And = new Operator(OperatorMethodInfo.And, 4, '&', '&');
		public static readonly Operator Coalesce = new Operator(OperatorMethodInfo.Coalesce, 2, '?', '?');
		public static readonly Operator Or = new Operator(OperatorMethodInfo.Or, 3, '|', '|');
		public static readonly Operator ArgumentSeperator = new Operator(null, 1, ',');
		public static readonly Operator ParenthesisBegin = new Operator(null, 0, '(');
		public static readonly Operator ParenthesisEnd = new Operator(null, 0, ')');

		
		/// <summary>
		/// All operators sorted (somewhat) by commonality
		/// </summary>
		public static readonly Operator[] AllCoreOperators = new Operator[] {
			ParenthesisBegin, ParenthesisEnd, Add, Not, Equal, NotEqual, And, Or, LessThan, GreaterThan, ObjectAccessor,
			GreaterThanOrEqual, LessThanOrEqual,Subtract,Multiply,Divide,ArgumentSeperator,Coalesce,Modulus
		};
	}

}
