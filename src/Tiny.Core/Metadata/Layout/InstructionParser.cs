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
using System.Text;
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
            LookFor(new byte[] {0xFE, 0x12}).Then(SetOpcode(Opcode.Unaligned, "unaligned")).Then(ParseByteAsInt()).Then(ParseNestedInstruction()),
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
            LookFor(0x32).Then(SetOpcode(Opcode.Blt, "blt.s")).Then(ParseBranchTarget8()),
            LookFor(0x40).Then(SetOpcode(Opcode.BltUn, "blt.un")).Then(ParseBranchTarget32()),
            LookFor(0x33).Then(SetOpcode(Opcode.BltUn, "blt.un.s")).Then(ParseBranchTarget8()),
            LookFor(0x38).Then(SetOpcode(Opcode.Br, "br")).Then(ParseBranchTarget32()),
            LookFor(0x2B).Then(SetOpcode(Opcode.Br, "br.s")).Then(ParseBranchTarget8()),
            LookFor(0x1).Then(SetOpcode(Opcode.Break, "break")),
            LookFor(0x39).Then(SetOpcode(Opcode.BrFalse, "brfalse")).Then(ParseBranchTarget32()),
            LookFor(0x2C).Then(SetOpcode(Opcode.BrFalse, "brfalse.s")).Then(ParseBranchTarget8()),
            LookFor(0x3A).Then(SetOpcode(Opcode.BrTrue, "brtrue")).Then(ParseBranchTarget32()),
            LookFor(0x2D).Then(SetOpcode(Opcode.BrTrue, "brtrue.s")).Then(ParseBranchTarget8()),
            LookFor(0x28).Then(SetOpcode(Opcode.Call, "call")).Then(ParseMethod()),
            LookFor(0x29).Then(SetOpcode(Opcode.CallI, "calli")).Then(ParseMethod()),
            LookFor(new byte[] {0xFE, 0x01}).Then(SetOpcode(Opcode.Ceq, "ceq")),
            LookFor(new byte[] {0xFE, 0x02}).Then(SetOpcode(Opcode.Cgt, "cgt")),
            LookFor(new byte[] {0xFE, 0x03}).Then(SetOpcode(Opcode.CgtUn, "cgt.un")),
            LookFor(0xC3).Then(SetOpcode(Opcode.CkFinite, "ckfinite")),
            LookFor(new byte[] {0xFe, 0x04}).Then(SetOpcode(Opcode.Clt, "clt")),
            LookFor(new byte[] {0xFe, 0x04}).Then(SetOpcode(Opcode.CltUn, "clt.un")),
            LookFor(0x67).Then(SetOpcode(Opcode.Conv, "conv.i1")).Then(SetOperand(CLRType.I1)),
            LookFor(0x68).Then(SetOpcode(Opcode.Conv, "conv.i2")).Then(SetOperand(CLRType.I2)),
            LookFor(0x69).Then(SetOpcode(Opcode.Conv, "conv.i4")).Then(SetOperand(CLRType.I4)),
            LookFor(0x6A).Then(SetOpcode(Opcode.Conv, "conv.i8")).Then(SetOperand(CLRType.I8)),
            LookFor(0x6B).Then(SetOpcode(Opcode.Conv, "conv.r4")).Then(SetOperand(CLRType.R4)),
            LookFor(0x6C).Then(SetOpcode(Opcode.Conv, "conv.r8")).Then(SetOperand(CLRType.R8)),
            LookFor(0xD2).Then(SetOpcode(Opcode.Conv, "conv.u1")).Then(SetOperand(CLRType.U1)),
            LookFor(0xD1).Then(SetOpcode(Opcode.Conv, "conv.u2")).Then(SetOperand(CLRType.U2)),
            LookFor(0x6D).Then(SetOpcode(Opcode.Conv, "conv.u4")).Then(SetOperand(CLRType.U4)),
            LookFor(0x6E).Then(SetOpcode(Opcode.Conv, "conv.u8")).Then(SetOperand(CLRType.U8)),
            LookFor(0xD3).Then(SetOpcode(Opcode.Conv, "conv.i")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0xE0).Then(SetOpcode(Opcode.Conv, "conv.u")).Then(SetOperand(CLRType.UnsignedNativeInteger)),
            LookFor(0x76).Then(SetOpcode(Opcode.Conv, "conv.r.un")).Then(SetOperand(CLRType.NativeFloat)),
            LookFor(0xB3).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.i1")).Then(SetOperand(CLRType.I1)),
            LookFor(0xB5).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.i2")).Then(SetOperand(CLRType.I2)),
            LookFor(0xB7).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.i4")).Then(SetOperand(CLRType.I4)),
            LookFor(0xB9).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.i8")).Then(SetOperand(CLRType.I8)),
            LookFor(0xB4).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.u1")).Then(SetOperand(CLRType.U1)),
            LookFor(0xB6).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.u2")).Then(SetOperand(CLRType.U2)),
            LookFor(0xB8).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.u4")).Then(SetOperand(CLRType.U4)),
            LookFor(0xBA).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.u8")).Then(SetOperand(CLRType.U8)),
            LookFor(0xD4).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.i")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0xD5).Then(SetOpcode(Opcode.ConvOvf, "conv.ovf.u")).Then(SetOperand(CLRType.UnsignedNativeInteger)),
            LookFor(0x82).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.i1.un")).Then(SetOperand(CLRType.I1)),
            LookFor(0x83).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.i2.un")).Then(SetOperand(CLRType.I2)),
            LookFor(0x84).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.i4.un")).Then(SetOperand(CLRType.I4)),
            LookFor(0x85).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.i8.un")).Then(SetOperand(CLRType.I8)),
            LookFor(0x86).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.u1.un")).Then(SetOperand(CLRType.U1)),
            LookFor(0x87).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.u2.un")).Then(SetOperand(CLRType.U2)),
            LookFor(0x88).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.u4.un")).Then(SetOperand(CLRType.U4)),
            LookFor(0x89).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.u8.un")).Then(SetOperand(CLRType.U8)),
            LookFor(0x8A).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.i.un")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0x8B).Then(SetOpcode(Opcode.ConvOvfUn, "conv.ovf.u.un")).Then(SetOperand(CLRType.UnsignedNativeInteger)),
            LookFor(new byte[] {0xFE, 0x17}).Then(SetOpcode(Opcode.CpBlk, "cpblk")),
            LookFor(0x5B).Then(SetOpcode(Opcode.Div, "div")),
            LookFor(0x5C).Then(SetOpcode(Opcode.DivUn, "div.un")),
            LookFor(0x5C).Then(SetOpcode(Opcode.Dup, "dup")),
            LookFor(new byte[] {0xFE, 0x11}).Then(SetOpcode(Opcode.EndFilter, "endfilter")),
            LookFor(0xDC).Then(SetOpcode(Opcode.EndFinally, "endfinally")),
            LookFor(new byte[] {0xFE, 0x18}).Then(SetOpcode(Opcode.InitBlk, "initblk")),
            LookFor(0x27).Then(SetOpcode(Opcode.Jmp, "jmp")).Then(ParseMethod()),
            LookFor(new byte[] {0xFE, 0x09}).Then(SetOpcode(Opcode.LdArg, "ldarg")).Then(ParseUShortAsInt()),
            LookFor(0x0E).Then(SetOpcode(Opcode.LdArg, "ldarg.s")).Then(ParseByteAsInt()),
            LookFor(0x02).Then(SetOpcode(Opcode.LdArg, "ldarg.0")).Then(SetOperand(0)),
            LookFor(0x03).Then(SetOpcode(Opcode.LdArg, "ldarg.1")).Then(SetOperand(1)),
            LookFor(0x04).Then(SetOpcode(Opcode.LdArg, "ldarg.2")).Then(SetOperand(2)),
            LookFor(0x05).Then(SetOpcode(Opcode.LdArg, "ldarg.3")).Then(SetOperand(3)),
            LookFor(new byte[] {0xFE, 0xA}).Then(SetOpcode(Opcode.LdArgA, "ldarga")).Then(ParseUShortAsInt()),
            LookFor(new byte[] {0x0F}).Then(SetOpcode(Opcode.LdArgA, "ldarga.s")).Then(ParseByteAsInt()),
            LookFor(0x20).Then(SetOpcode(Opcode.Ldc, "ldc.i4")).Then(ParseIntOperand()),
            LookFor(0x21).Then(SetOpcode(Opcode.Ldc, "ldc.i8")).Then(ParseLongOperand()),
            LookFor(0x22).Then(SetOpcode(Opcode.Ldc, "ldc.r4")).Then(ParseSingleOperand()),
            LookFor(0x23).Then(SetOpcode(Opcode.Ldc, "ldc.r8")).Then(ParseDoubleOperand()),
            LookFor(0x16).Then(SetOpcode(Opcode.Ldc, "ldc.i4.0")).Then(SetOperand(0)),
            LookFor(0x17).Then(SetOpcode(Opcode.Ldc, "ldc.i4.1")).Then(SetOperand(1)),
            LookFor(0x18).Then(SetOpcode(Opcode.Ldc, "ldc.i4.2")).Then(SetOperand(2)),
            LookFor(0x19).Then(SetOpcode(Opcode.Ldc, "ldc.i4.3")).Then(SetOperand(3)),
            LookFor(0x1A).Then(SetOpcode(Opcode.Ldc, "ldc.i4.4")).Then(SetOperand(4)),
            LookFor(0x1B).Then(SetOpcode(Opcode.Ldc, "ldc.i4.5")).Then(SetOperand(5)),
            LookFor(0x1C).Then(SetOpcode(Opcode.Ldc, "ldc.i4.6")).Then(SetOperand(6)),
            LookFor(0x1D).Then(SetOpcode(Opcode.Ldc, "ldc.i4.7")).Then(SetOperand(7)),
            LookFor(0x1E).Then(SetOpcode(Opcode.Ldc, "ldc.i4.8")).Then(SetOperand(8)),
            LookFor(0x15).Then(SetOpcode(Opcode.Ldc, "ldc.i4.m1")).Then(SetOperand(-1)),
            LookFor(0x1F).Then(SetOpcode(Opcode.Ldc, "ldc.i4.s")).Then(ParseSByteAsInt()),
            LookFor(new byte[] {0xFE,0x06}).Then(SetOpcode(Opcode.LdFtn,"ldftn")).Then(ParseByteAsInt()),
            LookFor(0x46).Then(SetOpcode(Opcode.LdInd, "ldind.i1")).Then(SetOperand(CLRType.I1)),
            LookFor(0x48).Then(SetOpcode(Opcode.LdInd, "ldind.i2")).Then(SetOperand(CLRType.I2)),
            LookFor(0x4A).Then(SetOpcode(Opcode.LdInd, "ldind.i4")).Then(SetOperand(CLRType.I4)),
            LookFor(0x4C).Then(SetOpcode(Opcode.LdInd, "ldind.i8")).Then(SetOperand(CLRType.I8)),
            LookFor(0x47).Then(SetOpcode(Opcode.LdInd, "ldind.u1")).Then(SetOperand(CLRType.U1)),
            LookFor(0x49).Then(SetOpcode(Opcode.LdInd, "ldind.u2")).Then(SetOperand(CLRType.U2)),
            LookFor(0x4B).Then(SetOpcode(Opcode.LdInd, "ldind.u4")).Then(SetOperand(CLRType.U4)),
            LookFor(0x4E).Then(SetOpcode(Opcode.LdInd, "ldind.r4")).Then(SetOperand(CLRType.R4)),
            LookFor(0x4C).Then(SetOpcode(Opcode.LdInd, "ldind.u8")).Then(SetOperand(CLRType.U8)),
            LookFor(0x4F).Then(SetOpcode(Opcode.LdInd, "ldind.r8")).Then(SetOperand(CLRType.R8)),
            LookFor(0x4D).Then(SetOpcode(Opcode.LdInd, "ldind.i")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0x50).Then(SetOpcode(Opcode.LdInd, "ldind.ref")).Then(SetOperand(CLRType.Object)),
            LookFor(new byte[] {0xFE, 0x0C}).Then(SetOpcode(Opcode.LdLoc,"ldloc")).Then(ParseUShortAsInt()),
            LookFor(0x11).Then(SetOpcode(Opcode.LdLoc, "ldloc.s")).Then(ParseByteAsInt()),
            LookFor(0x06).Then(SetOpcode(Opcode.LdLoc,"ldloc.0")).Then(SetOperand(0)),
            LookFor(0x07).Then(SetOpcode(Opcode.LdLoc,"ldloc.1")).Then(SetOperand(1)),
            LookFor(0x08).Then(SetOpcode(Opcode.LdLoc, "ldloc.2")).Then(SetOperand(2)),
            LookFor(0x09).Then(SetOpcode(Opcode.LdLoc, "ldloc.3")).Then(SetOperand(3)),
            LookFor(new byte[] {0xFE,0x0D}).Then(SetOpcode(Opcode.LdLocA,"ldloca")).Then(ParseUShortAsInt()),
            LookFor(0x12).Then(SetOpcode(Opcode.LdLocA, "ldloca.s")).Then(ParseByteAsInt()),
            LookFor(0x14).Then(SetOpcode(Opcode.LdNull,"ldnull")),
            LookFor(0xDD).Then(SetOpcode(Opcode.Leave, "leave")).Then(ParseBranchTarget32()),
            LookFor(0xDE).Then(SetOpcode(Opcode.Leave, "leave.s")).Then(ParseBranchTarget8()),
            LookFor(new byte[] {0xFE,0x0F}).Then(SetOpcode(Opcode.LocAlloc, "localloc")),
            LookFor(0x5A).Then(SetOpcode(Opcode.Mul,"mul")),
            LookFor(0xD8).Then(SetOpcode(Opcode.MulOvf, "mul.ovf")),
            LookFor(0xD9).Then(SetOpcode(Opcode.MulOvfUn, "mul.ovf.un")),
            LookFor(0x65).Then(SetOpcode(Opcode.Neg,"neg")),
            LookFor(0x00).Then(SetOpcode(Opcode.Nop,"nop")),
            LookFor(0x66).Then(SetOpcode(Opcode.Not,"not")),
            LookFor(0x60).Then(SetOpcode(Opcode.Or, "or")),
            LookFor(0x26).Then(SetOpcode(Opcode.Pop, "pop")),
            LookFor(0x5D).Then(SetOpcode(Opcode.Rem, "rem")),
            LookFor(0x5E).Then(SetOpcode(Opcode.RemUn, "rem.un")),
            LookFor(0x2A).Then(SetOpcode(Opcode.Ret, "ret")),
            LookFor(0x62).Then(SetOpcode(Opcode.Shl, "shl")),
            LookFor(0x63).Then(SetOpcode(Opcode.Shr, "shr")),
            LookFor(0x64).Then(SetOpcode(Opcode.ShrUn, "shr.un")),
            LookFor(new byte[] {0xFE,0x0B}).Then(SetOpcode(Opcode.StArg, "starg")).Then(ParseUShortAsInt()),
            LookFor(0x10).Then(SetOpcode(Opcode.StArg, "starg.s")).Then(ParseByteAsInt()),
            LookFor(0x52).Then(SetOpcode(Opcode.StInd, "stind.i1")).Then(SetOperand(CLRType.I1)),
            LookFor(0x53).Then(SetOpcode(Opcode.StInd, "stind.i2")).Then(SetOperand(CLRType.I2)),
            LookFor(0x54).Then(SetOpcode(Opcode.StInd, "stind.i4")).Then(SetOperand(CLRType.I4)),
            LookFor(0x55).Then(SetOpcode(Opcode.StInd, "stind.i8")).Then(SetOperand(CLRType.I8)),
            LookFor(0x56).Then(SetOpcode(Opcode.StInd, "stind.r4")).Then(SetOperand(CLRType.R4)),
            LookFor(0x57).Then(SetOpcode(Opcode.StInd, "stind.r8")).Then(SetOperand(CLRType.R8)),
            LookFor(0xDF).Then(SetOpcode(Opcode.StInd, "stind.i")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0x51).Then(SetOpcode(Opcode.StInd, "stind.ref")).Then(SetOperand(CLRType.Object)),
            LookFor(new byte[] {0xFE, 0x0E}).Then(SetOpcode(Opcode.StInd, "stloc")).Then(ParseUShortAsInt()),
            LookFor(0x13).Then(SetOpcode(Opcode.StLoc, "stloc.c")).Then(ParseByteAsInt()),
            LookFor(0x0A).Then(SetOpcode(Opcode.StLoc, "stloc.0")).Then(SetOperand(0)),
            LookFor(0x0B).Then(SetOpcode(Opcode.StLoc, "stloc.1")).Then(SetOperand(1)),
            LookFor(0x0C).Then(SetOpcode(Opcode.StLoc, "stloc.2")).Then(SetOperand(2)),
            LookFor(0x0D).Then(SetOpcode(Opcode.StLoc, "stloc.3")).Then(SetOperand(3)),
            LookFor(0x59).Then(SetOpcode(Opcode.Sub, "sub")),
            LookFor(0xDA).Then(SetOpcode(Opcode.Sub, "sub.ovf")),
            LookFor(0xDB).Then(SetOpcode(Opcode.Sub, "sub.ovf.un")),
            LookFor(0x45).Then(SetOpcode(Opcode.Switch, "switch")).Then(ParseSwitchTable()),
            LookFor(0x61).Then(SetOpcode(Opcode.Xor, "xor")),
            LookFor(0x8C).Then(SetOpcode(Opcode.Box, "box")).Then(ParseType()),
            LookFor(0x6F).Then(SetOpcode(Opcode.CallVirt, "callvirt")).Then(ParseMethod()),
            LookFor(0x74).Then(SetOpcode(Opcode.CastClass, "castclass")).Then(ParseType()),
            LookFor(0x70).Then(SetOpcode(Opcode.CpObj, "cpbj")).Then(ParseType()),
            LookFor(new byte[] {0xFE, 0x15}).Then(SetOpcode(Opcode.InitObj, "initobj")).Then(ParseType()),
            LookFor(0xA3).Then(SetOpcode(Opcode.LdElem, "ldelem")).Then(ParseType()),
            LookFor(0x90).Then(SetOpcode(Opcode.LdElem, "ldelem.i1")).Then(SetOperand(CLRType.I1)),
            LookFor(0x92).Then(SetOpcode(Opcode.LdElem, "ldelem.i2")).Then(SetOperand(CLRType.I2)),
            LookFor(0x94).Then(SetOpcode(Opcode.LdElem, "ldelem.i4")).Then(SetOperand(CLRType.I4)),
            LookFor(0x96).Then(SetOpcode(Opcode.LdElem, "ldelem.i8")).Then(SetOperand(CLRType.I8)),
            LookFor(0x91).Then(SetOpcode(Opcode.LdElem, "ldelem.u1")).Then(SetOperand(CLRType.U1)),
            LookFor(0x93).Then(SetOpcode(Opcode.LdElem, "ldelem.u2")).Then(SetOperand(CLRType.U2)),
            LookFor(0x95).Then(SetOpcode(Opcode.LdElem, "ldelem.u4")).Then(SetOperand(CLRType.U4)),
            LookFor(0x98).Then(SetOpcode(Opcode.LdElem, "ldelem.r4")).Then(SetOperand(CLRType.R4)),
            LookFor(0x99).Then(SetOpcode(Opcode.LdElem, "ldelem.r8")).Then(SetOperand(CLRType.R8)),
            LookFor(0x97).Then(SetOpcode(Opcode.LdElem, "ldelem.i")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0x9A).Then(SetOpcode(Opcode.LdElem, "ldelem.ref")).Then(SetOperand(CLRType.Object)),
            LookFor(0x8F).Then(SetOpcode(Opcode.LdElemA, "ldelema")).Then(ParseType()),
            LookFor(0x7B).Then(SetOpcode(Opcode.LdFld, "ldfld")).Then(ParseField()),
            LookFor(0x7C).Then(SetOpcode(Opcode.LdFldA, "ldflda")).Then(ParseField()),
            LookFor(0x8E).Then(SetOpcode(Opcode.LdLen, "ldlen")),
            LookFor(0x71).Then(SetOpcode(Opcode.LdObj, "ldobj")).Then(ParseType()),
            LookFor(0x7E).Then(SetOpcode(Opcode.LdSFld, "ldsfld")).Then(ParseField()),
            LookFor(0x7F).Then(SetOpcode(Opcode.LdSFldA, "ldsflda")).Then(ParseField()),
            LookFor(0x72).Then(SetOpcode(Opcode.LdStr, "ldstr")).Then(ParseStringLiteral()),
            LookFor(0xD0).Then(SetOpcode(Opcode.LdToken, "ldtoken")).Then(ParseToken()),
            LookFor(new byte[] {0xFE, 0x07}).Then(SetOpcode(Opcode.LdVirtFtn, "ldvirtftn")).Then(ParseMethod()),
            LookFor(0xC6).Then(SetOpcode(Opcode.MkRefAny, "mkrefany")).Then(ParseType()),
            LookFor(0x8D).Then(SetOpcode(Opcode.NewArr, "mkrefany")).Then(ParseType()),
            LookFor(0x73).Then(SetOpcode(Opcode.NewObj, "newobj")).Then(ParseMethod()),
            LookFor(new byte[] {0xFE, 0x1D}).Then(SetOpcode(Opcode.RefAnyType, "refanytype")),
            LookFor(0xC2).Then(SetOpcode(Opcode.RefAnyVal, "refanyval")).Then(ParseType()),
            LookFor(new byte[] {0xFe, 0x1A}).Then(SetOpcode(Opcode.Rethrow, "rethrow")),
            LookFor(new byte[] {0xFe, 0x1C}).Then(SetOpcode(Opcode.SizeOf, "sizeof")).Then(ParseType()),
            LookFor(0xA4).Then(SetOpcode(Opcode.StElem, "stelem")).Then(ParseType()),
            LookFor(0x9C).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.I1)),
            LookFor(0x9D).Then(SetOpcode(Opcode.StElem, "stelem.i2")).Then(SetOperand(CLRType.I2)),
            LookFor(0x9E).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.I4)),
            LookFor(0x9F).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.I8)),
            LookFor(0xA0).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.R4)),
            LookFor(0xA1).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.R8)),
            LookFor(0x9B).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.NativeInteger)),
            LookFor(0xA2).Then(SetOpcode(Opcode.StElem, "stelem.i1")).Then(SetOperand(CLRType.Object)),
            LookFor(0x7D).Then(SetOpcode(Opcode.StFld, "stfld")).Then(ParseField()),
            LookFor(0x81).Then(SetOpcode(Opcode.StObj, "stobj")).Then(ParseType()),
            LookFor(0x80).Then(SetOpcode(Opcode.StsFld, "stsfld")).Then(ParseField()),
            LookFor(0x7A).Then(SetOpcode(Opcode.Throw, "throw")),
            LookFor(0x79).Then(SetOpcode(Opcode.Unbox, "unbox")).Then(ParseType()),
            LookFor(0xA5).Then(SetOpcode(Opcode.UnboxAny, "unbox.any")).Then(ParseType())
        );

        static IParser<byte, InstructionParseState, Instruction> Grammar(
            params IParser<byte, InstructionParseState, InstructionParseState>[] parsers
        )
        {
            var tableParser = CreateTableParser(parsers);
            return new FunctionalParser<byte, InstructionParseState, Instruction>(
                startState=> {
                    var endState = tableParser.Parse(startState);
                    if (endState == null) {
                        return null;
                    }
                    return ParseState.Create(
                        endState.ConsumedInput,
                        endState.RemainingInput,
                        CreateInstruction(endState.Result)
                    );
                },
                tableParser.LookAhead
            );
        }

        static IParser<byte,InstructionParseState, InstructionParseState> LookFor(byte b)
        {
            return Consume(b);
        }

        static IParser<byte, InstructionParseState, InstructionParseState> LookFor(byte[] bytes)
        {
            return new LookAheadWrapper<byte, InstructionParseState, InstructionParseState>(bytes);
        }

        static IParser<byte, InstructionParseState, InstructionParseState> CreateTableParser(
            IEnumerable<IParser<byte, InstructionParseState, InstructionParseState>> parsers
        )
        {
            var q = (from p in parsers group p by p.LookAhead).Select(x => {
                if (x.Count() > 1) {
                    return Consume(x.Key).Then(CreateTableParser(x.Select(y =>
                    {
                        var w = x.AssumeIs<LookAheadWrapper<byte, InstructionParseState, InstructionParseState>>();
                        return w.SubList(1);
                    })));
                }
                else {
                    return x.First();
                }
            });

            return TableParser.Create((byte) 0, q);
        }

        static IParser<byte, InstructionParseState, InstructionParseState> Consume(byte b)
        {
            return FunctionalParser.Create(
                (IParseState<byte, InstructionParseState> input)=> {
                    if (input == null || input.RemainingInput.Count == 0 || input.RemainingInput[0] != b) {
                        return null;
                    }
                    return ParseState.Create(input.ConsumedInput.Expand(1), input.RemainingInput.SubList(1), input.Result);
                },
                b
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ID()
        {
            return FunctionalParser.Create(
                (IParseState<byte, InstructionParseState> input) => input,
                (byte)0
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> SetOpcode(Opcode opcode, String pretty)
        {
            return FunctionalParser.Create(
                (IParseState<byte, InstructionParseState> input) => {
                    if (input == null) {
                        return null;
                    }
                    var result = input.Result;
                    result.Opcode = opcode;
                    result.PrettyPrint = builder => builder.Append(pretty);
                    return ParseState.Create(input.ConsumedInput, input.RemainingInput, result);
                },
                (byte)0
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> SetOperand(Object operand)
        {
            return FunctionalParser.Create(
                (IParseState<byte, InstructionParseState> input) => {
                    if (input == null) {
                        return null;
                    }
                    var result = input.Result;
                    result.Operand = operand;
                    return ParseState.Create(input.ConsumedInput, input.RemainingInput, result);
                },
                (byte)0
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseToken<TOperand>(
            Func<MetadataToken, InstructionParseState, TOperand> factory, 
            Action<TOperand, StringBuilder> prettyPrinter
        )
        {
            return FunctionalParser.Create(
                (IParseState<byte, InstructionParseState> input) =>{
                    if (input == null || input.RemainingInput.Count < 4) {
                        return null;
                    }
                    var result = input.Result;
                    var operand = factory(
                        new MetadataToken(new OneBasedIndex(
                            input.RemainingInput.ReadLittleEndianUInt()
                        )),
                        input.Result
                    );
                    result.Operand = factory;
                    result.PrettyPrint = result.PrettyPrint.Then(builder =>builder.Append(" ")).Then(builder=>prettyPrinter(operand, builder));
                    return ParseState.Create(input.ConsumedInput.Expand(4), input.RemainingInput.SubList(4), result);
                },
                (byte)0
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseField()
        {
            return ParseToken((t, i) => new FieldReference(t), (f, b) => f.GetFullName(b));
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseType()
        {
            return ParseToken((t, i) => new TypeReference(t, i.Module), (t, b) => t.GetFullName(b));
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseMethod()
        {
            return ParseToken((t, i) => new MethodReference(t), (m, b) => m.GetFullName(b));
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseNestedInstruction()
        {
            return FunctionalParser.Create(
                (IParseState<byte, InstructionParseState> input)=> {
                    var output = s_parser.Parse(input);
                    var result = input.Result;
                    result.NestedInstruction = output.Result;
                    result.PrettyPrint = result.PrettyPrint.Then(b => b.Append(" ")).Then(
                        b => output.Result.ToString(b)
                    );
                    return ParseState.Create(output.ConsumedInput, output.RemainingInput, result);
                },
                (byte)0
            );
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseSkipFlags()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseByteAsInt()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseSByteAsInt()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseUShortAsInt()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseIntOperand()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseLongOperand()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseSingleOperand()
        {
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseDoubleOperand()
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

        static IParser<byte, InstructionParseState, InstructionParseState> ParseSwitchTable()
        {
            throw new NotImplementedException();
        }


        static IParser<byte, InstructionParseState, InstructionParseState> ParseStringLiteral()
        {
            //TODO: Parse an offset into the string heap, and read the data into a string.
            throw new NotImplementedException();
        }

        static IParser<byte, InstructionParseState, InstructionParseState> ParseToken()
        {
            //TODO: Parse an offset into the string heap, and read the data into a string.
            throw new NotImplementedException();
        }

        static Instruction CreateInstruction(InstructionParseState state)
        {
            throw new NotImplementedException();
        }
    }
}
