using Expressions.Net.Evaluation;
using Expressions.Net.Tokenization.ITokens;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressions.Net.Compilation
{
	internal static class ExpressionILGeneratorExtensions
	{
		public static bool EmitVariable(this ILGenerator methodIL, VariableToken variable)
		{
			// Push the IVariables instance onto the stack
			methodIL.Emit(OpCodes.Ldarg_0);

			// Push the variable onto the stack
			methodIL.Emit(OpCodes.Ldstr, variable.VariableName);

			// Call VarLookup (pops 2: this, variableName)
			methodIL.Emit(OpCodes.Callvirt, FunctionsMethodInfo.VarLookup);

			// Return success
			return true;
		}

		public static bool EmitNumberConstant(this ILGenerator methodIL, ConstantNumberToken constant)
		{
			// TODO: Introduce an ITokenConstantConverter shared between the Compiler and Tokenizer to handle conversion

			// Return failure if the constant doesn't parse as a double
			if (!constant.TryGetValue(out var value) || !value.IsNumber())
				return false;

			// Push the double onto the stack
			methodIL.Emit(OpCodes.Ldc_R8, value.AsDouble());

			// Call NumberValue to retrieve an IValue for the loaded constant
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.NumberValue);

			// Return success
			return true;
		}

		public static bool EmitStringConstant(this ILGenerator methodIL, ConstantStringToken constant)
		{
			// Push the string onto the stack
			methodIL.Emit(OpCodes.Ldstr, constant.EscapedText);

			// Call StringValue to retrieve an IValue for the loaded constant
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.StringValue);

			// Return success
			return true;
		}

		public static bool EmitBoolConstant(this ILGenerator methodIL, ConstantBooleanToken constant)
		{
			// TODO: Introduce an ITokenConstantConverter shared between the Compiler and Tokenizer to handle conversion

			// Return failure if the constant doesn't parse as a bool
			if (!constant.TryGetValue(out var value) || !value.IsBoolean())
				return false;

			// Push the either 1 or 0 onto the stack
			methodIL.Emit(value.AsBoolean() ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);

			// Call BooleanValue to retrieve an IValue for the loaded constant
			methodIL.Emit(OpCodes.Call, FunctionsMethodInfo.BooleanValue);

			// Return success
			return true;
		}

		public static bool EmitGetterFunctionCall(this ILGenerator methodIL, GetterFunctionToken getterFunction)
		{
			// Push the string onto the stack
			methodIL.Emit(OpCodes.Ldstr, getterFunction.PropertyName);

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
