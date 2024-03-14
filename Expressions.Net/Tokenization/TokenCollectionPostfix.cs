using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	public sealed class TokenCollectionPostfix : ReadOnlyCollection<Token>
	{
		public TokenCollectionPostfix(IList<Token> tokens) 
			: base(tokens) { }

		public static TokenCollectionPostfix FromInfixOrder(TokenCollectionInfix tokens)
		{
			var postfixResult = new HashSet<Token>();
			var operatorStack = new Stack<Token>();

			foreach (var token in tokens)
			{
				if (token.IsOperatorType())
				{
					// When the token is a start parenthesis or function token:
					if (token.IsBeginParenthesis())
					{
						// Push start parenthesis
						operatorStack.Push(token);
					}
					// When the token is an end parenthesis:
					else if (token.IsEndParenthesis())
					{
						// Pop all operators until the start parenthesis is met
						while (operatorStack.Any() && !operatorStack.Peek().IsBeginParenthesis())
							postfixResult.Add(operatorStack.Pop());

						// The next operator should be the start paranthesis at this point
						if (operatorStack.Any() && !operatorStack.Peek().IsBeginParenthesis())
							throw new Exception("End of expression expected");
						else
							operatorStack.Pop();
					}
					// When the token is any other operator
					else
					{
						// Pop and add to result as long as the operator on the stack has a higher precedens than the next token
						while (operatorStack.Any() && operatorStack.Peek().HasHigherPrecedensThan(token))
							postfixResult.Add(operatorStack.Pop());

						operatorStack.Push(token);
					}
				}
				else
				{
					// All operands (non-operators) are added to directly to result;
					postfixResult.Add(token);
				}
			}

			// Pops any remaning operrators and add them to the result
			while (operatorStack.Any())
				postfixResult.Add(operatorStack.Pop());

			return new TokenCollectionPostfix(postfixResult.ToList());
		}
	}
}
