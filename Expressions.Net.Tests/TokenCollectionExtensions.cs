﻿using Expressions.Net.Tokenization;

namespace Expressions.Net.Tests
{
    internal static class TokenCollectionExtensions
    {
        public static string[] ToTypeNames(this TokenCollectionInfix tokens) => tokens.Select(x => x.GetType().Name).ToArray();
    }
}