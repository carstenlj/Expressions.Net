using Expressions.Net.Evaluation.Functions;
using Expressions.Net.Evaluation.IValues;
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

		private static IValue CreateNumber(double val) => new NumberValue(val);
		private static IValue CreateString(string  val) => new StringValue(val);
		private static IValue CreateBoolean(bool val) => new BooleanValue(val);
	}
}
