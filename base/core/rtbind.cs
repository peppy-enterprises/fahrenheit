using System.Threading;

namespace Fahrenheit.Core;

/// <summary>
///     Represents an object of type <typeparamref name="T"/> initialized at runtime.
/// </summary>
internal class FhRuntimeBinding<T> {
    protected readonly Lock _impl_lock;
    protected          T?   _impl;

    public FhRuntimeBinding() {
        _impl_lock = new Lock();
    }

    public bool get_impl([NotNullWhen(true)] out T? impl) {
        lock (_impl_lock) {
            return (impl = _impl) != null;
        }
    }

    public void set_impl(T impl) {
        lock (_impl_lock) {
            FhInternal.Log.Info(typeof(T).Name);
            _impl = impl;
        }
    }
}
