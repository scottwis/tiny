using System;
using System.Collections;
using System.Collections.Generic;

namespace Tiny.Metadata.Layout
{
    struct TableWrapper : IReadOnlyList<IntPtr>
    {
        readonly MetadataTable m_table;
        readonly PEFile m_peFile;

        public TableWrapper(MetadataTable table, PEFile peFile)
        {
            m_table = table.CheckDefined("table");
            m_peFile = peFile.CheckDefined("peFile");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IntPtr> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i) {
                yield return this[i];
            }
        }

        public int Count
        {
            get { return m_table.RowCount(m_peFile); }
        }

        public unsafe IntPtr this[int index]
        {
            get { return (IntPtr)m_table.GetRow(index.ToZB(), m_peFile); }
        }
    }
}
