using System;
using System.Collections;

namespace Yatmi.Core
{
    internal class LimitedCollection<T> : IEnumerable
    {
        private readonly T[] _arr;
        private readonly int _max;
        private readonly object _lock;
        private int _index;


        /// <summary>
        /// Creates a new instance of <see cref="LimitedCollection{T}"/>
        /// </summary>
        /// <param name="max">How many items that can be stored before the first one is overwritten</param>
        public LimitedCollection(int max)
        {
            if (max <= 10)
            {
                max = 10;
            }

            _arr = new T[max];
            _max = max;
            _lock = new object();
            _index = 0;
        }


        /// <summary>
        /// Adds a value, if the collection is full the first value is overwritten
        /// </summary>
        public void Add(T item)
        {
            lock (_lock)
            {
                if (_index + 1 > _max)
                {
                    _index = 0;
                }

                _arr[_index] = item;
                _index++;
            }
        }


        /// <summary>
        /// Finds a value
        /// </summary>
        /// <param name="matcher">Method that should return true when the item matches</param>
        public T Find(Func<T, bool> matcher)
        {
            for (int i = 0; i < _max; i++)
            {
                if (_arr[i] != null && matcher(_arr[i]))
                {
                    return _arr[i];
                }
            }

            return default;
        }


        /// <summary>
        /// Checks if an item exists in the collection
        /// </summary>
        /// <param name="needle">Item to look for</param>
        public bool Contains(T needle)
        {
            for (int i = 0; i < _max; i++)
            {
                if (_arr[i] != null && _arr[i].Equals(needle))
                {
                    return true;
                }
            }

            return false;
        }


        public IEnumerator GetEnumerator() => _arr.GetEnumerator();
    }
}
