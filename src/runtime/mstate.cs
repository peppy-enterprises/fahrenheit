using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fahrenheit.Core.Runtime;

// todo 2F0650 AUTOSAVE 2F0DA0 LISTALL

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public unsafe delegate void FUN_002F01B0_load(int save_list_idx);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public unsafe delegate void FUN_002F09A0_save(int save_list_idx);

internal struct FhSaveListEntry {
    public readonly int index;
    public readonly int _0x04;
    public readonly int _0x08;
}

[FhLoad(FhGameType.FFX)]
public unsafe class FhSaveLifecycleModule : FhModule {
    private readonly FhMethodHandle<FUN_002F01B0_load> _handle_onload;
    private readonly FhMethodHandle<FUN_002F09A0_save> _handle_onsave;

    public FhSaveLifecycleModule() {
        _handle_onload = new(this, "FFX.exe", h_onload, offset: 0x2F01B0);
        _handle_onsave = new(this, "FFX.exe", h_onsave, offset: 0x2F09A0);
    }

    public override bool init(FileStream global_state_file) {
        return _handle_onload.hook()
            && _handle_onsave.hook();
    }

    private string _get_state_fn_for_index(FhModContext mod_context, string file_name, int index) {
        if (index < 0) index++; // game itself does this (?)

        // map the index in the save slot selector to the actual _index_ in the save filename
        ReadOnlySpan<FhSaveListEntry> save_list     = new ReadOnlySpan<FhSaveListEntry>(FhUtil.ptr_at<nint>(0x8E7308), 200);
        FhSaveListEntry               selected_save = save_list[index];

        string local_state_dir = Path.Join(
            mod_context.Paths.StateDir.FullName,
            FhInternal.PathFinder.get_save_name_for_index(selected_save.index));

        Directory.CreateDirectory(local_state_dir);

        return Path.Join(local_state_dir, $"{file_name}.state.json");
    }

    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    private void h_onsave(int save_list_idx) {
        foreach (FhModContext mod_context in FhInternal.ModController.get_all()) {
            foreach (FhModuleContext module_context in mod_context.Modules) {
                string module_type = module_context.Module.ModuleType;
                string state_fn    = _get_state_fn_for_index(mod_context, module_type, save_list_idx);

                using (FileStream state_fs = File.Open(state_fn, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                    _logger.Info($"{module_type} -> {state_fn}");
                    module_context.Module.save_local_state(state_fs);
                }
            }
        }

        _handle_onsave.orig_fptr(save_list_idx);
    }

    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    public void h_onload(int save_list_idx) {
        foreach (FhModContext mod_context in FhInternal.ModController.get_all()) {
            foreach (FhModuleContext module_context in mod_context.Modules) {
                string module_type = module_context.Module.ModuleType;
                string state_fn    = _get_state_fn_for_index(mod_context, module_type, save_list_idx);

                if (!File.Exists(state_fn)) continue;

                using (FileStream state_fs = File.Open(state_fn, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                    _logger.Info($"{module_type} -> {state_fn}");
                    module_context.Module.load_local_state(state_fs);
                }
            }
        }

        _handle_onload.orig_fptr(save_list_idx);
    }
}
