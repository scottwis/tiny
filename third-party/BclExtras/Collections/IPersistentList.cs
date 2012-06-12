using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    /// <summary>
    /// Represents a persistent or immutable list which is indexable
    /// </summary>
    public interface IPersistentList<T> : IReadOnlyListEx<T>
    {
        IPersistentList<T> Insert(int index, T value);
        IPersistentList<T> RemoveAt(int index);
        IPersistentList<T> Clear();
        IPersistentList<T> Add(T value);
        IPersistentList<T> Remove(T value);
    }
}
