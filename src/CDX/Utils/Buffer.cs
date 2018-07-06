using System;
using System.Diagnostics;

namespace CDX.Utils
{
    public class FloatBuffer : Buffer<float>
    {
        public override bool isReadOnly()
        {
            return false;
        }


        public override bool isDirect()
        {
            return true;
        }
    }

    public abstract class Buffer<T>
    {
        protected T[] array;

        private int _mark     = -1;
        private int _position = 0;
        private int _limit;
        private int _capacity;

        public Buffer<T> position(int newPosition)
        {
            if (newPosition > _limit | newPosition < 0)
                throw createPositionException(newPosition);
            _position = newPosition;
            if (_mark > _position) _mark = -1;
            return this;
        }

        public int getLimit()
        {
            return _limit;
        }


        public int position()
        {
            return _position;
        }

        public Buffer<T> limit(int newLimit)
        {
            if (newLimit > _capacity | newLimit < 0)
                throw createLimitException(newLimit);
            _limit = newLimit;
            if (_position > _limit) _position = _limit;
            if (_mark > _limit) _mark         = -1;
            return this;
        }

        public Buffer<T> mark()
        {
            _mark = _position;
            return this;
        }

        public Buffer<T> reset()
        {
            int m = _mark;
            if (m < 0)
                throw new Exception();
            _position = m;
            return this;
        }

        public Buffer<T> clear()
        {
            _position = 0;
            _limit    = _capacity;
            _mark     = -1;
            return this;
        }

        public Buffer<T> flip()
        {
            _limit    = _position;
            _position = 0;
            _mark     = -1;
            return this;
        }

        public Buffer<T> rewind()
        {
            _position = 0;
            _mark     = -1;
            return this;
        }

        public int remaining()
        {
            return _limit - _position;
        }

        public bool hasRemaining()
        {
            return _position < _limit;
        }

        public T[] array_() => array;

        public abstract bool isReadOnly();

        //public abstract int arrayOffset();
        public abstract bool isDirect();
        //public abstract Buffer<T> slice();

        static Exception createCapacityException(int capacity)
        {
            Debug.Assert(capacity < 0, "capacity expected to be negative");
            return new Exception("capacity < 0: ("
                                 + capacity + " < 0)");
        }

        private Exception createLimitException(int newLimit)
        {
            string msg = null;

            if (newLimit > _capacity)
            {
                msg = "newLimit > capacity: (" + newLimit + " > " + _capacity + ")";
            }
            else
            {
                // assume negative
                Debug.Assert(newLimit < 0, "newLimit expected to be negative");
                msg = "newLimit < 0: (" + newLimit + " < 0)";
            }

            return new Exception(msg);
        }

        private Exception createPositionException(int newPosition)
        {
            string msg;

            if (newPosition > _limit)
            {
                msg = "newPosition > limit: (" + newPosition + " > " + _limit + ")";
            }
            else
            {
                // assume negative
                Debug.Assert(newPosition < 0, "newPosition expected to be negative");
                msg = "newPosition < 0: (" + newPosition + " < 0)";
            }

            return new Exception(msg);
        }
    }
}