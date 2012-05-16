using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    /// <summary>
    /// Represents a list which cannot be modified
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        T this[int index] { get; }
        int IndexOf(T value);
    }
}
