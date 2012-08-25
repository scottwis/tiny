using System;
using System.Text;

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

        public InstructionParseState(int offset, MethodDefinition method) : this()
        {
            MethodDefinition = method;
            InstructionContext = new InstructionContext(offset);
        }
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
