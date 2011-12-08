﻿//
// NAttrArgs
//
// Copyright (c) 2011 Pete Barber
//
// Licensed under the The Code Project Open License (CPOL.html)
// http://www.codeproject.com/info/cpol10.aspx 
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAttrArgs
{
	public class OptionalArgumentsParser<T> where T: class
	{
		private readonly IEnumerable<MemberAttribute> _options;
		private readonly T _t = null;
		private readonly ArgIterator _argIt = null;

		public OptionalArgumentsParser(T t, IEnumerable<MemberAttribute> options, ArgIterator argIt)
		{
			_t = t;
			_options = options;
			_argIt = argIt;
		}

		public void Parse()
		{
			if (_options.Count() == 0) return;

			bool isOption = false;

			while (_argIt.MoveNext() && (isOption = _argIt.Current.StartsWith("-")))
				ParseOptionalArgument();

			if (isOption == false)
				_argIt.MoveBack();
		}

		private void ParseOptionalArgument()
		{
			string argName = ParseOptionalArgName(_argIt.Current);

			MemberAttribute handler = FindMatchingOptionOrThrow(argName);

			if (handler.HasOptionalArgument == false)
				MemberSetter.SetOptionalFlagArg(_t, handler, true);
			else
			{
				if (_argIt.MoveNext() == true)
					MemberSetter.SetArg(_t, handler, _argIt.Current);
				else
					throw new Exception(string.Format("Missing value for argument for {0}", argName));
			}
		}

		private MemberAttribute FindMatchingOptionOrThrow(string argName)
		{
			try
			{
				return (from option in _options where option.ArgName == argName select option).Single();
			}
			catch (InvalidOperationException)
			{
				throw new Exception(string.Format("No matching argument for: {0}", argName));
			}
		}

		private string ParseOptionalArgName(string arg)
		{
			if (arg.Length == 1)
				throw new Exception("Missing argument name");

			return arg.Substring(1);
		}
	}
}