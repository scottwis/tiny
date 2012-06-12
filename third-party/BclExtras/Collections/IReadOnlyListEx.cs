using System.Collections.Generic;

namespace BclExtras.Collections
{
    /// <summary>
    /// Represents a list which cannot be modified
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyListEx<T> : IReadOnlyCollectionEx<T>, IReadOnlyList<T>
    {
        int IndexOf(T value);
    }
}
