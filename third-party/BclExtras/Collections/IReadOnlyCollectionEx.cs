using System.Collections.Generic;

namespace BclExtras.Collections
{
    /// <summary>
    /// Represents a collection which cannot be modified
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyCollectionEx<T> : IReadOnlyCollection<T>
    {
        bool Contains(T value);
    }
}
