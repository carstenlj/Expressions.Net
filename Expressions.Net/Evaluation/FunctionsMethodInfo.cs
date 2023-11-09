using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressions.Net.Evaluation
{
	internal static class FunctionsMethodInfo
	{
		public static readonly MethodInfo Property = typeof(ObjectFunctions).GetMethod(nameof(ObjectFunctions.Property));
		public static readonly MethodInfo VarLookup = typeof(IVariables).GetMethod(nameof(IVariables.Lookup));
		public static readonly MethodInfo NumberValue = typeof(FunctionsMethodInfo).GetMethod(nameof(CreateNumber), BindingFlags.Static |BindingFlags.NonPublic);
		public static readonly MethodInfo StringValue = typeof(FunctionsMethodInfo).GetMethod(nameof(CreateString), BindingFlags.Static | BindingFlags.NonPublic);
		public static readonly MethodInfo BooleanValue = typeof(FunctionsMethodInfo).GetMethod(nameof(CreateBoolean), BindingFlags.Static | BindingFlags.NonPublic);

		//public static readonly IDictionary<string, IDictionary<int, MethodInfo>> Cache = new Dictionary<string, IDictionary<int, MethodInfo>>(StringComparer.OrdinalIgnoreCase) {
		//	{ nameof(OperatorFunctions.Add), GroupByOperandCount(typeof(OperatorFunctions), nameof(OperatorFunctions.Add))},
		//	{ nameof(OperatorFunctions.Not), GroupByOperandCount(typeof(OperatorFunctions), nameof(OperatorFunctions.Not))},
		//	{ nameof(OperatorFunctions.Multiply), GroupByOperandCount(typeof(OperatorFunctions), nameof(OperatorFunctions.Multiply))},
		//	{ nameof(OperatorFunctions.Divide), GroupByOperandCount(typeof(OperatorFunctions), nameof(OperatorFunctions.Divide))},
		//	{ nameof(CommonFunctions.Length), GroupByOperandCount(typeof(CommonFunctions), nameof(CommonFunctions.Length))}
		//};

		private static IValue CreateNumber(double val) => new NumberValue(val);
		private static IValue CreateString(string  val) => new StringValue(val);
		private static IValue CreateBoolean(bool val) => new BooleanValue(val);

		//private static IDictionary<int, MethodInfo> GroupByOperandCount(Type type, string methodName)
		//{
		//	return type
		//		.GetMethods().Where(x => x.Name == methodName)
		//		.GroupBy(x => x.GetParameters().Count())
		//		.ToDictionary(x => x.Key, x => x.Single());
		//}
	}
}
