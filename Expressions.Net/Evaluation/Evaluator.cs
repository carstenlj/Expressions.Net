using Expressions.Net.Evaluation.IValues;
using Expressions.Net.Tokenization;
using Expressions.Net.Tokenization.ITokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net.Evaluation
{
	internal sealed class Evaluator
	{
		public IFunctionsProvider FunctionsProvider { get; }

		public Evaluator(IFunctionsProvider functionsProvider)
		{
			FunctionsProvider = functionsProvider;
		}

		public IValue Evaluate(IEnumerable<IToken> tokensPostfix, IVariables variables)
		{
			// Evaluate the postfix ordered token using stack
			var stack = new Stack<IValue>();
			var joinOps = 0;
			foreach (var token in tokensPostfix)
			{
				if (token is VariableToken variable)
				{
					// Resolve the variable value and push the operand onto the stack
					var variableValue = variables.Lookup(variable.VariableName);
					if (variableValue == null)
						return InvalidValue.VariableNotDeclared(variable.VariableName);

					stack.Push(variableValue);
				}
				else if (token is IConstantToken constant)
				{
					// Push the operand directly onto stack
					stack.Push(constant.TryGetValue(out var value) ? value : InvalidValue.CannotResolveConstantValue(constant.Type));
				}
				else if (token is IOperatorToken @operator)
				{
					if (token is OperatorToken op && op.Operator == Operator.ArgumentSeperator)
					{
						joinOps++;
						continue;
					}

					// Pop the amound of operands needs for the operator
					var operands = new IValue[@operator.OperandCount + joinOps];
					for (int i = operands.Length; i > 0; i--)
						operands[i - 1] = stack.Pop();

					// Apply the operator using the operands and push the result back onto the stack
					var lookupResult = FunctionsProvider.LookupFunctionInfo(@operator.FunctionName, operands.Select(x => x.Type).ToArray());
					var result = (IValue)lookupResult.MethodInfo.Invoke(null, operands);

					// Reset joinOps counter after resolving function
					if (token is FunctionToken)
						joinOps = 0;

					stack.Push(result);
				}
				else
				{
					// Other tokens types are not supported
					throw new NotSupportedException($"Token type '{token.GetType().Name}' is not supported");
				}
			}

			// After looping we should be left with a single value on the stack if the expression is valid 
			if (stack.Count > 1)
				return InvalidValue.OperatorOrOperandMissing();

			// Pop and return the last value which is the result
			return stack.Pop();
		}
	}
}
