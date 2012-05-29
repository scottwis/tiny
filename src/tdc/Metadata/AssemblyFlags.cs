namespace Tiny.Decompiler.Metadata
{
    enum AssemblyFlags : uint
    {
        PublicKey = 0x001,
        Retargetable = 0x0100,
        DisableJitcompilerOptimizer = 0x4000,
        EnableJitcompilerTracking = 0x8000
    }
}
