using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using Expressions.Net.Evaluation.IValueTypes;
using Expressions.Net.Tokenization;
using Expressions.Net.Tokenization.ITokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressions.Net.Compilation
{
	internal sealed class ExpressionCompiler : IExpressionCompiler
	{
		private static readonly Type[] ArgumentTypes = new[] { typeof(IVariables) };
		private static readonly Type ReturnType = typeof(IValue);

		public IFunctionsProvider FunctionsProvider { get; }

		public ExpressionCompiler(IFunctionsProvider functionsProvider)
		{
			FunctionsProvider = functionsProvider;
		}

		public ExpressionDelegate Compile(TokenCollectionPostfix tokens)
		{
			// Create a new type builder to hold the Exec method for this expression
			var method = new DynamicMethod($"ExecuteExpression_{Guid.NewGuid()}", ReturnType, ArgumentTypes, true);
			var methodIL = method.GetILGenerator();
			var typeStack = new Stack<IValueType>();
			var argSeperatorCount = 0;
			var isValid = true;

			// Emit the IL foreach of the post fix ordered tokens
			foreach (var token in tokens)
			{
				// TODO: In order enable compilation errors, we need to know the specific operands of a given function or operator.
				// To achive that we basically need to simulate the execution by keeping a stack
				if (token is IOperatorToken methodToken)
				{
					if (token is OperatorToken operatorToken)
					{
						if (operatorToken.Operator == Operator.ArgumentSeperator)
						{
							argSeperatorCount++;
							continue;
						}

						isValid &= EmitFunctionToken(methodToken.FunctionName, methodToken.OperandCount, typeStack, methodIL);
						continue;
					}

					if (token is FunctionToken)
					{
						isValid &= EmitFunctionToken(methodToken.FunctionName, methodToken.OperandCount + argSeperatorCount, typeStack, methodIL);
						argSeperatorCount = 0;
						continue;
					}

					if (token is GetterFunctionToken getterToken)
					{
						// TODO: IF a schema is set, we can avoid ambigous types here
						_ = PopServeralInReserveOrder(typeStack, 1);
						typeStack.Push(AmbiguousValueType.Any);
						isValid &= methodIL.EmitGetterFunctionCall(getterToken);
						continue;
					}

					throw new InvalidOperationException($"Unrecognized {nameof(IOperatorToken)} implementation");
				}
				else
				{
					isValid &= EmitNonFunctionToken(token, typeStack, methodIL);
				}
			}

			// If the expression is not valid, return a function that consistenly yields an invalid value
			if (!isValid)
				return InvalidExpressionFunction("Unclassified error");

			// Emit return
			methodIL.Emit(OpCodes.Ret);

			// Create the delegate function
			return (ExpressionDelegate)method.CreateDelegate(typeof(ExpressionDelegate));
		}

		private bool EmitFunctionToken(string functionName, int argCount, Stack<IValueType> typeStack, ILGenerator methodIL)
		{
			var argTypes = PopServeralInReserveOrder(typeStack, argCount);
			var functionLookup = FunctionsProvider.LookupFunctionInfo(functionName, argTypes);

			if (!functionLookup.Success)
				throw new Exception($"Function '{functionName}' does not exist or no overloads exists that accepts {(string.Join(",",argTypes.Select(x => x.ToString())))} as arguments");

			if (functionLookup.ReturnType == null)
				throw new InvalidOperationException($"Function '{functionName}' exists but incorrectly declared (no return type)");

			if (functionLookup.MethodInfo == null)
				throw new InvalidOperationException($"Function '{functionName}' exists but incorrectly declared (no method info)");

			typeStack.Push(functionLookup.ReturnType);
			return methodIL.EmitFunctionCall(functionLookup.MethodInfo);
		}

		private static bool EmitNonFunctionToken(IToken token, Stack<IValueType> typeStack, ILGenerator methodIL)
		{
			if (token is ConstantStringToken stringToken)
			{
				typeStack.Push(StringType.Invariant);
				return methodIL.EmitStringConstant(stringToken);
			}

			if (token is ConstantNumberToken numberToken)
			{
				typeStack.Push(NumberType.Invariant);
				return methodIL.EmitNumberConstant(numberToken);
			}

			if (token is ConstantBooleanToken boolToken)
			{
				typeStack.Push(BooleanType.Invariant);
				return methodIL.EmitBoolConstant(boolToken);
			}

			if (token is VariableToken varToken)
			{
				// TODO: If a schema of the variable set is available we can avoid ambigous types
				typeStack.Push(AmbiguousValueType.Any);
				return methodIL.EmitVariable(varToken);
			}

			return false;
		}

		private static IValueType[] PopServeralInReserveOrder(Stack<IValueType> typeStack, int count)
		{
			var operands = new IValueType[count];
			for (var i = count; i > 0; i--)
				operands[i - 1] = typeStack.Pop();

			return operands;
		}

		private static IValue InvalidExpressionFunction(IVariables? variables = null)
		{
			return InvalidValue.InvalidExpressionFunction();
		}

		private static ExpressionDelegate InvalidExpressionFunction(string message)
		{
			return (IVariables? variables) => InvalidValue.InvalidExpressionFunction(message);
		}
	}
}
