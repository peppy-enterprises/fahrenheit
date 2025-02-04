using System.Collections;

namespace Fahrenheit.Core;

public unsafe class FhLinkedList<T> : IEnumerable<T> where T : unmanaged {
    private readonly T*             _start;
    private readonly Func<T, nint>  _get_next;
    private readonly Func<T, nint>? _get_prev;

    public FhLinkedList(T* start, Func<T, nint> get_next) {
        _start    = start;
        _get_next = get_next;
        _get_prev = null;
    }

    public FhLinkedList(T* start, Func<T, nint> get_next, Func<T, nint>? get_prev) {
        _start    = start;
        _get_next = get_next;
        _get_prev = get_prev;
    }

    public int length {
        get {
            int len = 0;
            for (T* current = _start; _get_next(*current) != 0; current = (T*)_get_next(*current), len++);
            return len;
        }
    }

    public T this[int i] {
        get {
            T* current = _start;

            for (; i > 0; i--) {
                if (_get_next(*current) == 0) throw new IndexOutOfRangeException();
                current = (T*)_get_next(*current);
            }

            return *current;
        }
    }

    public IEnumerator<T> GetEnumerator() {
        return new Enumerator(_start, _get_next);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public class Enumerator : IEnumerator<T> {
        private readonly T*            _start;
        private          T*            _current;
        private readonly Func<T, nint> _get_next;

        public Enumerator(T* start, Func<T, nint> get_next) {
            _start = _current = start;
            _get_next = get_next;
        }

        public T           Current => *_current;
        object IEnumerator.Current => Current!;

        public void Dispose() { }

        public bool MoveNext() {
            _current = (T*)_get_next(*_current);
            return _current != null;
        }

        public void Reset() {
            _current = _start;
        }
    }
}
