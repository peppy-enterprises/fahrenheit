using System;
using System.Collections;
using System.Collections.Generic;

namespace Fahrenheit.CoreLib;

public unsafe class FhLinkedList<T> : IEnumerable<T>
        where T: unmanaged {
    private readonly T* start;
    private Func<T, nint> get_next;
    private Func<T, nint>? get_prev;

    public int length {
        get {
            int len = 0;
            for (T* current = start; get_next(*current) != 0; current = (T*)get_next(*current), len++);
            return len;
        }
    }

    public FhLinkedList(T* start, Func<T, nint> get_next) {
        this.start = start;
        this.get_next = get_next;
        this.get_prev = null;
    }

    public FhLinkedList(T* start, Func<T, nint> get_next, Func<T, nint>? get_prev) {
        this.start = start;
        this.get_next = get_next;
        this.get_prev = get_prev;
    }

    public T this[int i] {
        get {
            T* current = start;
            for (; i > 0; i--) {
                if (get_next(*current) == 0) {
                    throw new IndexOutOfRangeException();
                }
                current = (T*)get_next(*current);
            }

            return *current;
        }
    }

    public IEnumerator<T> GetEnumerator() {
        return new Enumerator<T>(start, get_next);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public class Enumerator<T> : IEnumerator<T>
            where T: unmanaged {
        private readonly T* start;
        private T* current;
        private Func<T, nint> get_next;

        public Enumerator(T* start, Func<T, nint> get_next) {
            this.start = current = start;
            this.get_next = get_next;
        }

        public T Current => *current;
        object IEnumerator.Current => Current!;

        public void Dispose() { }

        public bool MoveNext() {
            current = (T*)get_next(*current);
            return current != null;
        }

        public void Reset() {
            current = start;
        }
    }
}
