using System;
using System.Reflection;

namespace Expressions.Net.Evaluation.Functions
{
	public static class OperatorMethodInfo
	{
		public static readonly MethodInfo Add = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Add)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo And = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.And)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Coalesce = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Coalesce)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Divide = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Divide)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Equal = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Equal)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo GreaterThan = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.GreaterThan)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo GreaterThanOrEqual = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.GreaterThanOrEqual)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo LessThan = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.LessThan)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo LessThanOrEqual = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.LessThanOrEqual)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Modulus = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Modulus)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Multiply = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Multiply)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Not = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Not)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo NotEqual = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.NotEqual)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Or = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Or)) ?? throw new InvalidOperationException();
		public static readonly MethodInfo Subtract = typeof(OperatorFunctions).GetMethod(nameof(OperatorFunctions.Subtract)) ?? throw new InvalidOperationException();
	}
}
