// Opcode.cs
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

namespace Tiny.Metadata
{
    //# An enum defining opcodes for MSIL instructions. The opcodes presented here are normalized,
    //# so that functionally equivalent encodings (ldarg.0 vs ldarg 0, beq vs beq.s, etc) are represented by a single
    //# opcode (ldarg, beq). To determine the particular encoding used by an instruction see [Instruction.Encoding]. To
    //# acquire a human readable representation of an instruction (including it's encoding), call
    //# [Instruction.ToString()].
    //
    //NOTE: Normalizing the opcodes does make me a little nervous. My plan is to re-use the Tiny.Core code to
    //implement reflection, with the System.Reflection.* types implemented as simple wrappers. However, the
    //System.Reflection.Emit code uses non-normalized opcodes. That will slightly complicate the wrapper used for
    //instructions. However, I have some experience using Mono.Cecil (which uses non-normalized opcodes) and it
    //can be somewhat cumbersome to use. for example, when implementing data flow over a CFG with instructions,
    //transfer functions need to switch on many similar opcodes. I'm hoping that, on balance, the tradeoffs will favor
    //going with normalized opcodes. The Instruction class is defined to still preserve information about the underlying
    //encoding used.
    public enum Opcode
    {
        Add,
        AddOvf,
        AddOvfUn,
        And,
        ArgList,
        Beq,
        Bge,
        BgeUn,
        Bgt,
        BgtUn,
        Ble,
        BleUn,
        Blt,
        BltUn,
        Bne,
        Box,
        Br,
        Break,
        BrFalse,
        BrTrue,
        Call,
        CallI,
        CallVirt,
        CastClass,
        Ceq,
        Cgt,
        CgtUn,
        CkFinite,
        Clt,
        CltUn,
        Constrained, 
        Conv,
        ConvOvf,
        ConvOvfUn,
        CpBlk,
        CpObj,
        Div,
        DivUn,
        Dup,
        EndFilter,
        EndFinally,
        InitBlk,
        InitObj,
        IsInst,
        Jmp,
        LdArg,
        LdArgA,
        Ldc,
        LdElem,
        LdElemA,
        LdFld,
        LdFldA,
        LdFtn,
        LdInd,
        LdLen,
        LdLoc,
        LdLocA,
        LdNull,
        LdObj,
        LdSFld,
        LdSFldA,
        LdStr,
        LdToken,
        LdVirtFtn,
        Leave,
        LocAlloc,
        MkRefAny,
        Mul,
        MulOvf,
        MulOvfUn,
        Neg,
        NewArr,
        NewObj,
        No,
        Nop,
        Not,
        Or,
        Pop,
        Readonly,
        RefAnyType,
        RefAnyVal,
        Rem,
        RemUn,
        Ret,
        Rethrow,
        Shl,
        Shr,
        ShrUn,
        SizeOf,
        StArg,
        StElem,
        StFld,
        StInd,
        StLoc,
        StObj,
        StsFld,
        Sub,
        SubOvf,
        SubOvfUn,
        Switch,
        Tail,
        Throw,
        Unaligned, 
        Unbox,
        UnboxAny,
        Volatile,
        Xor,
    }
}
