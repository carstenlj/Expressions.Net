using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressions.Net.Compilation
{
	internal static class ExpressionILGeneratorExtensions
	{
		public static bool EmitVariable(this ILGenerator methodIL, Token variable)
		{
			// Push the IVariables instance onto the stack
			methodIL.Emit(OpCodes.Ldarg_0);

			// Push the variable onto the stack
			methodIL.Emit(OpCodes.Ldstr, variable.GetArgument);

			// Call VarLookup (pops 2: this, variableName)
			methodIL.Emit(OpCodes.Callvirt, FunctionsMethodInfo.VarLookup);

			// Return success
			return true;
		}

		public static bool EmitNumberConstant(this ILGenerator methodIL, Token constant)
		{
			// TODO: Introduce an ITokenConstantConverter shared between the Compiler and Tokenizer to handle conversion

			// Return failure if the constant doesn't parse as a double
			if (constant.ConstantValue is null || !constant.ConstantValue.IsNumber())
				return false;

			// Push the double onto the stack
			// TODO: Push the constant directly onto the stack
			methodIL.Emit(OpCodes.Ldc_R8, constant.ConstantValue.AsDouble());

			// Call NumberValue to retrieve an IValue for the loaded constant
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.NumberValue);

			// Return success
			return true;
		}

		public static bool EmitStringConstant(this ILGenerator methodIL, Token constant)
		{
			// Push the string onto the stack
			methodIL.Emit(OpCodes.Ldstr, constant.ConstantValue?.AsString());

			// Call StringValue to retrieve an IValue for the loaded constant
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.StringValue);

			// Return success
			return true;
		}

		public static bool EmitBoolConstant(this ILGenerator methodIL, Token constant)
		{
			// TODO: Introduce an ITokenConstantConverter shared between the Compiler and Tokenizer to handle conversion

			// Return failure if the constant doesn't parse as a bool
			if (constant.ConstantValue is null || !constant.ConstantValue.IsBoolean())
				return false;

			// Push the either 1 or 0 onto the stack
			methodIL.Emit((constant.ConstantValue?.AsBoolean() ?? false) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);

			// Call BooleanValue to retrieve an IValue for the loaded constant
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.BooleanValue);

			// Return success
			return true;
		}

		public static bool EmitGetterFunctionCall(this ILGenerator methodIL, Token getterFunction)
		{
			// Push the string onto the stack
			methodIL.Emit(OpCodes.Ldstr, getterFunction.GetArgument);

			// Call Property to retrieve an IValue
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.Property);

			// Return success
			return true;
		}

		public static bool EmitFunctionCall(this ILGenerator methodIL, MethodInfo methodInfo, int nullArgsToPush = 0)
		{
			// Push null args onto the stack, to match the signature of methods with optional parameters
			for (int i = 0; i < nullArgsToPush; i++)
			{
				methodIL.Emit(OpCodes.Ldnull);
			}

			// Push the result onto the stack
			methodIL.Emit(OpCodes.Call, methodInfo);

			// Return success
			return true;
		}
	}
}
