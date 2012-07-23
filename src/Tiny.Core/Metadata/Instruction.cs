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
    //# Represents an instruction in a managed method body.
    public class Instruction
    {
        readonly Opcode m_opcode;
        readonly int m_offset;
        readonly int m_size;
        volatile object m_operand;
        readonly IReadOnlyList<byte> m_encoding;
        volatile object m_prettyPrint;
        readonly Instruction m_modifiedInstruction;

        //# Creates an instruction instance. Note that if [operand] is of type Func<Object> the value of [Operand] will
        //# be computed lazyily from the return value of the function. However, the value returned from
        //# `((Func<Object>)operand)()` cannot its self be a Func<Object>. If it is, calling [Operand] will result in
        //# an exception.
        internal Instruction(
            Opcode opcode,
            int offset,
            int size,
            object operand,
            IReadOnlyList<byte> encoding,
            String prettyPrint
        )
        {
            m_opcode = opcode.CheckDefined("opcode");
            m_offset = offset.CheckGTE(0, "offset");
            m_size = size.CheckGTE(1, "size");
            m_operand = operand;
            m_encoding = encoding;
            m_prettyPrint = prettyPrint.CheckNotNull("prettyPrint");
        }

        //# Creates an instruction instance. Note that if [operand] is of type Func<Object> the value of [Operand] will
        //# be computed lazyily from the return value of the function. However, the value returned from
        //# `((Func<Object>)operand)()` cannot its self be a Func<Object>. If it is, calling [Operand] will result in
        //# an exception.
        internal Instruction(
            Opcode opcode,
            int offset,
            int size,
            object operand,
            IReadOnlyList<byte> encoding,
            Func<String> prettyPrint,
            Instruction modifiedInstruction
        )
        {
            m_opcode = opcode.CheckDefined("opcode");
            m_offset = offset.CheckGTE(0, "offset");
            m_size = size.CheckGTE(1, "size");
            m_operand = operand;
            m_encoding = encoding;
            m_prettyPrint = prettyPrint.CheckNotNull("prettyPrint");
            m_modifiedInstruction = modifiedInstruction;
        }

        //# Returns the normalized opcode of the instruction.
        public Opcode Opcode
        {
            get { return m_opcode;  }
        }

        //# The offset of the begining of the instruction in the method body.
        public int Offset
        {
            get { return m_offset; }
        }

        //# The size of the instruction, in bytes. For instructions with prefixes ([ModifiedInstruction] `!= null`)
        //# such as "tail", or "volatile" then this will include the size of both the prefix and the target instruction.
        //# (To obtain the size of the prefix, use `Size - ModifiedInstruction.Size`).
        public int Size
        {
            get { return m_size; }
        }

        //# The instruction's operand, or null if the instruction has no operand. Note that for prefixed instructions
        //# this does not return the target instruction (some modifiers such as "constrained", "unaligned" and "no"
        //# define operands in addition to the instruction being modified, so [Operand] is reserved for that use). See
        //# [ModifiedInstruction] for more info.
        //#
        //# Note: For branch instructions this will include the branch target.
        public object Operand
        {
            get
            {
                var operand = m_operand as Func<Object>;
                if (operand != null) {
                    var o = operand();
                    if (o is Func<Object>) {
                        throw new InternalErrorException("Too many layers of indirection in instruction operand.");
                    }
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_operand, o, null);
                    #pragma warning restore 420
                }
                return m_operand;
            }
        }

        //# Returns the raw bytes encoding the instruction. For prefixed instructions this will contain the bytes for
        //# both the prefix and the target instruction. The bytes corresponding to just the prefix can be obtained using:
        //# `Encoding.SubList(Size - ModifiedInstruction.Size)`.
        public IReadOnlyList<byte> Encoding
        {
            get { return m_encoding; }
        }

        //# Returns a human readable representation of the instruction that reflects the particular instruction
        //# encoding (i.e "ldarg.0 vs ldarg 0") used. For prefixed instructions this will include both the prefix and
        //# the modified instruction (i.e. "no nullcheck tail callvirt Foo.Bar()").
        public override string ToString()
        {
            var f = m_prettyPrint as Func<String>;
            if (f != null) {
                var pretty = f();
                #pragma warning disable 420
                Interlocked.CompareExchange(ref m_prettyPrint, pretty, null);
                #pragma warning restore 420
            }
            return m_prettyPrint.ToString();
        }

        //# If the instruction is a prefixed instruction, returns the instruction being modified. For example, given a
        //# "volatile unaligned 1 ldloc.0" instruction, the values of `ModifiedInstruction.ToString()` and 
        //# `ModifiedInstruction.ModifiedInstruction.ToString()` would be "unaligned 1 ldloc.0" and "ldloc.0".
        //#
        //# Note: The name "ModifiedInstruction" is used rather than "TargetExpression" to avoid confusion when dealing
        //# with branch instructions that specify their target using the [Operand] property.
        public Instruction ModifiedInstruction
        {
            get { return m_modifiedInstruction; }
        }
    }
}
