using System;
using System.Text;

namespace Tiny.Metadata.Layout
{
    struct InstructionParseState
    {
        public Module Module;
        public Opcode Opcode;
        public Action<StringBuilder> PrettyPrint;
        public object Operand;
        public Instruction NestedInstruction;
    }
}
