using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using Fahrenheit.CoreLib;

namespace Fahrenheit.Modules.TrueRNG;

/* [fkelava 9/9/24 22:26]
 * Modules in Fahrenheit are instantiated through their configurations.
 *
 * Configuration objects are loaded in JSON form from disk. You are free to include any fields you desire as long as they are JSON-serializable.
 *
 * Every module _must_ have an associated configuration, even if you do not plan to include any custom fields.
 */

public sealed record TrueRNGModuleConfig : FhModuleConfig {
    [JsonConstructor]
    public TrueRNGModuleConfig(string configName,
                               bool   configEnabled) : base(configName, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new TrueRNGModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

/* [fkelava 9/9/24 22:26]
 * Every function you intend to hook or invoke must have an associated _delegate_ declared, i.e. a function pointer typedef.
 *
 * They must be annotated with [UnmanagedFunctionPointer(CallingConvention.YOUR_CALLING_CONVENTION_HERE)],
 * and have a matching signature to the function you are trying to hook or invoke.
 *
 * The signature is a union of the parameter list, calling convention, and return type. One delegate fits _all_ functions with the same signature.
 *
 * The name of the delegate is left free for you to declare. Here I chose to use the function's original name.
 *
 * Given the function signature
 * > uint __cdecl brnd(int param_1)
 *
 * the correct declaration is:
 * > [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
 * > public delegate uint brnd(int param_1);
 */

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate uint brnd(int param_1);

public class TrueRNGModule : FhModule {
    /* [fkelava 9/9/24 22:26]
     * Every function you intend to hook or invoke must have an associated _method handle_ declared.
     *
     * Declare them as follows:
     * > private readonly FhMethodHandle<DELEGATE_TYPE_HERE> _handle;
     *
     * Initialize them in the constructor as follows:
     * > _handle = new FhMethodHandle<DELEGATE_TYPE_NAME>(this, EXE_OR_DLL_NAME, new DELEGATE_TYPE_NAME(HOOK_FUNCTION_NAME), [opt] OFFSET_IN_EXE_OR_DLL, [opt] NAME_OF_FUNCTION_IN_EXE_OR_DLL);
     *
     * where one of the last two parameters is required, whichever one; where HOOK_FUNCTION_NAME is the name of a function whose signature matches the delegate DELEGATE_TYPE_NAME.
     */
    private readonly TrueRNGModuleConfig  _module_config;
    private readonly FhMethodHandle<brnd> _brnd_handle;

    public TrueRNGModule(TrueRNGModuleConfig moduleConfig) : base(moduleConfig) {
        _module_config = moduleConfig;
        _brnd_handle   = new FhMethodHandle<brnd>(this, "FFX.exe", new brnd(h_brnd), offset: 0x398900);
        _moduleState   = FhModuleState.InitSuccess;
    }

    public uint h_brnd(int param_1) {
        /* [fkelava 9/9/24 22:26]
         * Call the original function of a method handle as follows:
         * > _handle.orig_fptr.Invoke(...);
         *
         * DO NOT store the value of _handle.orig_fptr across _handle.hook() calls, as _handle.hook() mutates it!
         * DO NOT issue calls through orig_fptr concurrently with a _handle.hook() call!
         */
        return _brnd_handle.orig_fptr.Invoke(0);
    }

    public override bool FhModuleInit() {
        /* [fkelava 9/9/24 22:26]
         * Hook the target function of a method handle as follows:
         * > bool hook_successful = _handle.hook();
         *
         * DO NOT store the value of _handle.orig_fptr across _handle.hook() calls, as _handle.hook() mutates it!
         * DO NOT issue calls through orig_fptr concurrently with a _handle.hook() call!
         */

        return _brnd_handle.hook();
    }

    public override bool FhModuleOnError() {
        return true;
    }
}
