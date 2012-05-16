using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Collections
{
    public class BinarySearchTreeNode<TKey, TValue> : RedBlackTreeNode<BinarySearchTreeNode<TKey, TValue>>
    {
        public TKey Key { get; internal set; }
        public TValue Value { get; internal set; }

        public BinarySearchTreeNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [SuppressMessage("Microsoft.Naming", "CA1710")]
    public sealed class BinarySearchTree<TKey, TValue> : RedBlackTree<TKey, BinarySearchTreeNode<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        private sealed class BinarySearchTreeNodeInformation : RedBlackTreeNodeInformation<TKey, BinarySearchTreeNode<TKey, TValue>>
        {
            public BinarySearchTreeNodeInformation(IComparer<TKey> comparer)
                : base(comparer)
            {

            }

            public override TKey GetKey(BinarySearchTreeNode<TKey, TValue> node)
            {
                return node.Key;
            }

            public override void SwapData(BinarySearchTreeNode<TKey, TValue> left, BinarySearchTreeNode<TKey, TValue> right)
            {
                TKey temp = left.Key;
                left.Key = right.Key;
                right.Key = temp;

                TValue value = left.Value;
                left.Value = right.Value;
                right.Value = value;
            }

        }

        public BinarySearchTree(IComparer<TKey> comparer)
            : base(new BinarySearchTreeNodeInformation(comparer))
        {

        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            Add(new BinarySearchTreeNode<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            BinarySearchTreeNode<TKey, TValue> found;
            return TryGetNode(key, out found);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                List<TKey> list = new List<TKey>();
                foreach (var cur in this)
                {
                    list.Add(cur.Key);
                }
                return list;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            BinarySearchTreeNode<TKey, TValue> found;
            if (!TryGetNode(key, out found))
            {
                value = default(TValue);
                return false;
            }

            value = found.Value;
            return true;
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> list = new List<TValue>();
                foreach (var cur in this)
                {
                    list.Add(cur.Value);
                }
                return list;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (!TryGetValue(key, out value))
                {
                    throw new ArgumentOutOfRangeException("key");
                }

                return value;
            }
            set
            {
                BinarySearchTreeNode<TKey, TValue> node;
                if (TryGetNode(key, out node))
                {
                    node.Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            this.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var cur in this)
            {
                array[index] = new KeyValuePair<TKey, TValue>(cur.Key, cur.Value);
                ++index;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            foreach (var cur in this)
            {
                yield return new KeyValuePair<TKey, TValue>(cur.Key, cur.Value);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        #endregion
    }

    public static class BinarySearchTree
    {
        public static BinarySearchTree<TKey, TValue> Create<TKey, TValue>()
            where TKey : IComparable<TKey>
        {
            return new BinarySearchTree<TKey, TValue>(Comparer<TKey>.Default);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification="Used for type infererence only")]
        public static BinarySearchTree<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
            where TKey : IComparable<TKey>
        {
            return new BinarySearchTree<TKey, TValue>(Comparer<TKey>.Default);
        }

        public static BinarySearchTree<TKey, TValue> Create<TKey, TValue>(IComparer<TKey> comparer)
        {
            return new BinarySearchTree<TKey, TValue>(comparer);
        }
    }


}
