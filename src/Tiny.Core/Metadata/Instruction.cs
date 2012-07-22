// Instruction.cs
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
using System.Collections.Generic;
using System.Threading;

namespace Tiny.Metadata
{
    public class Instruction
    {
        readonly Opcode m_opcode;
        readonly int m_offset;
        readonly int m_size;
        readonly InstructionFlags m_flags;
        readonly byte m_alignment;
        volatile object m_operand;
        readonly IReadOnlyList<byte> m_encoding;

        internal Instruction(
            Opcode opcode,
            int offset,
            int size,
            InstructionFlags flags,
            byte alignment,
            object operand,
            IReadOnlyList<byte> encoding
        )
        {
            m_opcode = opcode.CheckDefined("opcode");
            m_offset = offset.CheckGTE(0, "offset");
            m_size = size.CheckGTE(1, "size");
            m_flags = flags;
            m_alignment = alignment;
            m_operand = operand;
            m_encoding = encoding;
        }

        public Opcode Opcode
        {
            get { return m_opcode;  }
        }

        public int Offset
        {
            get { return m_offset; }
        }

        public int Size
        {
            get { return m_size; }
        }

        public InstructionFlags Flags
        {
            get { return m_flags; }
        }

        public int? Alignment
        {
            get
            {
                if ((m_flags & InstructionFlags.Unaligned) != 0) {
                    return m_alignment;
                }
                return null;
            }
        }

        public object Operand
        {
            get
            {
                var operand = m_operand as Func<Object>;
                if (operand != null) {
                    var o = operand();
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_operand, o, null);
                    #pragma warning restore 420
                }
                return m_operand;
            }
        }

        public IReadOnlyList<byte> Encoding
        {
            get { return m_encoding; }
        }

        public override string ToString()
        {
            //TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}
