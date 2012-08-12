using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Collections;

namespace Tiny.Metadata.Layout
{
    struct InstructionParseState
    {
        public MethodDefinition MethodDefinition;
        public InstructionContext InstructionContext;
        public Opcode Opcode;
        public Action<StringBuilder> PrettyPrint;
        public object Operand;
        public Instruction NestedInstruction;
    }

    class InstructionContext
    {
        public readonly int Offset;
        public int Size;

        public InstructionContext(int offset)
        {
            Offset = offset;
        }
    }
}
