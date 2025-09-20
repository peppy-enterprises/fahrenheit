// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/* [fkelava 6/6/23 21:29]
 * Vararg P/Invokes must be declared assembly-local due to https://github.com/dotnet/runtime/issues/87188.
 */

/// <summary>
///     Contains non-game, i.e. OS or library native invocations used by Fahrenheit.
/// </summary>
internal static unsafe partial class FhPInvoke {
    /// <summary>
    ///     Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
    /// </summary>
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandle(string? lpModuleName);

#if DEBUG
    private const string hook_lib_name = "minhook.x32d.dll";
#else
    private const string hook_lib_name = "minhook.x32.dll";
#endif

    /// <summary>
    ///     Creates a hook for the specified target function, in disabled state.
    /// </summary>
    /// <param name="pTarget">A pointer to the target function, which will be overridden by the detour function.</param>
    /// <param name="pDetour">A pointer to the detour function, which will override the target function.</param>
    /// <param name="ppOriginal">A pointer to the trampoline function, which will be used to call the original target function. This parameter can be NULL.</param>
    [LibraryImport(hook_lib_name)]
    internal static partial int MH_CreateHook(
        nint  pTarget,
        nint  pDetour,
        nint* ppOriginal);

    /// <summary>
    ///     Enables an already created hook.
    /// </summary>
    /// <param name="pTarget">A pointer to the target function. If this parameter is MH_ALL_HOOKS, all created hooks are enabled in one go.</param>
    [LibraryImport(hook_lib_name)]
    internal static partial int MH_EnableHook(nint pTarget);
}
