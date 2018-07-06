using System;
using System.Collections.Generic;

namespace CDX.Utils
{
    public abstract class Pool<T>
    {
        public readonly  int      max;
        public           int      peak;
        private readonly Queue<T> _freeObjects;

        public Pool(int initialCapacity = 16, int max = int.MaxValue)
        {
            _freeObjects = new Queue<T>(initialCapacity);
            this.max     = max;
        }

        protected abstract T newObject();

        public virtual T obtain()
        {
            return _freeObjects.Count == 0 ? newObject() : _freeObjects.Dequeue();
        }

        public virtual void free(T objectz)
        {
            if (objectz == null) throw new Exception("object cannot be null.");
            if (_freeObjects.Count < max)
            {
                _freeObjects.Enqueue(objectz);
                peak = Math.Max(peak, _freeObjects.Count);
            }

            reset(objectz);
        }

        public virtual void freeAll(IList<T> objects)
        {
            if (objects == null) throw new Exception("objects cannot be null");
            var freeObjects = _freeObjects;
            int max         = this.max;
            for (int i = 0; i < objects.Count; i++)
            {
                T o = objects[i];
                if (o == null) continue;
                if (freeObjects.Count < max) freeObjects.Enqueue(o);
                reset(o);
            }

            peak = Math.Max(peak, freeObjects.Count);
        }

        protected void reset(T objectz)
        {
            if (objectz is IPoolable) ((IPoolable) objectz).reset();
        }


        public void clear()
        {
            while (_freeObjects.Count != 0)
            {
                _freeObjects.Dequeue();
            }
        }

        public int getFree()
        {
            return _freeObjects.Count;
        }
    }

    public interface IPoolable
    {
        void reset();
    }
}