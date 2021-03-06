using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras
{
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}")]
    [Serializable]
    public struct Tuple<TFirst> : IEquatable<Tuple<TFirst>>, IComparable<Tuple<TFirst>>, IComparable
    {
        private readonly TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } }

        public Tuple(TFirst valueFirst)
        {
            m_first = valueFirst;
        }
        public int Count { get { return 1; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((Tuple<TFirst>)obj);
        }
        public bool Equals(Tuple<TFirst> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            int code = 0;
            code += EqualityComparer<TFirst>.Default.GetHashCode(m_first);
            return code;
        }
        public static bool operator ==(Tuple<TFirst> left, Tuple<TFirst> right)
        {
            return EqualityComparer<Tuple<TFirst>>.Default.Equals(left, right);
        }
        public static bool operator !=(Tuple<TFirst> left, Tuple<TFirst> right)
        {
            return !EqualityComparer<Tuple<TFirst>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((Tuple<TFirst>)obj);
        }
        public int CompareTo(Tuple<TFirst> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(Tuple<TFirst> left, Tuple<TFirst> right)
        {
            return Comparer<Tuple<TFirst>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(Tuple<TFirst> left, Tuple<TFirst> right)
        {
            return Comparer<Tuple<TFirst>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class Tuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static Tuple<TFirst> Create<TFirst>(TFirst valueFirst) { return new Tuple<TFirst>(valueFirst); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}")]
    [Serializable]
    public struct Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>, IComparable<Tuple<TFirst, TSecond>>, IComparable
    {
        private readonly TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } }

        private readonly TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } }

        public Tuple(TFirst valueFirst, TSecond valueSecond)
        {
            m_first = valueFirst;
            m_second = valueSecond;
        }
        public int Count { get { return 2; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((Tuple<TFirst, TSecond>)obj);
        }
        public bool Equals(Tuple<TFirst, TSecond> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            int code = 0;
            code += EqualityComparer<TFirst>.Default.GetHashCode(m_first);
            code += EqualityComparer<TSecond>.Default.GetHashCode(m_second);
            return code;
        }
        public static bool operator ==(Tuple<TFirst, TSecond> left, Tuple<TFirst, TSecond> right)
        {
            return EqualityComparer<Tuple<TFirst, TSecond>>.Default.Equals(left, right);
        }
        public static bool operator !=(Tuple<TFirst, TSecond> left, Tuple<TFirst, TSecond> right)
        {
            return !EqualityComparer<Tuple<TFirst, TSecond>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((Tuple<TFirst, TSecond>)obj);
        }
        public int CompareTo(Tuple<TFirst, TSecond> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(Tuple<TFirst, TSecond> left, Tuple<TFirst, TSecond> right)
        {
            return Comparer<Tuple<TFirst, TSecond>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(Tuple<TFirst, TSecond> left, Tuple<TFirst, TSecond> right)
        {
            return Comparer<Tuple<TFirst, TSecond>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class Tuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static Tuple<TFirst, TSecond> Create<TFirst, TSecond>(TFirst valueFirst, TSecond valueSecond) { return new Tuple<TFirst, TSecond>(valueFirst, valueSecond); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}, Third={Third}")]
    [SuppressMessage("Microsoft.Design", "CA1005")]
    [Serializable]
    public struct Tuple<TFirst, TSecond, TThird> : IEquatable<Tuple<TFirst, TSecond, TThird>>, IComparable<Tuple<TFirst, TSecond, TThird>>, IComparable
    {
        private readonly TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } }

        private readonly TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } }

        private readonly TThird m_third;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TThird Third { get { return m_third; } }

        public Tuple(TFirst valueFirst, TSecond valueSecond, TThird valueThird)
        {
            m_first = valueFirst;
            m_second = valueSecond;
            m_third = valueThird;
        }
        public int Count { get { return 3; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    case 2: return m_third;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((Tuple<TFirst, TSecond, TThird>)obj);
        }
        public bool Equals(Tuple<TFirst, TSecond, TThird> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second) && EqualityComparer<TThird>.Default.Equals(m_third, other.m_third)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            int code = 0;
            code += EqualityComparer<TFirst>.Default.GetHashCode(m_first);
            code += EqualityComparer<TSecond>.Default.GetHashCode(m_second);
            code += EqualityComparer<TThird>.Default.GetHashCode(m_third);
            return code;
        }
        public static bool operator ==(Tuple<TFirst, TSecond, TThird> left, Tuple<TFirst, TSecond, TThird> right)
        {
            return EqualityComparer<Tuple<TFirst, TSecond, TThird>>.Default.Equals(left, right);
        }
        public static bool operator !=(Tuple<TFirst, TSecond, TThird> left, Tuple<TFirst, TSecond, TThird> right)
        {
            return !EqualityComparer<Tuple<TFirst, TSecond, TThird>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((Tuple<TFirst, TSecond, TThird>)obj);
        }
        public int CompareTo(Tuple<TFirst, TSecond, TThird> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            code = Comparer<TThird>.Default.Compare(m_third, other.m_third); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(Tuple<TFirst, TSecond, TThird> left, Tuple<TFirst, TSecond, TThird> right)
        {
            return Comparer<Tuple<TFirst, TSecond, TThird>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(Tuple<TFirst, TSecond, TThird> left, Tuple<TFirst, TSecond, TThird> right)
        {
            return Comparer<Tuple<TFirst, TSecond, TThird>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class Tuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static Tuple<TFirst, TSecond, TThird> Create<TFirst, TSecond, TThird>(TFirst valueFirst, TSecond valueSecond, TThird valueThird) { return new Tuple<TFirst, TSecond, TThird>(valueFirst, valueSecond, valueThird); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}, Third={Third}, Fourth={Fourth}")]
    [SuppressMessage("Microsoft.Design", "CA1005")]
    [Serializable]
    public struct Tuple<TFirst, TSecond, TThird, TFourth> : IEquatable<Tuple<TFirst, TSecond, TThird, TFourth>>, IComparable<Tuple<TFirst, TSecond, TThird, TFourth>>, IComparable
    {
        private readonly TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } }

        private readonly TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } }

        private readonly TThird m_third;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TThird Third { get { return m_third; } }

        private readonly TFourth m_fourth;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFourth Fourth { get { return m_fourth; } }

        public Tuple(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth)
        {
            m_first = valueFirst;
            m_second = valueSecond;
            m_third = valueThird;
            m_fourth = valueFourth;
        }
        public int Count { get { return 4; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    case 2: return m_third;
                    case 3: return m_fourth;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((Tuple<TFirst, TSecond, TThird, TFourth>)obj);
        }
        public bool Equals(Tuple<TFirst, TSecond, TThird, TFourth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second) && EqualityComparer<TThird>.Default.Equals(m_third, other.m_third) && EqualityComparer<TFourth>.Default.Equals(m_fourth, other.m_fourth)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            int code = 0;
            code += EqualityComparer<TFirst>.Default.GetHashCode(m_first);
            code += EqualityComparer<TSecond>.Default.GetHashCode(m_second);
            code += EqualityComparer<TThird>.Default.GetHashCode(m_third);
            code += EqualityComparer<TFourth>.Default.GetHashCode(m_fourth);
            return code;
        }
        public static bool operator ==(Tuple<TFirst, TSecond, TThird, TFourth> left, Tuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return EqualityComparer<Tuple<TFirst, TSecond, TThird, TFourth>>.Default.Equals(left, right);
        }
        public static bool operator !=(Tuple<TFirst, TSecond, TThird, TFourth> left, Tuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return !EqualityComparer<Tuple<TFirst, TSecond, TThird, TFourth>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((Tuple<TFirst, TSecond, TThird, TFourth>)obj);
        }
        public int CompareTo(Tuple<TFirst, TSecond, TThird, TFourth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            code = Comparer<TThird>.Default.Compare(m_third, other.m_third); if (code != 0) { return code; }
            code = Comparer<TFourth>.Default.Compare(m_fourth, other.m_fourth); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(Tuple<TFirst, TSecond, TThird, TFourth> left, Tuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return Comparer<Tuple<TFirst, TSecond, TThird, TFourth>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(Tuple<TFirst, TSecond, TThird, TFourth> left, Tuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return Comparer<Tuple<TFirst, TSecond, TThird, TFourth>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class Tuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static Tuple<TFirst, TSecond, TThird, TFourth> Create<TFirst, TSecond, TThird, TFourth>(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth) { return new Tuple<TFirst, TSecond, TThird, TFourth>(valueFirst, valueSecond, valueThird, valueFourth); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}, Third={Third}, Fourth={Fourth}, Fifth={Fifth}")]
    [SuppressMessage("Microsoft.Design", "CA1005")]
    [Serializable]
    public struct Tuple<TFirst, TSecond, TThird, TFourth, TFifth> : IEquatable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>, IComparable<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>, IComparable
    {
        private readonly TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } }

        private readonly TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } }

        private readonly TThird m_third;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TThird Third { get { return m_third; } }

        private readonly TFourth m_fourth;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFourth Fourth { get { return m_fourth; } }

        private readonly TFifth m_fifth;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFifth Fifth { get { return m_fifth; } }

        public Tuple(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth, TFifth valueFifth)
        {
            m_first = valueFirst;
            m_second = valueSecond;
            m_third = valueThird;
            m_fourth = valueFourth;
            m_fifth = valueFifth;
        }
        public int Count { get { return 5; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    case 2: return m_third;
                    case 3: return m_fourth;
                    case 4: return m_fifth;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((Tuple<TFirst, TSecond, TThird, TFourth, TFifth>)obj);
        }
        public bool Equals(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second) && EqualityComparer<TThird>.Default.Equals(m_third, other.m_third) && EqualityComparer<TFourth>.Default.Equals(m_fourth, other.m_fourth) && EqualityComparer<TFifth>.Default.Equals(m_fifth, other.m_fifth)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            int code = 0;
            code += EqualityComparer<TFirst>.Default.GetHashCode(m_first);
            code += EqualityComparer<TSecond>.Default.GetHashCode(m_second);
            code += EqualityComparer<TThird>.Default.GetHashCode(m_third);
            code += EqualityComparer<TFourth>.Default.GetHashCode(m_fourth);
            code += EqualityComparer<TFifth>.Default.GetHashCode(m_fifth);
            return code;
        }
        public static bool operator ==(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> left, Tuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return EqualityComparer<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Equals(left, right);
        }
        public static bool operator !=(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> left, Tuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return !EqualityComparer<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((Tuple<TFirst, TSecond, TThird, TFourth, TFifth>)obj);
        }
        public int CompareTo(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            code = Comparer<TThird>.Default.Compare(m_third, other.m_third); if (code != 0) { return code; }
            code = Comparer<TFourth>.Default.Compare(m_fourth, other.m_fourth); if (code != 0) { return code; }
            code = Comparer<TFifth>.Default.Compare(m_fifth, other.m_fifth); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> left, Tuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return Comparer<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(Tuple<TFirst, TSecond, TThird, TFourth, TFifth> left, Tuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return Comparer<Tuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class Tuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static Tuple<TFirst, TSecond, TThird, TFourth, TFifth> Create<TFirst, TSecond, TThird, TFourth, TFifth>(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth, TFifth valueFifth) { return new Tuple<TFirst, TSecond, TThird, TFourth, TFifth>(valueFirst, valueSecond, valueThird, valueFourth, valueFifth); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}")]
    [Serializable]
    public struct MutableTuple<TFirst> : IEquatable<MutableTuple<TFirst>>, IComparable<MutableTuple<TFirst>>, IComparable
    {
        private TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } set { m_first = value; } }

        public MutableTuple(TFirst valueFirst)
        {
            m_first = valueFirst;
        }
        public int Count { get { return 1; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: m_first = (TFirst)value; break;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((MutableTuple<TFirst>)obj);
        }
        public bool Equals(MutableTuple<TFirst> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            return 1;
        }
        public static bool operator ==(MutableTuple<TFirst> left, MutableTuple<TFirst> right)
        {
            return EqualityComparer<MutableTuple<TFirst>>.Default.Equals(left, right);
        }
        public static bool operator !=(MutableTuple<TFirst> left, MutableTuple<TFirst> right)
        {
            return !EqualityComparer<MutableTuple<TFirst>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((MutableTuple<TFirst>)obj);
        }
        public int CompareTo(MutableTuple<TFirst> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(MutableTuple<TFirst> left, MutableTuple<TFirst> right)
        {
            return Comparer<MutableTuple<TFirst>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(MutableTuple<TFirst> left, MutableTuple<TFirst> right)
        {
            return Comparer<MutableTuple<TFirst>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class MutableTuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static MutableTuple<TFirst> Create<TFirst>(TFirst valueFirst) { return new MutableTuple<TFirst>(valueFirst); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}")]
    [Serializable]
    public struct MutableTuple<TFirst, TSecond> : IEquatable<MutableTuple<TFirst, TSecond>>, IComparable<MutableTuple<TFirst, TSecond>>, IComparable
    {
        private TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } set { m_first = value; } }

        private TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } set { m_second = value; } }

        public MutableTuple(TFirst valueFirst, TSecond valueSecond)
        {
            m_first = valueFirst;
            m_second = valueSecond;
        }
        public int Count { get { return 2; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: m_first = (TFirst)value; break;
                    case 1: m_second = (TSecond)value; break;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((MutableTuple<TFirst, TSecond>)obj);
        }
        public bool Equals(MutableTuple<TFirst, TSecond> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            return 1;
        }
        public static bool operator ==(MutableTuple<TFirst, TSecond> left, MutableTuple<TFirst, TSecond> right)
        {
            return EqualityComparer<MutableTuple<TFirst, TSecond>>.Default.Equals(left, right);
        }
        public static bool operator !=(MutableTuple<TFirst, TSecond> left, MutableTuple<TFirst, TSecond> right)
        {
            return !EqualityComparer<MutableTuple<TFirst, TSecond>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((MutableTuple<TFirst, TSecond>)obj);
        }
        public int CompareTo(MutableTuple<TFirst, TSecond> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(MutableTuple<TFirst, TSecond> left, MutableTuple<TFirst, TSecond> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(MutableTuple<TFirst, TSecond> left, MutableTuple<TFirst, TSecond> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class MutableTuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static MutableTuple<TFirst, TSecond> Create<TFirst, TSecond>(TFirst valueFirst, TSecond valueSecond) { return new MutableTuple<TFirst, TSecond>(valueFirst, valueSecond); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}, Third={Third}")]
    [SuppressMessage("Microsoft.Design", "CA1005")]
    [Serializable]
    public struct MutableTuple<TFirst, TSecond, TThird> : IEquatable<MutableTuple<TFirst, TSecond, TThird>>, IComparable<MutableTuple<TFirst, TSecond, TThird>>, IComparable
    {
        private TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } set { m_first = value; } }

        private TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } set { m_second = value; } }

        private TThird m_third;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TThird Third { get { return m_third; } set { m_third = value; } }

        public MutableTuple(TFirst valueFirst, TSecond valueSecond, TThird valueThird)
        {
            m_first = valueFirst;
            m_second = valueSecond;
            m_third = valueThird;
        }
        public int Count { get { return 3; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    case 2: return m_third;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: m_first = (TFirst)value; break;
                    case 1: m_second = (TSecond)value; break;
                    case 2: m_third = (TThird)value; break;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((MutableTuple<TFirst, TSecond, TThird>)obj);
        }
        public bool Equals(MutableTuple<TFirst, TSecond, TThird> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second) && EqualityComparer<TThird>.Default.Equals(m_third, other.m_third)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            return 1;
        }
        public static bool operator ==(MutableTuple<TFirst, TSecond, TThird> left, MutableTuple<TFirst, TSecond, TThird> right)
        {
            return EqualityComparer<MutableTuple<TFirst, TSecond, TThird>>.Default.Equals(left, right);
        }
        public static bool operator !=(MutableTuple<TFirst, TSecond, TThird> left, MutableTuple<TFirst, TSecond, TThird> right)
        {
            return !EqualityComparer<MutableTuple<TFirst, TSecond, TThird>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((MutableTuple<TFirst, TSecond, TThird>)obj);
        }
        public int CompareTo(MutableTuple<TFirst, TSecond, TThird> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            code = Comparer<TThird>.Default.Compare(m_third, other.m_third); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(MutableTuple<TFirst, TSecond, TThird> left, MutableTuple<TFirst, TSecond, TThird> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond, TThird>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(MutableTuple<TFirst, TSecond, TThird> left, MutableTuple<TFirst, TSecond, TThird> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond, TThird>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class MutableTuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static MutableTuple<TFirst, TSecond, TThird> Create<TFirst, TSecond, TThird>(TFirst valueFirst, TSecond valueSecond, TThird valueThird) { return new MutableTuple<TFirst, TSecond, TThird>(valueFirst, valueSecond, valueThird); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}, Third={Third}, Fourth={Fourth}")]
    [SuppressMessage("Microsoft.Design", "CA1005")]
    [Serializable]
    public struct MutableTuple<TFirst, TSecond, TThird, TFourth> : IEquatable<MutableTuple<TFirst, TSecond, TThird, TFourth>>, IComparable<MutableTuple<TFirst, TSecond, TThird, TFourth>>, IComparable
    {
        private TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } set { m_first = value; } }

        private TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } set { m_second = value; } }

        private TThird m_third;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TThird Third { get { return m_third; } set { m_third = value; } }

        private TFourth m_fourth;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFourth Fourth { get { return m_fourth; } set { m_fourth = value; } }

        public MutableTuple(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth)
        {
            m_first = valueFirst;
            m_second = valueSecond;
            m_third = valueThird;
            m_fourth = valueFourth;
        }
        public int Count { get { return 4; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    case 2: return m_third;
                    case 3: return m_fourth;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: m_first = (TFirst)value; break;
                    case 1: m_second = (TSecond)value; break;
                    case 2: m_third = (TThird)value; break;
                    case 3: m_fourth = (TFourth)value; break;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((MutableTuple<TFirst, TSecond, TThird, TFourth>)obj);
        }
        public bool Equals(MutableTuple<TFirst, TSecond, TThird, TFourth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second) && EqualityComparer<TThird>.Default.Equals(m_third, other.m_third) && EqualityComparer<TFourth>.Default.Equals(m_fourth, other.m_fourth)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            return 1;
        }
        public static bool operator ==(MutableTuple<TFirst, TSecond, TThird, TFourth> left, MutableTuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return EqualityComparer<MutableTuple<TFirst, TSecond, TThird, TFourth>>.Default.Equals(left, right);
        }
        public static bool operator !=(MutableTuple<TFirst, TSecond, TThird, TFourth> left, MutableTuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return !EqualityComparer<MutableTuple<TFirst, TSecond, TThird, TFourth>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((MutableTuple<TFirst, TSecond, TThird, TFourth>)obj);
        }
        public int CompareTo(MutableTuple<TFirst, TSecond, TThird, TFourth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            code = Comparer<TThird>.Default.Compare(m_third, other.m_third); if (code != 0) { return code; }
            code = Comparer<TFourth>.Default.Compare(m_fourth, other.m_fourth); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(MutableTuple<TFirst, TSecond, TThird, TFourth> left, MutableTuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond, TThird, TFourth>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(MutableTuple<TFirst, TSecond, TThird, TFourth> left, MutableTuple<TFirst, TSecond, TThird, TFourth> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond, TThird, TFourth>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class MutableTuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static MutableTuple<TFirst, TSecond, TThird, TFourth> Create<TFirst, TSecond, TThird, TFourth>(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth) { return new MutableTuple<TFirst, TSecond, TThird, TFourth>(valueFirst, valueSecond, valueThird, valueFourth); }
    }
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    [DebuggerDisplay("First={First}, Second={Second}, Third={Third}, Fourth={Fourth}, Fifth={Fifth}")]
    [SuppressMessage("Microsoft.Design", "CA1005")]
    [Serializable]
    public struct MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> : IEquatable<MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>>, IComparable<MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>>, IComparable
    {
        private TFirst m_first;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFirst First { get { return m_first; } set { m_first = value; } }

        private TSecond m_second;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TSecond Second { get { return m_second; } set { m_second = value; } }

        private TThird m_third;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TThird Third { get { return m_third; } set { m_third = value; } }

        private TFourth m_fourth;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFourth Fourth { get { return m_fourth; } set { m_fourth = value; } }

        private TFifth m_fifth;
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public TFifth Fifth { get { return m_fifth; } set { m_fifth = value; } }

        public MutableTuple(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth, TFifth valueFifth)
        {
            m_first = valueFirst;
            m_second = valueSecond;
            m_third = valueThird;
            m_fourth = valueFourth;
            m_fifth = valueFifth;
        }
        public int Count { get { return 5; } }
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m_first;
                    case 1: return m_second;
                    case 2: return m_third;
                    case 3: return m_fourth;
                    case 4: return m_fifth;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: m_first = (TFirst)value; break;
                    case 1: m_second = (TSecond)value; break;
                    case 2: m_third = (TThird)value; break;
                    case 3: m_fourth = (TFourth)value; break;
                    case 4: m_fifth = (TFifth)value; break;
                    default: throw new InvalidOperationException("Bad Index");
                }
            }
        }
        public override bool Equals(object obj)
        {
            return Equals((MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>)obj);
        }
        public bool Equals(MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return false; }
            if (
            EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) && EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second) && EqualityComparer<TThird>.Default.Equals(m_third, other.m_third) && EqualityComparer<TFourth>.Default.Equals(m_fourth, other.m_fourth) && EqualityComparer<TFifth>.Default.Equals(m_fifth, other.m_fifth)
            ) { return true; }
            return false;
        }
        public override int GetHashCode()
        {
            return 1;
        }
        public static bool operator ==(MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> left, MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return EqualityComparer<MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Equals(left, right);
        }
        public static bool operator !=(MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> left, MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return !EqualityComparer<MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Equals(left, right);
        }
        public int CompareTo(object obj)
        {
            return CompareTo((MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>)obj);
        }
        public int CompareTo(MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> other)
        {
            if (Object.ReferenceEquals(other, null)) { return 1; }
            int code;
            code = Comparer<TFirst>.Default.Compare(m_first, other.m_first); if (code != 0) { return code; }
            code = Comparer<TSecond>.Default.Compare(m_second, other.m_second); if (code != 0) { return code; }
            code = Comparer<TThird>.Default.Compare(m_third, other.m_third); if (code != 0) { return code; }
            code = Comparer<TFourth>.Default.Compare(m_fourth, other.m_fourth); if (code != 0) { return code; }
            code = Comparer<TFifth>.Default.Compare(m_fifth, other.m_fifth); if (code != 0) { return code; }
            return 0;
        }
        public static bool operator >(MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> left, MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Compare(left, right) > 0;
        }
        public static bool operator <(MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> left, MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> right)
        {
            return Comparer<MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>>.Default.Compare(left, right) < 0;
        }
    }
    public static partial class MutableTuple
    {
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth> Create<TFirst, TSecond, TThird, TFourth, TFifth>(TFirst valueFirst, TSecond valueSecond, TThird valueThird, TFourth valueFourth, TFifth valueFifth) { return new MutableTuple<TFirst, TSecond, TThird, TFourth, TFifth>(valueFirst, valueSecond, valueThird, valueFourth, valueFifth); }
    }
}
