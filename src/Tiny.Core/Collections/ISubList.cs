using System.Collections.Generic;

namespace Tiny.Collections
{
    interface ISubList<out T> : IReadOnlyList<T>
    {
        IReadOnlyList<T> Wrapped { get;  }
        int StartIndex { get; }
    }
}