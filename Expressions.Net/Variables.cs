﻿using Expressions.Net.Evaluation;
using Expressions.Net.Evaluation.IValues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressions.Net
{
	public class Variables : IVariables
	{
		public string[] Keys => Data.Keys.ToArray();
		private IDictionary<string, IValue> Data { get; }


		public Variables() : this(new Dictionary<string, IValue>()) { }
		public Variables(IDictionary<string, IValue> dictionary)
		{
			Data = new Dictionary<string, IValue>(dictionary, StringComparer.OrdinalIgnoreCase);
		}

		public IValue Lookup(string variableName) => Data[variableName];
		public Variables AddNumber(string key, double? value) => AddValue(key, new NumberValue(value));
		public Variables AddString(string key, string value) => AddValue(key, new StringValue(value));
		public Variables AddBoolean(string key, bool? value) => AddValue(key, new BooleanValue(value));
		
		public Variables AddValue(string key, IValue value)
		{
			Data.Add(key, value);
			return this;
		}
	}
}
