// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void FUN_002F0650_autosave(nint arg1, nint arg2);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate nint FUN_002F0DA0_list(nint arg1, nint arg2);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public unsafe delegate void FUN_002F01B0_load(int menu_selection_index);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public unsafe delegate void FUN_002F09A0_save(int menu_selection_index);

internal readonly struct FhSaveListEntry {
    public readonly int index;
    public readonly int _0x04;
    public readonly int _0x08;
}

/// <summary>
///     Implements the 'local state' and 'separate saves' mechanisms of Fahrenheit.
///     <para/>
///     'Local state' is a unique file for each save that <see cref="FhModule"/>s have access to at save/load time.
///     This allows information unique to that save game to be persisted to disk.
///     <para/>
///     'Separate saves' is a mechanism by which mods can restrict the visibility of save games
///     made using them. If such a mod is no longer loaded, these saves will not appear in the load menu.
///     <para/>
///     Do not interface with this module directly. Instead, implement:
///     <br/> - <see cref="FhModule.load_local_state(FileStream, FhLocalStateInfo)"/>
///     <br/> - <see cref="FhModule.save_local_state(FileStream)"/>
/// </summary>
[FhLoad(FhGameType.FFX | FhGameType.FFX2)]
public unsafe class FhLocalStateModule : FhModule {
    private readonly FhMethodHandle<FUN_002F01B0_load>     _handle_onload;
    private readonly FhMethodHandle<FUN_002F09A0_save>     _handle_onsave;
    private readonly FhMethodHandle<FUN_002F0650_autosave> _handle_onautosave;
    private readonly FhMethodHandle<FUN_002F0DA0_list>     _handle_onlist;

    public FhLocalStateModule() {
        FhMethodLocation location_onload     = new(0x2F01B0, 0x11D040);
        FhMethodLocation location_onsave     = new(0x2F09A0, 0x11D880);
        FhMethodLocation location_onautosave = new(0x2F0650, 0x11D510);
        FhMethodLocation location_onlist     = new(0x2F0DA0, 0x11DC50);

        _handle_onload     = new(this, location_onload,     h_onload);
        _handle_onsave     = new(this, location_onsave,     h_onsave);
        _handle_onautosave = new(this, location_onautosave, h_onautosave);
        _handle_onlist     = new(this, location_onlist,     h_onlist);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _handle_onload    .hook()
            && _handle_onsave    .hook()
            && _handle_onautosave.hook()
            && _handle_onlist    .hook();
    }

    private string _get_state_dir_path_save(FhModContext mod_context, int menu_selection_index) {
        if (menu_selection_index != 0) return _get_state_dir_path_load(mod_context, menu_selection_index);

        // a list of 200 entries, 1 indicating the slot is used, -1 indicating it is unused
        ReadOnlySpan<int> used_slots_list   = FhGlobal.game_type switch {
            FhGameType.FFX  => new ReadOnlySpan<int>(FhUtil.ptr_at<nint>(0x8E7C68), 200),
            FhGameType.FFX2 => new ReadOnlySpan<int>(FhUtil.ptr_at<nint>(0x9ECD30), 200),
        };

        int actual_slot_index = 0;
        for (; actual_slot_index < 200; actual_slot_index++) {
            if (used_slots_list[actual_slot_index] == -1) break;
        }

        string local_state_dir = Path.Join(
            mod_context.Paths.StateDir.FullName,
            FhInternal.PathFinder.get_save_name_for_index(actual_slot_index));

        Directory.CreateDirectory(local_state_dir);
        return local_state_dir;
    }

    private string _get_state_dir_path_load(FhModContext mod_context, int menu_selection_index) {
        if (menu_selection_index < 0) menu_selection_index++; // game itself does this (?)

        // map the index in the save slot selector to the actual _index_ in the save filename
        ReadOnlySpan<FhSaveListEntry> save_list     = FhGlobal.game_type switch {
            FhGameType.FFX  => new ReadOnlySpan<FhSaveListEntry>(FhUtil.ptr_at<nint>(0x8E7308), 200),
            FhGameType.FFX2 => new ReadOnlySpan<FhSaveListEntry>(FhUtil.ptr_at<nint>(0x9EC3D0), 200),
        };
        FhSaveListEntry               selected_save = save_list[menu_selection_index];

        string local_state_dir = Path.Join(
            mod_context.Paths.StateDir.FullName,
            FhInternal.PathFinder.get_save_name_for_index(selected_save.index));

        Directory.CreateDirectory(local_state_dir);
        return local_state_dir;
    }

    private void h_onautosave(nint arg1, nint arg2) {
        _handle_onautosave.orig_fptr(arg1, arg2);
    }

    private nint h_onlist(nint arg1, nint arg2) {
        return _handle_onlist.orig_fptr(arg1, arg2);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
    private void h_onsave(int menu_selection_index) {
        foreach (FhModContext mod_context in FhApi.ModController.get_mods()) {
            foreach (FhModuleContext module_context in mod_context.Modules) {
                string module_type   = module_context.Module.ModuleType;
                string state_dir     = _get_state_dir_path_save(mod_context, menu_selection_index);

                string state_fn      = Path.Join(state_dir, module_type);
                string state_meta_fn = Path.Join(state_dir, $"{module_type}.meta.json");

                FhLocalStateInfo state_meta = new(mod_context.Manifest.Version);

                _logger.Info($"Saving local state metadata for module {module_type}.");
                using (FileStream   state_meta_fs = File.Open(state_meta_fn, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter writer        = new StreamWriter(state_meta_fs, Encoding.UTF8)) {
                    writer.Write(JsonSerializer.Serialize(state_meta, FhUtil.InternalJsonOpts));
                }

                _logger.Info($"Saving local state for module {module_type} to {state_fn}.");
                using (FileStream state_fs = File.Open(state_fn, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {
                    _logger.Info($"{module_type} -> {state_fn}");
                    module_context.Module.save_local_state(state_fs);
                }
            }
        }

        _handle_onsave.orig_fptr(menu_selection_index);
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
    private void h_onload(int menu_selection_index) {
        foreach (FhModContext mod_context in FhApi.ModController.get_mods()) {
            foreach (FhModuleContext module_context in mod_context.Modules) {
                string module_type   = module_context.Module.ModuleType;
                string state_dir     = _get_state_dir_path_load(mod_context, menu_selection_index);

                string state_fn      = Path.Join(state_dir, module_type);
                string state_meta_fn = Path.Join(state_dir, $"{module_type}.meta.json");

                if (!File.Exists(state_fn)) continue;

                FhLocalStateInfo? state_meta;

                _logger.Info($"Loading local state metadata for module {module_type}.");
                using (FileStream state_meta_fs = File.Open(state_meta_fn, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    state_meta = JsonSerializer.Deserialize<FhLocalStateInfo>(state_meta_fs, FhUtil.InternalJsonOpts)
                        ?? throw new Exception("FH_E_LOCAL_STATE_META_BLOCK_NULL");
                }

                _logger.Info($"Loading local state for module {module_type}.");
                using (FileStream state_fs = File.Open(state_fn, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)) {
                    _logger.Info($"{module_type} -> {state_fn}");
                    module_context.Module.load_local_state(state_fs, state_meta!);
                }
            }
        }

        _handle_onload.orig_fptr(menu_selection_index);
    }
}
