using System;
using System.Collections.Generic;
using CDX.Utils;

namespace CDX.Graphics.G3D
{
    public abstract class Attribute : IComparable<Attribute>
    {
        private static readonly List<string> types = new List<string>();

        public static long getAttributeType(string alias)
        {
            for (int i = 0; i < types.Count; i++)
                if (types[i].CompareTo(alias) == 0)
                    return 1L << i;
            return 0;
        }

        public static string getAttributeAlias(long type)
        {
            int idx = -1;
            while (type != 0 && ++idx < 63 && (((type >> idx) & 1) == 0))
                ;
            return (idx >= 0 && idx < types.Count) ? types[idx] : null;
        }

        protected static long register(string alias)
        {
            long result = getAttributeType(alias);
            if (result > 0) return result;
            types.Add(alias);
            return 1L << (types.Count - 1);
        }

        public readonly long type;

        private readonly int typeBit;

        protected Attribute(long type)
        {
            this.type    = type;
            this.typeBit = JavaUtils.numberOfTrailingZeros(type);
        }

        public abstract Attribute copy();

        protected internal bool equals(Attribute other)
        {
            return other.GetHashCode() == GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            if (!(obj is Attribute)) return false;
            var other = (Attribute) obj;
            if (this.type != other.type) return false;
            return equals(other);
        }

        public override string ToString()
        {
            return getAttributeAlias(type);
        }

        public override int GetHashCode()
        {
            return 7489 * typeBit;
        }

        public abstract int CompareTo(Attribute other);
    }
}