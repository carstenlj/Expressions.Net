using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Expressions.Net.Tokenization
{
	public sealed class TokenCollectionInfix : ReadOnlyCollection<IToken>
	{
		public TokenCollectionInfix(IList<IToken> tokens) 
			: base(tokens) { }
	}
}
