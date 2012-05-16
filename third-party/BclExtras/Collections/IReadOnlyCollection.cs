using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    /// <summary>
    /// Represents a collection which cannot be modified
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        int Count { get; }
        bool Contains(T value);
    }
}
