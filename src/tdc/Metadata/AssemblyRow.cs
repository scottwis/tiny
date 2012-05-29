using System.Configuration.Assemblies;
using System.Runtime.InteropServices;

namespace Tiny.Decompiler.Metadata
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct AssemblyRow
    {
        public AssemblyHashAlgorithm HashAlgorithm
        {
            get
            {
                fixed(AssemblyRow * pThis = &this) {
                    return (AssemblyHashAlgorithm) (*((uint*) pThis));
                }
            }
        }

        [FieldOffset(4)]
        public readonly ushort MajorVersion;

        [FieldOffset(6)]
        public readonly ushort MinorVersion;

        [FieldOffset(8)]
        public readonly ushort BuildNumber;

        [FieldOffset(10)]
        public readonly ushort RevisionNumber;

        [FieldOffset(12)]
        public readonly AssemblyFlags Flags;

        public uint GetPublicKey(PEFile peFile)
        {
            fixed (AssemblyRow * pThis = & this) {
                byte* pPublicKey = (byte*) pThis + 16;

                if (peFile.GetHeapIndexSize(StreamID.Blob) == 2) {
                    return *(ushort*) pPublicKey;
                }
                return *(uint*) pPublicKey;
            }
        }
    }
}
