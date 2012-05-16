using System;
using System.Collections.Generic;
using System.Diagnostics;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    internal sealed class DebuggerEnumerableView<T>
    {
        private IEnumerable<T> m_enumerable;

        public DebuggerEnumerableView(IEnumerable<T> enumerable)
        {
            m_enumerable = enumerable;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return m_enumerable.ToArray(); }
        }
    }
}
