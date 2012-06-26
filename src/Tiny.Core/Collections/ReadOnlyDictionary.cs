// ReadOnlyDictionary.cs
//  
// Author:
//     Scott Wisniewski <scott@scottdw2.com>
//  
// Copyright (c) 2012 Scott Wisniewski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if posix

using System.Collections;
using System.Collections.Generic;
using Tiny;

//As of the time of me writing this,the tip of the mono master repo does not define the ReadOnlyDictionaryClass from
//v4.5 of the .net framework, so I'm defining my own version here. Once mono adds their copy, we should remove this.
//NOTE: I haven't been super careful about making the interface match exactly that of ReadOnlyDictionary from the BCL,
//mainly because I'm on an airplane and I don't have access to the internet to lookup the msdn docs. If you see
//compatability issues,that's probably why.
namespace System.Collections.ObjectModel
{
    class ReadOnlyDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IDictionary,
        ICollection
    {
        private IDictionary<TKey, TValue> m_wrapped;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> wrapped)
        {
            m_wrapped = wrapped.CheckNotNull("wrapped");
        }

        void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
        {
            throw new NotSupportedException("Dictionary is read only.");
        }

        public bool ContainsKey (TKey key)
        {
            return m_wrapped.ContainsKey(key);
        }
        
        bool IDictionary<TKey, TValue>.Remove (TKey key)
        {
            throw new NotSupportedException("Dictionary is read only.");
        }
        
        public bool TryGetValue (TKey key, out TValue value)
        {
            return m_wrapped.TryGetValue(key,out value);
        }
        
        TValue IDictionary<TKey, TValue>.this[TKey key]
        { 
            get
            {
                return this[key];
            }
            set
            {
                throw new NotSupportedException("Dictionary is read only.");
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            get
            {
                var ret = m_wrapped.Keys;
                if (! ret.AssumeNotNull().IsReadOnly) {
                    ret = new Collection<TKey>(ret);
                }
                return ret;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                var ret = m_wrapped.Keys;
                if (! (ret.AssumeNotNull().IsReadOnly && (ret is ICollection))) {
                    return new Collection<TKey>(ret);
                }
                return (ICollection)ret;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                var ret= m_wrapped.Values;
                if (!ret.AssumeNotNull().IsReadOnly) {
                    ret= new Collection<TValue>(ret);
                }
                return ret;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                var ret = m_wrapped.Values;
                if (!(ret.AssumeNotNull().IsReadOnly && ret is ICollection)) {
                    ret = new Collection<TValue>(ret);
                }
                return (ICollection)ret;
            }
        }

        public int Count {
            get { return m_wrapped.Count; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Dictionary is read only.");
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
        {
            throw new NotSupportedException("Dictionary is read only.");
        }

        public bool Contains (KeyValuePair<TKey, TValue> item)
        {
            return m_wrapped.Contains(item);
        }

        public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            m_wrapped.CopyTo(array,arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("Dictionary is read only.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
        {
            return new Enumerator(this.GetEnumerator());
        }

        public TValue this [TKey key]
        {
            get { return m_wrapped[key]; }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach (var item in this) {
                    yield return item.Key;
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var item in this) {
                    yield return item.Value;
                }
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this.GetEnumerator());
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                throw new NotSupportedException("Dictionary is read only");
            }
        }

        void IDictionary.Add(object key,object value)
        {
            throw new NotSupportedException("Dictionary is read only");
        }

        void IDictionary.Clear()
        {
            throw new NotSupportedException("Dictionary is read only");
        }

        bool IDictionary.Contains(object o)
        {
            return o is TKey && m_wrapped.ContainsKey((TKey)o);
        }

        void IDictionary.Remove(object o)
        {
            throw new NotSupportedException("Dictionary is read only");
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                var col = m_wrapped as ICollection;
                return col != null && col.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                var col = m_wrapped as ICollection;
                if (col != null) {
                    return col.SyncRoot;
                }
                return null;
            }
        }

       void ICollection.CopyTo(Array array,int index)
       {
           foreach (var item in this)
           {
               array.SetValue(item, index++);
           }
       }

        private class Collection<T> : ICollection<T>, IReadOnlyCollection<T>, ICollection
        {
            readonly ICollection<T> m_wrapped;

            public Collection(ICollection<T> wrapped)
            {
                m_wrapped = wrapped.CheckNotNull("wrapped");
            }

            public int Count {
                get { return m_wrapped.Count; }
            }

            public bool IsReadOnly {
                get { return m_wrapped.IsReadOnly; }
            }

            void ICollection<T>.Add (T item)
            {
                throw new NotSupportedException("The collection is read only");
            }

            void ICollection<T>.Clear ()
            {
                throw new NotSupportedException("The collection is read only");
            }
            
            public bool Contains (T item)
            {
                return m_wrapped.Contains(item);
            }

            public void CopyTo (T[] array, int arrayIndex)
            {
                m_wrapped.CopyTo(array, arrayIndex);
            }

            bool ICollection<T>.Remove (T item)
            {
                throw new NotSupportedException("The collection is read only");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (var item in m_wrapped)
                {
                    yield return item;
                }
            }

            bool ICollection.IsSynchronized {
                get
                {
                    var col = m_wrapped as ICollection;
                    return col != null && col.IsSynchronized;
                }
            }

            object ICollection.SyncRoot 
            {
                get
                {
                    var col = m_wrapped as ICollection;
                    if (col != null) {
                        return col.SyncRoot;
                    }
                    return null;
                }
            }

            void ICollection.CopyTo (Array array, int index)
            {
                foreach (var item in this) {
                    array.SetValue(item, index++);
                }
            }
        }

        private class Enumerator : IEnumerator<KeyValuePair<TKey,TValue>>, IDictionaryEnumerator
        {
            readonly IEnumerator<KeyValuePair<TKey,TValue>> m_wrapped;

            public Enumerator(IEnumerator<KeyValuePair<TKey,TValue>> wrapped)
            {
                m_wrapped = wrapped.CheckNotNull("wrapped");
            }

            public void Dispose()
            {
               if (m_wrapped != null) {
                   m_wrapped.Dispose();
               }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return m_wrapped.Current; }
            }

            public bool MoveNext()
            {
                return m_wrapped.MoveNext();
            }

            object IDictionaryEnumerator.Key
            {
                get { return Current.Key; }
            }

            object IDictionaryEnumerator.Value
            {
                get{ return Current.Value; }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get { return new DictionaryEntry(Current.Key, Current.Value); }
            }

            public void Reset()
            {
                m_wrapped.Reset();
            }
        }
    }
}

#endif
