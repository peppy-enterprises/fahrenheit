using System;

namespace Fahrenheit.CoreLib;

public unsafe readonly ref struct FhMemoryMonitor<T> where T : unmanaged
{
    private readonly FhPointer _ptr;

    public FhMemoryMonitor(FhPointer ptr)
    {
        _ptr = ptr;
    }

    public bool AwaitValue(T target)
    {
        if (!_ptr.DerefOffsets(out nint vptr))
        {
            FhLog.Log(LogLevel.Warning, $"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        void* maptr = vptr.ToPointer(); // Void pointer to monitored addr.
        T     maval = *(T*)maptr;       // Value at monitored addr at call start.

        if (maval.Equals(target))
            return true;

        /* [fkelava 26/6/23 11:03]
         * See https://devblogs.microsoft.com/oldnewthing/20180118-00/?p=97825,
         * https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitonaddress.
         */
        T  cur    = maval;
        T* curptr = &cur;

        while (!cur.Equals(target))
        {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), 1); 
            cur = *(T*)maptr;
        }
        
        return true;
    }

    public bool AwaitValue(in ReadOnlySpan<T> targets)
    {
        if (!_ptr.DerefOffsets(out nint vptr))
        {
            FhLog.Log(LogLevel.Warning, $"AwaitValue was called but the supplied pointer resolved to nothing.");
            return false;
        }

        void* maptr = vptr.ToPointer(); // Void pointer to monitored addr.
        T     maval = *(T*)maptr;       // Value at monitored addr at call start.

        foreach (T target in targets)
        {
            if (maval.Equals(target))
                return true;
        }

        /* [fkelava 26/6/23 11:03]
         * See https://devblogs.microsoft.com/oldnewthing/20180118-00/?p=97825,
         * https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitonaddress.
         */
        T    cur    = maval;
        T*   curptr = &cur;
        bool match  = false;

        while (!match)
        {
            FhPInvoke.WaitOnAddress(curptr, maptr, sizeof(T), 1); 
            cur = *(T*)maptr;

            foreach (T target in targets)
            {
                match = cur.Equals(target);
                if (match) break;
            }
        }
        
        return true;
    }
}
