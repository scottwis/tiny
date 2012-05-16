using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Collections
{
    #region NodeColor

    public enum NodeColor
    {
        Red,
        Black
    }

    #endregion

    #region RedBlackTreeNode<TNode>

    /// <summary>
    /// Base class for RedBlackTreeNode classes.  
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class RedBlackTreeNode<TNode>
        where TNode : RedBlackTreeNode<TNode>
    {
        public NodeColor NodeColor { get; internal set; }
        public TNode Parent { get; internal set; }
        public TNode Left { get; internal set; }
        public TNode Right { get; internal set; }

        /// <summary>
        /// Is this the left child of the Parent node
        /// </summary>
        public bool IsLeftChild
        {
            get
            {
                if (Parent != null && Object.ReferenceEquals(Parent.Left, this))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Is this the right child of the Parent node
        /// </summary>
        public bool IsRightChild
        {
            get
            {
                if (Parent != null && Object.ReferenceEquals(Parent.Right, this))
                {
                    return true;
                }

                return false;
            }
        }

        internal TNode GrandParent
        {
            get { return Parent != null ? Parent.Parent : null; }
        }

        internal TNode Uncle
        {
            get
            {
                var grandParent = GrandParent;
                if (grandParent != null)
                {
                    if (Object.ReferenceEquals(Parent, grandParent.Left))
                    {
                        return grandParent.Right;
                    }
                    else
                    {
                        return grandParent.Left;
                    }
                }

                return null;
            }
        }

        internal TNode Sibling
        {
            get
            {
                if (Parent != null)
                {
                    if (IsLeftChild)
                    {
                        return Parent.Left;
                    }
                    else
                    {
                        return Parent.Right;
                    }
                }

                return null;
            }
        }

        public RedBlackTreeNode()
        {
            this.NodeColor = NodeColor.Red;
        }
    }

    #endregion

    #region RedBlackTreeNodeInformation<TKey,TNode>

    public abstract class RedBlackTreeNodeInformation<TKey, TNode>
        where TNode : RedBlackTreeNode<TNode>
    {
        private IComparer<TKey> m_comparer;

        public IComparer<TKey> Comparer
        {
            get { return m_comparer; }
        }

        protected RedBlackTreeNodeInformation(IComparer<TKey> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            m_comparer = comparer;
        }

        public abstract TKey GetKey(TNode node);

        public abstract void SwapData(TNode left, TNode right);
    }

    #endregion

    #region RedBlackTree<TKey,TNode>

    [SuppressMessage("Microsoft.Naming", "CA1710")]
    public class RedBlackTree<TKey, TNode> : ICollection<TNode>
        where TNode : RedBlackTreeNode<TNode>
    {
        private TNode m_rootNode;
        private int m_count;
        private IComparer<TKey> m_comparer;
        private RedBlackTreeNodeInformation<TKey, TNode> m_info;

        public TNode Root
        {
            get { return m_rootNode; }
        }

        public int Count
        {
            get { return m_count; }
        }

        public ICollection<TNode> Nodes
        {
            get { return new List<TNode>(this); }
        }

        public RedBlackTree(RedBlackTreeNodeInformation<TKey, TNode> info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            m_comparer = info.Comparer;
            m_info = info;
        }

        #region Public Methods

        public bool TryGetNode(TKey search, out TNode node)
        {
            var cur = m_rootNode;
            while (cur != null)
            {
                int comp = m_comparer.Compare(m_info.GetKey(cur), search);
                if (comp == 0)
                {
                    node = cur;
                    return true;
                }
                else if (comp < 0)
                {
                    cur = cur.Right;
                }
                else
                {
                    cur = cur.Left;
                }
            }

            node = null;
            return false;
        }

        public void Add(TNode toInsert)
        {
            if (m_rootNode == null)
            {
                m_rootNode = toInsert;
                m_rootNode.Parent = null;
                m_rootNode.Left = null;
                m_rootNode.Right = null;
                m_rootNode.NodeColor = NodeColor.Black;
            }
            else
            {
                var cur = m_rootNode;
                do
                {
                    int comp = m_comparer.Compare(m_info.GetKey(cur), m_info.GetKey(toInsert));
                    if (comp == 0)
                    {
                        throw new InvalidOperationException("Duplicate node detected");
                    }

                    if (comp <= 0)
                    {
                        if (null == cur.Right)
                        {
                            cur.Right = toInsert;
                            toInsert.Parent = cur;
                            break;
                        }
                        else
                        {
                            cur = cur.Right;
                        }
                    }
                    else
                    {
                        if (null == cur.Left)
                        {
                            cur.Left = toInsert;
                            toInsert.Parent = cur;
                            break;
                        }
                        else
                        {
                            cur = cur.Left;
                        }
                    }
                } while (true);

                PostInsert(toInsert);
            }

            m_count++;
        }

        public bool Remove(TKey key)
        {
            TNode node;
            if (!TryGetNode(key, out node))
            {
                return false;
            }

            return Remove(node);
        }

        public bool Remove(TNode node)
        {
            // If there are two children then we need to copy the value into a child node and
            // actually delete that node
            if (node.Left != null && node.Right != null)
            {
                var cur = node.Left;
                while (cur.Left != null)
                {
                    cur = cur.Left;
                }

                m_info.SwapData(node, cur);
                node = cur;
            }

            RemoveCore(node);
            --m_count;
            return true;
        }

        public void Clear()
        {
            m_rootNode = null;
            m_count = 0;
        }

        #endregion

        #region Private Methods

        private void PostInsert(TNode inserted)
        {
            Contract.ThrowIfFalse(NodeColor.Red == inserted.NodeColor);
            if (Object.ReferenceEquals(m_rootNode, inserted))
            {
                inserted.NodeColor = NodeColor.Black;
                return;
            }

            PostInsertCheckParentBlack(inserted);
        }

        private void PostInsertCheckParentBlack(TNode inserted)
        {
            if (inserted.Parent.NodeColor == NodeColor.Black)
            {
                // Rules are maintained when you insert under a black node
                return;
            }

            PostInsertCheckRedUncle(inserted);
        }


        private void PostInsertCheckRedUncle(TNode inserted)
        {
            if (inserted.Uncle != null && NodeColor.Red == inserted.Uncle.NodeColor)
            {
                // Parent and Uncle are red.  Make them black and switch the GrandParent to red.
                // GrandParent may now violate the rulse so run PostInsert
                inserted.Uncle.NodeColor = NodeColor.Black;
                inserted.Parent.NodeColor = NodeColor.Black;
                inserted.GrandParent.NodeColor = NodeColor.Red;
                PostInsert(inserted.GrandParent);
                return;
            }

            PostInsertCheckMismatchParentUncle(inserted);
        }

        private void PostInsertCheckMismatchParentUncle(TNode inserted)
        {
            if (inserted.Uncle == null || NodeColor.Black == inserted.Uncle.NodeColor)
            {
                // Parent and Uncle have differing node colors.  We already know that the Parent
                // is red 
                if (inserted.IsRightChild && inserted.Parent.IsLeftChild)
                {
                    RotateLeft(inserted.Parent);
                    inserted = inserted.Left;
                }
                else if (inserted.IsLeftChild && inserted.Parent.IsRightChild)
                {
                    RotateRight(inserted.Parent);
                    inserted = inserted.Right;
                }
            }

            if (inserted.Uncle == null || NodeColor.Black == inserted.Uncle.NodeColor)
            {
                inserted.Parent.NodeColor = NodeColor.Black;
                inserted.GrandParent.NodeColor = NodeColor.Red;
                if (inserted.IsLeftChild && inserted.Parent.IsLeftChild)
                {
                    RotateRight(inserted.GrandParent);
                }
                else
                {
                    RotateLeft(inserted.GrandParent);
                }
            }
        }

        /// <summary>
        /// For rotation functions, pass in the top of the subtree that needs to be
        /// rotated.  For instance
        /// 
        ///   P
        ///  / \
        ///  L  R
        ///  
        /// If we are trying to get the following tree
        /// 
        ///    R
        ///   /
        ///  P
        /// /
        /// L
        /// 
        /// Then "P" should be passed in
        ///
        /// </summary>
        /// <param name="topNode"></param>
        private void RotateLeft(TNode topNode)
        {
            Contract.ThrowIfNull(topNode);
            Contract.ThrowIfNull(topNode.Right);

            // 1) Make "R" the new top node
            var parent = topNode.Parent;
            var rightNode = topNode.Right;
            rightNode.Parent = parent;
            if (topNode.IsRightChild)
            {
                parent.Right = rightNode;
            }
            else if (topNode.IsLeftChild)
            {
                parent.Left = rightNode;
            }
            else
            {
                // It's the root node
                Contract.ThrowIfFalse(Object.ReferenceEquals(m_rootNode, topNode));
                m_rootNode = rightNode;
            }

            // 2) Make the Left child of "R" be the right child of "P" 
            if (rightNode.Left != null)
            {
                topNode.Right = rightNode.Left;
                topNode.Right.Parent = topNode;
            }
            else
            {
                topNode.Right = null;
            }

            // 3) Make "P" the left child of "R"
            topNode.Parent = rightNode;
            rightNode.Left = topNode;
        }

        /// <summary>
        /// For rotation functions, pass in the top of the subtree that needs to be
        /// rotated.  For instance
        /// 
        ///   P
        ///  / \
        ///  L  R
        ///  
        /// If we are trying to get the following tree
        /// 
        ///    L
        ///     \
        ///      P
        ///       \
        ///        R
        /// 
        /// Then "P" should be passed in
        ///
        /// </summary>
        /// <param name="topNode"></param>
        private void RotateRight(TNode topNode)
        {
            Contract.ThrowIfNull(topNode);
            Contract.ThrowIfNull(topNode.Left);

            var parent = topNode.Parent;
            var leftNode = topNode.Left;

            // 1) Make "L" the new top node 
            leftNode.Parent = parent;
            if (topNode.IsLeftChild)
            {
                parent.Left = leftNode;
            }
            else if (topNode.IsRightChild)
            {
                parent.Right = leftNode;
            }
            else
            {
                Contract.ThrowIfFalse(Object.ReferenceEquals(m_rootNode, topNode));
                m_rootNode = leftNode;
            }

            // 2) Make the right child of "L" the left child of "P"
            if (leftNode.Right != null)
            {
                topNode.Left = leftNode.Right;
                topNode.Left.Parent = topNode;
            }
            else
            {
                topNode.Left = null;
            }

            // 3) Make "P" the right child of "L"
            leftNode.Right = topNode;
            topNode.Parent = leftNode;
        }

        /// <summary>
        /// Called to remove a node with at least one null child
        /// </summary>
        /// <param name="cur"></param>
        private void RemoveCore(TNode cur)
        {
            Contract.ThrowIfNull(cur);

            // child can be null
            TNode child = (cur.Left != null) ? cur.Left : cur.Right;
            TNode parent = cur.Parent;
            bool isLeftChild = cur.IsLeftChild;

            if (object.ReferenceEquals(cur, m_rootNode))
            {
                // Simple case, deleting the root node
                m_rootNode = child;
                if (child != null)
                {
                    child.Parent = null;
                }
            }
            else
            {
                if (Object.ReferenceEquals(parent.Left, cur))
                {
                    parent.Left = child;
                }
                else
                {
                    parent.Right = child;
                }

                if (child != null)
                {
                    child.Parent = parent;
                }
            }

            if (cur.NodeColor == NodeColor.Black)
            {
                if (child != null && child.NodeColor == NodeColor.Red)
                {
                    child.NodeColor = NodeColor.Black;
                }
                else
                {
                    PostRemove(parent, child, isLeftChild);
                }
            }
        }

        private void PostRemove(TNode parent, TNode cur, bool isCurLeftChild)
        {
            if (Object.ReferenceEquals(cur, m_rootNode))
            {
                return;
            }

            var sibling = isCurLeftChild ? parent.Right : parent.Left;
            PostRemoveCheckRedSibling(parent, sibling, isCurLeftChild);
        }

        private void PostRemoveCheckRedSibling(TNode parent, TNode sibling, bool isCurLeftChild)
        {
            if (sibling != null && NodeColor.Red == sibling.NodeColor)
            {
                parent.NodeColor = NodeColor.Red;
                sibling.NodeColor = NodeColor.Black;
                if (isCurLeftChild)
                {
                    RotateLeft(parent);
                }
                else
                {
                    RotateRight(parent);
                }

                sibling = isCurLeftChild ? parent.Right : parent.Left;
            }

            PostRemoveCheckAllBlack(parent, sibling, isCurLeftChild);
        }

        private void PostRemoveCheckAllBlack(TNode parent, TNode sibling, bool isCurLeftChild)
        {
            bool siblingAndChildrenBlack = sibling != null
                && NodeColor.Black == sibling.NodeColor
                && (sibling.Left == null || NodeColor.Black == sibling.Left.NodeColor)
                && (sibling.Right == null || NodeColor.Black == sibling.Right.NodeColor);
            if (siblingAndChildrenBlack)
            {
                if (NodeColor.Black == parent.NodeColor)
                {
                    sibling.NodeColor = NodeColor.Red;
                    PostRemove(parent.Parent, parent, parent.IsLeftChild);
                }
                else
                {
                    parent.NodeColor = NodeColor.Black;
                    sibling.NodeColor = NodeColor.Red;
                }
                return;
            }

            PostRemoveCheckSiblingChildColorMismatch(parent, sibling, isCurLeftChild);
        }

        private void PostRemoveCheckSiblingChildColorMismatch(TNode parent, TNode sibling, bool isCurLeftChild)
        {
            if (sibling != null && NodeColor.Black == sibling.NodeColor)
            {
                if (isCurLeftChild
                    && (sibling.Left != null && NodeColor.Red == sibling.Left.NodeColor)
                    && (sibling.Right == null || NodeColor.Black == sibling.Right.NodeColor))
                {
                    sibling.NodeColor = NodeColor.Red;
                    sibling.Left.NodeColor = NodeColor.Black;
                    RotateRight(sibling);
                }
                else if (!isCurLeftChild
                    && (sibling.Left == null || NodeColor.Black == sibling.Left.NodeColor)
                    && (sibling.Right != null && NodeColor.Red == sibling.Right.NodeColor))
                {
                    sibling.NodeColor = NodeColor.Red;
                    sibling.Right.NodeColor = NodeColor.Black;
                    RotateLeft(sibling);
                }

                sibling = isCurLeftChild ? parent.Right : parent.Left;
            }

            if (sibling != null)
            {
                sibling.NodeColor = parent.NodeColor;
                parent.NodeColor = NodeColor.Black;
                if (isCurLeftChild)
                {
                    if (sibling.Right != null)
                    {
                        sibling.Right.NodeColor = NodeColor.Black;
                    }
                    RotateLeft(parent);
                }
                else
                {
                    if (sibling.Left != null)
                    {
                        sibling.Left.NodeColor = NodeColor.Black;
                    }
                    RotateRight(parent);
                }
            }
        }

        #endregion

        #region IEnumerable<RedBlackTreeNode<TKey,TValue>> Members

        public IEnumerator<TNode> GetEnumerator()
        {
            if (m_rootNode != null)
            {
                var toVisit = new Queue<TNode>();
                toVisit.Enqueue(m_rootNode);

                while (toVisit.Count > 0)
                {
                    var cur = toVisit.Dequeue();
                    yield return cur;

                    if (cur.Left != null)
                    {
                        toVisit.Enqueue(cur.Left);
                    }

                    if (cur.Right != null)
                    {
                        toVisit.Enqueue(cur.Right);
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TNode>)this).GetEnumerator();
        }

        #endregion

        #region ICollection<TNode> Members

        void ICollection<TNode>.Add(TNode item)
        {
            Add(item);
        }

        void ICollection<TNode>.Clear()
        {
            Clear();
        }

        bool ICollection<TNode>.Contains(TNode item)
        {
            TNode found;
            return TryGetNode(m_info.GetKey(item), out found);
        }

        void ICollection<TNode>.CopyTo(TNode[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var cur in this)
            {
                array[index] = cur;
                ++index;
            }
        }

        bool ICollection<TNode>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<TNode>.Remove(TNode item)
        {
            return Remove(item);
        }

        #endregion
    }

    #endregion

}


