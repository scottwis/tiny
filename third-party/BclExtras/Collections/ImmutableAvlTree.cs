using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    public enum InsertType
    {
        Add,
        Update
    }

    /// <summary>
    /// Immutable balancing tree class.  
    /// 
    /// Heavily influenced by Eric Lippert's blog postings on immutable collections
    /// http://blogs.msdn.com/ericlippert
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [SuppressMessage("Microsoft.Naming", "CA1710")]
    [Immutable]
    [Serializable]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public class ImmutableAvlTree<TKey, TValue> : IPersistentMap<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        #region Empty

        [Serializable]
        private sealed class EmptyAvlTree : ImmutableAvlTree<TKey, TValue>
        {
            public override ImmutableAvlTree<TKey, TValue> Left
            {
                get { throw CreateEmptyException(); }
            }

            public override ImmutableAvlTree<TKey, TValue> Right
            {
                get { throw CreateEmptyException(); }
            }

            protected override ImmutableAvlTree<TKey, TValue> AddCore(TKey key, TValue value, InsertType type, out InsertType resultType)
            {
                resultType = InsertType.Add;
                return ImmutableAvlTree.Create(key, value);
            }

            protected override ImmutableAvlTree<TKey, TValue> DeleteCore(TKey key)
            {
                throw new InvalidOperationException("Key does not exist");
            }

            protected override bool TryFindCore(TKey key, out TValue value)
            {
                value = default(TValue);
                return false;
            }

            protected override int CalculateBalanceFactor()
            {
                return 0;
            }

            private static InvalidOperationException CreateEmptyException()
            {
                return new InvalidOperationException("Tree is Empty");
            }

            public override string ToString()
            {
                return "Empty";
            }
        }

        private static readonly ImmutableAvlTree<TKey, TValue> s_empty = new EmptyAvlTree();

        public static ImmutableAvlTree<TKey, TValue> Empty
        {
            get { return s_empty; }
        }

        #endregion

        private readonly TKey m_key;
        private readonly TValue m_value;
        private readonly int m_height;
        private readonly int m_count;
        private readonly ImmutableAvlTree<TKey, TValue> m_left;
        private readonly ImmutableAvlTree<TKey, TValue> m_right;

        public TKey Key
        {
            get { return m_key; }
        }

        public TValue Value
        {
            get { return m_value; }
        }

        public int Height
        {
            get { return m_height; }
        }

        public bool IsLeaf
        {
            get { return 1 == m_height; }
        }

        public bool IsEmpty
        {
            get { return 0 == m_height; }
        }

        public virtual ImmutableAvlTree<TKey, TValue> Left
        {
            get { return m_left; }
        }

        public virtual ImmutableAvlTree<TKey, TValue> Right
        {
            get { return m_right; }
        }

        public IEnumerable<TKey> Keys
        {
            get { return this.Select((x) => x.First); }
        }

        public IEnumerable<TValue> Values
        {
            get { return this.Select((x) => x.Second); }
        }

        public int Count
        {
            get { return m_count; }
        }

        public ImmutableAvlTree(TKey key, TValue value)
            : this(key, value, Empty, Empty)
        {
        }

        private ImmutableAvlTree(TKey key, TValue value, ImmutableAvlTree<TKey, TValue> left, ImmutableAvlTree<TKey, TValue> right)
        {
            m_key = key;
            m_value = value;
            m_left = left ?? Empty;
            m_right = right ?? Empty;
            m_height = Math.Max(m_left.Height, m_right.Height) + 1;
            m_count = m_left.Count + m_right.Count + 1;
        }

        /// <summary>
        /// Adds a value into the tree.  Throws if the key is already in the tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ImmutableAvlTree<TKey, TValue> Add(TKey key, TValue value)
        {
            InsertType result;
            return AddCore(key, value, InsertType.Add, out result);
        }

        /// <summary>
        /// Updates or adds a value into the tree.  If there is already a value in the tree
        /// with the key, it will be replaced with the new value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ImmutableAvlTree<TKey, TValue> Update(TKey key, TValue value)
        {
            InsertType result;
            return Update(key, value, out result);
        }

        public ImmutableAvlTree<TKey, TValue> Update(TKey key, TValue value, out InsertType resultType)
        {
            return AddCore(key, value, InsertType.Update, out resultType);
        }

        protected ImmutableAvlTree()
        {
        }

        public ImmutableAvlTree<TKey, TValue> Delete(TKey key)
        {
            return DeleteCore(key);
        }

        public Option<TValue> Find(TKey key)
        {
            TValue value;
            if (TryFind(key, out value))
            {
                return Option.Create(value);
            }
            return Option.Empty;
        }

        public bool TryFind(TKey key, out TValue value)
        {
            return TryFindCore(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            TValue value;
            return TryFind(key, out value);
        }

        #region Protected

        protected virtual ImmutableAvlTree<TKey, TValue> AddCore(TKey key, TValue value, InsertType type, out InsertType resultType)
        {
            ImmutableAvlTree<TKey, TValue> newTree = null;
            var comp = key.CompareTo(m_key);
            if (comp < 0)
            {
                newTree = CreateNew(m_left.AddCore(key, value, type, out resultType), m_right);
            }
            else if (comp > 0)
            {
                newTree = CreateNew(m_left, m_right.AddCore(key, value, type, out resultType));
            }
            else
            {
                if (type == InsertType.Add)
                {
                    throw new InvalidOperationException("Key already exists in the tree");
                }

                // Update the value
                newTree = new ImmutableAvlTree<TKey, TValue>(key, value, Left, Right);
                resultType = InsertType.Update;
            }

            return newTree.EnsureBalanced();
        }

        protected virtual ImmutableAvlTree<TKey, TValue> DeleteCore(TKey key)
        {
            ImmutableAvlTree<TKey, TValue> newTree;
            var comp = key.CompareTo(m_key);
            if (0 == comp)
            {
                newTree = DeleteCurrentCore();
            }
            else if (comp < 0)
            {
                newTree = CreateNew(m_left.DeleteCore(key), m_right);
            }
            else
            {
                newTree = CreateNew(m_left, m_right.DeleteCore(key));
            }

            return newTree.EnsureBalanced();
        }

        protected virtual bool TryFindCore(TKey key, out TValue value)
        {
            ImmutableAvlTree<TKey, TValue> next;
            var comp = key.CompareTo(m_key);
            if (0 == comp)
            {
                value = m_value;
                return true;
            }
            else if (comp < 0)
            {
                next = m_left;
            }
            else
            {
                next = m_right;
            }

            return next.TryFind(key, out value);
        }

        protected virtual int CalculateBalanceFactor()
        {
            return m_right.Height - m_left.Height;
        }

        #endregion

        #region Private

        private ImmutableAvlTree<TKey, TValue> DeleteCurrentCore()
        {
            if (IsLeaf)
            {
                return Empty;
            }
            else if (Left.IsEmpty)
            {
                return Right;
            }
            else if (Right.IsEmpty)
            {
                return Left;
            }
            else
            {
                var cur = m_left;
                while (!cur.Right.IsEmpty)
                {
                    cur = cur.Right;
                }

                var newTree = CreateNew(m_left.DeleteCore(cur.Key), m_right);
                return newTree.EnsureBalanced();
            }
        }

        private ImmutableAvlTree<TKey, TValue> EnsureBalanced()
        {
            var balance = CalculateBalanceFactor();
            if (Math.Abs(balance) <= 1)
            {
                return this;
            }

            if (balance > 0)
            {
                // Right imbalance
                var rightBalance = m_right.CalculateBalanceFactor();
                if (rightBalance > 0)
                {
                    return RotateLeft();
                }
                else
                {
                    return RotateDoubleLeft();
                }
            }
            else
            {
                // Left imbalance
                var leftBalance = m_left.CalculateBalanceFactor();
                if (leftBalance < 0)
                {
                    return RotateRight();
                }
                else
                {
                    return RotateDoubleRight();
                }
            }
        }

        /// <summary>
        /// Single left AVL tree rotation
        /// </summary>
        /// <returns></returns>
        private ImmutableAvlTree<TKey, TValue> RotateLeft()
        {
            var newLeft = CreateNew(m_left, Right.Left);
            return Right.CreateNew(newLeft, Right.Right);
        }

        /// <summary>
        /// Single right AVL tree rotation
        /// </summary>
        /// <returns></returns>
        private ImmutableAvlTree<TKey, TValue> RotateRight()
        {
            var newRight = CreateNew(Left.Right, m_right);
            return Left.CreateNew(Left.Left, newRight);
        }

        /// <summary>
        /// Left-Right rotation
        /// </summary>
        /// <returns></returns>
        private ImmutableAvlTree<TKey, TValue> RotateDoubleRight()
        {
            var newLeft = Left.RotateLeft();
            var newHead = CreateNew(newLeft, m_right);
            return newHead.RotateRight();
        }

        /// <summary>
        /// Right-Left rotation
        /// </summary>
        /// <returns></returns>
        private ImmutableAvlTree<TKey, TValue> RotateDoubleLeft()
        {
            var newRight = Right.RotateRight();
            var newHead = CreateNew(m_left, newRight);
            return newHead.RotateLeft();
        }


        private ImmutableAvlTree<TKey, TValue> CreateNew(ImmutableAvlTree<TKey, TValue> left, ImmutableAvlTree<TKey, TValue> right)
        {
            return new ImmutableAvlTree<TKey, TValue>(m_key, m_value, left, right);
        }

        #endregion

        #region IEnumerable<Tuple<TKey,TValue>> Members

        public IEnumerator<Tuple<TKey, TValue>> GetEnumerator()
        {
            if (IsEmpty)
            {
                yield break;
            }

            var toVisit = new Stack<ImmutableAvlTree<TKey, TValue>>();
            toVisit.Push(this);

            while ( toVisit.Any() )
            {
                var cur = toVisit.Pop();
                yield return Tuple.Create(cur.Key, cur.Value);

                if (!cur.Left.IsEmpty)
                {
                    toVisit.Push(cur.Left);
                }

                if (!cur.Right.IsEmpty)
                {
                    toVisit.Push(cur.Right);
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Tuple<TKey, TValue>>)this).GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("K: {0}", m_key);
            if (!m_left.IsEmpty)
            {
                builder.AppendFormat("(L: {0})", m_left.ToString());
            }
            if (!m_right.IsEmpty)
            {
                builder.AppendFormat("(R: {0})", m_right.ToString());
            }
            return builder.ToString();
        }

        #region IReadOnlyMap<TKey,TValue> Members

        TValue IReadOnlyMap<TKey, TValue>.this[TKey key]
        {
            get { return Find(key).Value; }
        }

        #endregion

        #region IPersistentMap<TKey,TValue> Members

        IPersistentMap<TKey, TValue> IPersistentMap<TKey, TValue>.Add(TKey key, TValue value)
        {
            return Add(key, value);
        }

        IPersistentMap<TKey, TValue> IPersistentMap<TKey, TValue>.Update(TKey key, TValue value)
        {
            return Update(key, value);
        }

        IPersistentMap<TKey, TValue> IPersistentMap<TKey, TValue>.Remove(TKey key)
        {
            return Delete(key);
        }

        #endregion
    }

    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public static class ImmutableAvlTree
    {
        /// <summary>
        /// Create a new AVL tree based on the passed in parameters.  Comparer(Of T) will automatically create 
        /// a comparer for any type that implements IComparable(Of T) so leverage that class
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ImmutableAvlTree<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
            where TKey : IComparable<TKey>
        {
            return new ImmutableAvlTree<TKey, TValue>(key, value);
        }

        public static ImmutableAvlTree<TKey, TValue> Create<TKey, TValue>(IEnumerable<Tuple<TKey, TValue>> enumerable)
            where TKey : IComparable<TKey>
        {
            var tree = ImmutableAvlTree<TKey, TValue>.Empty;
            foreach (var cur in enumerable)
            {
                tree = tree.Add(cur.First, cur.Second);
            }
            return tree;
        }

        /// <summary>
        /// Create from an IDictionary instance
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ImmutableAvlTree<TKey, TValue> Create<TKey, TValue>(IDictionary<TKey, TValue> source)
            where TKey : IComparable<TKey>
        {
            var tree = ImmutableAvlTree<TKey, TValue>.Empty;
            foreach (var cur in source)
            {
                tree = tree.Add(cur.Key, cur.Value);
            }
            return tree;
        }

    }
}
