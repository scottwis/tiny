using System;
using System.Text;
using System.Threading;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    public sealed class TypeReference : Type
    {
        readonly object m_lockObject;
        readonly IToken m_token;
        readonly Module m_module;
        volatile Type m_resolved;

        internal TypeReference(IToken token, Module module) : base(TypeKind.TypeReference)
        {
            m_token = token.CheckValid("token", x => x.Table.CanReferenceType(), "Token references invalid table");

            m_module = module.CheckNotNull("module");
            if (m_token.Table == MetadataTable.TypeRef) {
                m_lockObject = new object();
            }
        }

        public Type Resolve()
        {
            CheckDisposed();
            if (m_resolved == null) {
                if (m_token.Table == MetadataTable.TypeDef) {
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_resolved, m_module.GetType(m_token.Index), null);
                    #pragma warning restore 420
                }
                else if (m_token.Table == MetadataTable.TypeSpec) {
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_resolved, m_module.PEFile.ParseTypeSpec(m_token.Index, m_module), null);
                    #pragma warning restore 420
                }
                else {
                    lock (m_lockObject) {
                        if (m_resolved == null) {
                            m_resolved = m_module.PEFile.ResolveTypeRef(m_token.Index, m_module);
                        }
                    }
                }
            }
            return m_resolved;
        }

        internal override void GetFullName(StringBuilder b)
        {
            Resolve().GetFullName(b);
        }

        void CheckDisposed()
        {
            if (m_module.PEFile.IsDisposed) {
                throw new ObjectDisposedException("TypeReference");
            }
        }
    }
}
