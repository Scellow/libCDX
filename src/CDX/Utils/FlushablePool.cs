using System.Collections.Generic;

namespace CDX.Utils
{
    public abstract class FlushablePool<T> : Pool<T>
    {
        protected List<T> obtained = new List<T>();
        
        public FlushablePool(){}
        public FlushablePool(int capacity = 16, int max = int.MaxValue) : base(capacity, max){}

        public override T obtain()
        {
            T result = base.obtain();
            obtained.Add(result);
            return result;
        }
        
        public void flush () {
            base.freeAll(obtained);
            obtained.Clear();
        }

        public override void free(T objectz)
        {
            obtained.Remove(objectz);
            base.free(objectz);
        }

        public override void freeAll(IList<T> objects)
        {
            foreach (var o in objects)
            {
                obtained.Remove(o);
            }
            
            base.freeAll(objects);
        }
    }
}