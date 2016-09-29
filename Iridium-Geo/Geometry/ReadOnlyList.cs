using System.Collections;
using System.Collections.Generic;

namespace Iridium.Geo
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly List<T> _list;

        public ReadOnlyList()
        {
            _list = new List<T>();
        }

        public ReadOnlyList(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _list.Count;

        public T this[int index] => _list[index];
    }
}