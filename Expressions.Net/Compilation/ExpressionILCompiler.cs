using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using Expressions.Net.Evaluation.IValueTypes;
using Expressions.Net.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressions.Net.Compilation
{
	public sealed class ExpressionILCompiler : IExpressionCompiler
	{
		public static readonly IExpressionCompiler Default = new ExpressionILCompiler(
			operatorProvider: OperatorProvider.Default,
			functionsProvider: new FunctionsProvider()
		);

		private static readonly Type[] ArgumentTypes = new[] { typeof(IVariables) };
		private static readonly Type ReturnType = typeof(IValue);

		private readonly bool _resolveAmbigousReturnTypes = false;
		private readonly IOperatorProvider _operatorProvider;
		private readonly IFunctionsProvider _functionsProvider;

		public ExpressionILCompiler(IOperatorProvider operatorProvider, IFunctionsProvider functionsProvider)
		{
			_operatorProvider = operatorProvider;
			_functionsProvider = functionsProvider;
		}

		public ExpressionDelegate Compile(TokenCollectionPostfix tokens, IDictionary<string, IValueType>? schema)
		{
			// Create a new type builder to hold the Exec method for this expression
			var method = new DynamicMethod($"ExecuteExpression_{Guid.NewGuid()}", ReturnType, ArgumentTypes, true);
			var methodIL = method.GetILGenerator();
			var typeStack = new Stack<IValueType>();
			var isValid = true;

			// Emit the IL foreach of the post fix ordered tokens
			foreach (var token in tokens)
			{
				// TODO: In order enable compilation errors, we need to know the specific operands of a given function or operator.
				//       To achive that we basically need to simulate the execution by keeping a stack
				if (token.IsOperandType())
				{
					isValid &= EmitNonFunctionToken(token, typeStack, schema, methodIL);
				}
				else
				{
					if (token.TokenType == TokenType.Operator)
					{
						if (token.IsArgumentSeperator())
						{
							typeStack.Push(new ArgsValueType(PopServeralInReserveOrder(typeStack, 2)));
							continue;
						}

						isValid &= EmitOperatorToken(token, typeStack, methodIL);
						continue;
					}

					if (token.TokenType.IsGlobalOrObjectFunction())
					{
						isValid &= EmitFunctionToken(token, typeStack, methodIL);
						continue;
					}

					if (token.TokenType == TokenType.ObjectAccessor)
					{
						// TODO: Support schema for object types
						_ = PopServeralInReserveOrder(typeStack, 1);
						typeStack.Push(AmbiguousValueType.Any);
						isValid &= methodIL.EmitGetterFunctionCall(token);
						continue;
					}

					throw new InvalidOperationException($"Unrecognized {nameof(TokenType)} implementation");
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

		private bool EmitOperatorToken(Token operatorToken, Stack<IValueType> typeStack, ILGenerator methodIL)
		{
			var methodInfo = _operatorProvider.LookupOperatorMethodInfo(operatorToken.OperatorHashCode);
			//var methodInfo = operatorToken?.Operator?.MethodInfo;
			if (methodInfo is null)
				return false;

			// NOTE: We're currently evaluating the actual return value of the operator in order to push the correct IValueType onto the stack.
			// In order to get more precise compilation errors and the option to generate valid test cases,
			// the operator functions should follow the same declarative pattern as the functions currently do.
			var args = PopServeralInReserveOrder(typeStack, operatorToken.FunctionOperandCount);

			if (_resolveAmbigousReturnTypes && !args.OfType<AmbiguousValueType>().Any())
			{
				var result = (IValue?)methodInfo?.Invoke(null, args.Select(x => x.CreateDefaultValue()).ToArray());
				if (result?.Type is InvalidType)
					throw new InvalidOperationException($"Operator '{operatorToken.ToString()}' returns an invalid value for argument types '{string.Join(',', args.Select(x => x.ToString()))}'");

				typeStack.Push(result?.Type ?? AmbiguousValueType.Any);
			}
			else
			{
				typeStack.Push(AmbiguousValueType.Any);
			}

			methodIL.EmitFunctionCall(methodInfo);
			return true;
		}

		private bool EmitFunctionToken(Token functionToken, Stack<IValueType> typeStack, ILGenerator methodIL)
		{
			var argTypes = ArgsValueType.GetTypes(PopServeralInReserveOrder(typeStack, functionToken.FunctionOperandCount)).ToArray();
			
			var functionLookup = _functionsProvider.LookupFunctionInfo(functionToken.FunctionName, argTypes);

			if (!functionLookup.Success)
				throw new Exception($"Index {functionToken.StartIndex}: Function '{functionToken.FunctionName}' does not exist or no overloads exists that accepts '{(string.Join(",",argTypes.Select(x => x.ToString())))}' as arguments");

			if (functionLookup.ReturnType is null)
				throw new InvalidOperationException($"Index {functionToken.StartIndex}: Function '{functionToken.FunctionName}' exists but incorrectly declared (no return type)");

			if (functionLookup.MethodInfo is null)
				throw new InvalidOperationException($"Index {functionToken.StartIndex}: Function '{functionToken.FunctionName}' exists but incorrectly declared (no method info)");

			if (_resolveAmbigousReturnTypes && functionLookup.ReturnType is AmbiguousValueType && !argTypes.OfType<AmbiguousValueType>().Any())
			{
				if (functionLookup.NullArgCount > 0)
					Array.Resize(ref argTypes, argTypes.Length + functionLookup.NullArgCount);
				
				var returnType = ((IValue?)functionLookup.MethodInfo.Invoke(this, argTypes.Select(x => x?.CreateDefaultValue() ?? null).ToArray()))?.Type;
				if (returnType is InvalidType)
					throw new InvalidOperationException($"Function {functionToken} returns an invalid value for argument types '{string.Join(',', argTypes.Select(x => x.ToString()))}'");
				
				typeStack.Push(returnType ?? AmbiguousValueType.Any);
			}
			else
			{
				typeStack.Push(functionLookup.ReturnType ?? AmbiguousValueType.Any);
			}
			
			return methodIL.EmitFunctionCall(functionLookup.MethodInfo, functionLookup.NullArgCount);
		}

		private static bool EmitNonFunctionToken(Token token, Stack<IValueType> typeStack, IDictionary<string, IValueType>? schema, ILGenerator methodIL)
		{
			if (token.TokenType == TokenType.VariableToken)
			{
				if (schema?.TryGetValue(token.GetArgument, out var varValueType) ?? false)
				{
					typeStack.Push(varValueType);
				}
				else
				{
					typeStack.Push(AmbiguousValueType.Any);
				}

				return methodIL.EmitVariable(token);
			}

			if (token.TokenType == TokenType.ConstantString)
			{
				typeStack.Push(StringType.Invariant);
				return methodIL.EmitStringConstant(token);
			}

			if (token.TokenType == TokenType.ConstantNumber)
			{
				typeStack.Push(NumberType.Invariant);
				return methodIL.EmitNumberConstant(token);
			}

			if (token.TokenType == TokenType.ConstantBoolean)
			{
				typeStack.Push(BooleanType.Invariant);
				return methodIL.EmitBoolConstant(token);
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

		private static ExpressionDelegate InvalidExpressionFunction(string message)
		{
			return (IVariables? variables) => InvalidValue.InvalidExpressionFunction(message);
		}
	}
}
