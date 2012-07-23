// InstructionParser.cs
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
using System.Linq;
using Tiny.Parsing;

namespace Tiny.Metadata.Layout
{
    class InstructionParser
    {
        static IParser<byte, InstructionParseState, Instruction> s_parser = Grammar(
            LookFor(new byte[] {0xFE, 0x16}).Then(SetOpcode(Opcode.Constrained, "constrained")).Then(ParseType()).Then(ParseNestedInstruction()),
            LookFor(new byte[] {0xFE, 0x19}).Then(SetOpcode(Opcode.No, "no")).Then(ParseSkipFlags()).Then(ParseNestedInstruction()),
            LookFor(new byte[] {0xFE, 0x1E}).Then(SetOpcode(Opcode.Readonly, "readonly")).Then(ParseNestedInstruction()),
            LookFor(new byte[] {0xFE, 0x14}).Then(SetOpcode(Opcode.Tail, "tail")).Then(ParseNestedInstruction()),
            LookFor(new byte[] {0xFE, 0x12}).Then(SetOpcode(Opcode.Unaligned, "unaligned")).Then(ParseByteOperand()).Then(ParseNestedInstruction()),
            LookFor(new byte[] {0xFE, 0x13}).Then(SetOpcode(Opcode.Volatile, "volatile")).Then(ParseNestedInstruction()),
            LookFor(0x58).Then(SetOpcode(Opcode.Add, "add")),
            LookFor(0xD6).Then(SetOpcode(Opcode.AddOvf, "add.ovf")),
            LookFor(0xD7).Then(SetOpcode(Opcode.AddOvfUn, "add.ovf.un")),
            LookFor(0x5F).Then(SetOpcode(Opcode.And, "and")),
            LookFor(new byte[] {0xFE, 0x00}).Then(SetOpcode(Opcode.ArgList, "arglist")),
            LookFor(0x3B).Then(SetOpcode(Opcode.Beq, "beq")).Then(ParseBranchTarget32()),
            LookFor(0x2B).Then(SetOpcode(Opcode.Beq, "beq.s")).Then(ParseBranchTarget8()),
            LookFor(0x3C).Then(SetOpcode(Opcode.Bge, "bge")).Then(ParseBranchTarget32()),
            LookFor(0x2F).Then(SetOpcode(Opcode.Bge, "bge.s")).Then(ParseBranchTarget8()),
            LookFor(0x41).Then(SetOpcode(Opcode.BgeUn, "bge.un")).Then(ParseBranchTarget32()),
            LookFor(0x34).Then(SetOpcode(Opcode.BgeUn, "bge.un.s")).Then(ParseBranchTarget8()),
            LookFor(0x3D).Then(SetOpcode(Opcode.Bgt, "bgt")).Then(ParseBranchTarget32()),
            LookFor(0x30).Then(SetOpcode(Opcode.Bgt, "bgt.s")).Then(ParseBranchTarget8()),
            LookFor(0x42).Then(SetOpcode(Opcode.BgtUn, "bgt.un")).Then(ParseBranchTarget32()),
            LookFor(0x35).Then(SetOpcode(Opcode.BgtUn, "bgt.un.s")).Then(ParseBranchTarget8()),
            LookFor(0x3E).Then(SetOpcode(Opcode.Ble, "ble")).Then(ParseBranchTarget32()),
            LookFor(0x31).Then(SetOpcode(Opcode.Ble, "ble.s")).Then(ParseBranchTarget8()),
            LookFor(0x43).Then(SetOpcode(Opcode.BleUn, "ble.un")).Then(ParseBranchTarget32()),
            LookFor(0x36).Then(SetOpcode(Opcode.BleUn, "ble.un.s")).Then(ParseBranchTarget8()),
            LookFor(0x3F).Then(SetOpcode(Opcode.Blt, "blt")).Then(ParseBranchTarget32()),
            LookFor(0x32).Then(SetOpcode(Opcode.Blt, "blt.s")).Then(ParseBranchTarget8())

            //TODO: Finish this
        );

        static IParser<byte, InstructionParseState, Instruction> Grammar(
            params IParser<byte, InstructionParseState, InstructionParseState>[] parsers
        )
        {
            throw new NotImplementedException();
        }

        static IParser<byte,InstructionParseState, InstructionParseState> LookFor(byte b)
        {
            return LookFor(new[] {b});
        }

        static IParser<byte, InstructionParseState, InstructionParseState> LookFor(byte[] bytes)
        {
            return new LookAheadWrapper<byte, InstructionParseState, InstructionParseState>(
                Consume(bytes),
                bytes
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> Consume(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate(ID(), (l, r) => l.Then(Consume(r)));
        }

        static IParser<byte, InstructionParseState, InstructionParseState> Consume(byte b)
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ID()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> SetOpcode(Opcode opcode, String pretty)
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseType()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseNestedInstruction()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseSkipFlags()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseByteOperand()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseBranchTarget32()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseBranchTarget8()
        {
            throw new NotImplementedException();
        }
    }
}
