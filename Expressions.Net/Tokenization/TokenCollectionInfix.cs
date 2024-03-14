using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	public sealed class TokenCollectionInfix : ReadOnlyCollection<Token>
	{
		public TokenCollectionInfix(IList<Token> tokens) 
			: base(tokens) { }
	}
}
