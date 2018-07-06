using System;
using System.Collections;
using System.Collections.Generic;

namespace CDX.Graphics.G3D
{
    public class Attributes : IComparable<Attributes>, IComparer<Attribute>, IEnumerable<Attribute>
    {
        protected          long            mask;
        protected readonly List<Attribute> attributes = new List<Attribute>();
        protected          bool            sorted = true;

        public void sort()
        {
            if (!sorted)
            {
                attributes.Sort(this);
                sorted = true;
            }
        }

        public long getMask() => mask;

        public Attribute get(long type)
        {
            if (has(type))
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (attributes[i].type == type) return attributes[i];
                }

            return null;
        }

        public T get<T>(long type) where T : Attribute
        {
            return (T) get(type);
        }

        public void clear()
        {
            mask = 0;
            attributes.Clear();
        }

        public int size() => attributes.Count;

        private void enable(long mask)
        {
            this.mask |= mask;
        }

        private void disable(long mask)
        {
            this.mask &= ~mask;
        }

        public void set(Attribute attribute)
        {
            var idx = indexOf(attribute.type);
            if (idx < 0)
            {
                enable(attribute.type);
                attributes.Add(attribute);
                sorted = false;
            }
            else
            {
                attributes[idx] = attribute;
            }

            sort(); //FIXME: See #4186
        }

        public void set(Attribute attribute1, Attribute attribute2)
        {
            set(attribute1);
            set(attribute2);
        }

        public void set(IEnumerable<Attribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                set(attribute);
            }
        }

        public void remove(long mask)
        {
            for (int i = attributes.Count - 1; i >= 0; i--)
            {
                long type = attributes[i].type;
                if ((mask & type) == type)
                {
                    attributes.RemoveAt(i);
                    disable(type);
                    sorted = false;
                }
            }

            sort(); //FIXME: See #4186
        }

        public bool has(long type)
        {
            return type != 0 && (this.mask & type) == type;
        }

        protected int indexOf(long type)
        {
            if (has(type))
                for (int i = 0; i < attributes.Count; i++)
                    if (attributes[i].type == type)
                        return i;
            return -1;
        }

        public bool same(Attributes other, bool compareValues)
        {
            if (other == this) return true;
            if ((other == null) || (mask != other.mask)) return false;
            if (!compareValues) return true;
            sort();
            other.sort();
            for (int i = 0; i < attributes.Count; i++)
                if (!attributes[i].equals(other.attributes[i]))
                    return false;
            return true;
        }

        public bool same(Attributes other)
        {
            return same(other, false);
        }

        public int Compare(Attribute arg0, Attribute arg1)
        {
            return (int) (arg0.type - arg1.type);
        }

        public int attributesHash()
        {
            sort();
            int  n      = attributes.Count;
            long result = 71 + mask;
            int  m      = 1;
            for (int i = 0; i < n; i++)
                result += mask * attributes[i].GetHashCode() * (m = (m * 7) & 0xFFFF);
            return (int) (result ^ (result >> 32));
        }

        public override int GetHashCode()
        {
            return attributesHash();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        public override bool Equals(object other)
        {
            if (!(other is Attributes)) return false;
            if (other == this) return true;
            return same((Attributes) other, true);
        }

        public int CompareTo(Attributes other)
        {
            if (other == this)
                return 0;
            if (mask != other.mask)
                return mask < other.mask ? -1 : 1;
            sort();
            other.sort();
            for (int i = 0; i < attributes.Count; i++)
            {
                int c = attributes[i].CompareTo(other.attributes[i]);
                if (c != 0)
                    return c < 0 ? -1 : (c > 0 ? 1 : 0);
            }

            return 0;
        }
    }
}