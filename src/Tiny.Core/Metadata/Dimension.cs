// Dimension.cs
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

using System.Text;

namespace Tiny.Metadata
{
    public struct Dimension
    {
        readonly int? m_lowerBound;
        readonly int? m_upperBound;

        public Dimension(int? lowerBound, int? upperBound)
        {
            m_lowerBound = lowerBound;
            m_upperBound = upperBound;
        }

        public int? LowerBound
        {
            get { return m_lowerBound; }
        }

        public int? UpperBound
        {
            get { return m_upperBound; }
        }

        public void GetFullName(StringBuilder b)
        {
            if (m_lowerBound.HasValue || m_upperBound.HasValue) {
                b.AppendFormat("{0}..{1}", m_lowerBound ?? 0, m_upperBound != null ? m_upperBound.ToString() : "");
            }
        }
    }
}
