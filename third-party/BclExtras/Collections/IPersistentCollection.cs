using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    /// <summary>
    /// Represtents a persistent or immutable collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPersistentCollection<T> : IReadOnlyCollection<T>, IEnumerable<T>
    {
        IPersistentCollection<T> Add(T value);
        IPersistentCollection<T> Remove(T value);
        IPersistentCollection<T> Clear();
    }
}
