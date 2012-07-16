using System;

namespace Tiny.Metadata.Layout
{
    struct MethodDefOrRef : IToken
    {
        readonly OneBasedIndex m_index;

        public MethodDefOrRef(OneBasedIndex index)
        {
            m_index = index;
        }

        public bool IsNull
        {
            get { return ((m_index & ~0x1u) >> 1) == 0; }
        }

        public MetadataTable Table
        {
            get 
            { 
                CheckNull();
                switch ((m_index & 0x1).Value) {
                    case 0:
                        return MetadataTable.MethodDef;
                    case 1:
                        return MetadataTable.MemberRef;
                    default:
                        throw new InvalidOperationException("Invalid metadata table.");
                }
            }
        }

        public ZeroBasedIndex Index
        {
            get 
            { 
                CheckNull();
                return (ZeroBasedIndex) ((m_index & ~0x1u) >> 1);
            }
        }

        void CheckNull()
        {
            if (IsNull) {
                throw new InvalidOperationException("The token is null.");
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IToken);
        }

        public override int GetHashCode()
        {
            if (IsNull) {
                return 0.GetHashCode();
            }
            return m_index.GetHashCode();
        }

        public bool Equals(IToken token)
        {
            if (token == null) {
                return false;
            }
            if (IsNull) {
                return token.IsNull;
            }
            return !token.IsNull && Table == token.Table && Index == token.Index;
        }

        public int CompareTo(IToken other)
        {
            if (other == null) {
                return 1;
            }

            if (IsNull) {
                return other.IsNull ? 0 : -1;
            }

            if (other.IsNull) {
                return 1;
            }

            var ret = Index.CompareTo(other.Index);
            if (ret == 0) {
                ret = Table.CompareTo(other.Table);
            }
            return ret;
        }
    }
}
