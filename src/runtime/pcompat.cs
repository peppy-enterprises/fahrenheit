// SPDX-License-Identifier: MIT

namespace Fahrenheit.Runtime;


[StructLayout(LayoutKind.Sequential)]
internal unsafe readonly struct PSimpleDoubleListElement<T> where T : unmanaged {
    public readonly PSimpleDoubleListElement<T>* m_next;
    public readonly PSimpleDoubleListElement<T>* m_prev;

    public readonly T* next(PSimpleDoubleListElement<T>* head) {
        if (m_next == head) return null;
        return (T*)m_next;
    }

    public readonly T* prev(PSimpleDoubleListElement<T>* head) {
        if (m_prev == head) return null;
        return (T*)m_prev;
    }
}

