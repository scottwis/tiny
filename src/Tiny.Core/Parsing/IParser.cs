// IParser.cs
//  
// Author:
//     Scott Wisniewski <scott@scottdw2.com>
//  
// Copyright (c) 2012 Scott Wisniewski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Tiny.Parsing
{
    //# An interface for a combinator parser with look ahead that converts an
    //# IParseState<TInput, TSourceState> to an IParseState<TInput, TResultState>
    interface IParser<TInput, in TSourceState, out TResultState>
    {
        //# The look ahead to acquire before attempting to parse input using this parser.
        //# It is up to individual combinators to figure out how to use this data. For example,
        //# [Parsers.Table] takes a set of parsers with unique LookAhead values and generates a look-up table
        //# based parser. Many combinators, such as [Parsers.Then] will ignore this value.
        TInput LookAhead { get;  }

        //# Parses the given input, if it can be accepted by this parser, and returns the new parse state.
        //# If the input can't be accepted by this parser, null is returned.
        IParseState<TInput, TResultState> Parse(IParseState<TInput, TSourceState> state);
    }
}
