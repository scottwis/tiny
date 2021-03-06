// IToken.cs
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

using System;

namespace Tiny.Metadata.Layout
{
    //# Defines an interface for a CLR metadata token. A token is an encoding of a table-name, index pair. In
    //# different contexts in a meta-data file a varying number of bits, and distinct set of value mappings are used
    //# to encode the table an index is associated with. This interface provides a common abstraction for all of those
    //# cases.
    interface IToken : IComparable<IToken>, IEquatable<IToken>
    {
        //# Returns true if the token is null, false otherwise.
        bool IsNull { get; }

        //# The meta-data table the token reference.
        //# Throws: [InvalidOperationException] if [IsNull] returns true.
        MetadataTable Table { get; }

        //# The index within the encoded table. 
        //# Throws: [InvalidOpeationException] if [IsNull] returns true.
        ZeroBasedIndex Index { get; }
    }

    static class TokenExtensions
    {
        public static IToken CheckValid(this IToken token, string parameterName, Func<IToken, bool> predicate, string message)
        {
            return token.CheckNotNull(parameterName).Check(
                x => !x.IsNull, 
                "Token references null row", 
                parameterName
            ).Check(predicate, message, parameterName);
        }
    }
}
