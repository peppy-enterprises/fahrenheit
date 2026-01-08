// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

/// <summary>
///     Implements the 'local state' mechanism of Fahrenheit.
///     <para/>
///     'Local state' is a unique file for each save that <see cref="FhModule"/>s have access to at save/load time.
///     This allows information unique to that save game to be persisted to disk.
///     <para/>
///     Do not interface with this module directly. Instead, implement:
///     <br/> - <see cref="FhModule.load_local_state(FileStream, FhLocalStateInfo)"/>
///     <br/> - <see cref="FhModule.save_local_state(FileStream)"/>
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2 | FhGameId.FFX2LM)]
public unsafe sealed class FhLocalStateModule : FhModule {

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return true;
    }

    /// <summary>
    ///     For the given save <paramref name="slot"/> and given module's <paramref name="module_context"/>,
    ///     returns the path of the local state directory and creates it.
    /// </summary>
    private static string _get_state_dir_path(FhModuleContext module_context, int slot) {
        string local_state_dir = Path.Join(
            module_context.Paths.LocalStateDir.FullName,
            FhSaveManager.get_save_name_for_slot(slot));

        Directory.CreateDirectory(local_state_dir);
        return local_state_dir;
    }

    /// <summary>
    ///     Loads all modules' local state from the selected save <paramref name="slot"/>.
    /// </summary>
    internal void state_load_slot(int slot) {
        foreach (FhModuleContext module_context in FhApi.Mods.get_modules()) {
            string module_type   = module_context.Module.ModuleType;
            string state_dir     = _get_state_dir_path(module_context, slot);

            string state_fn      = Path.Join(state_dir, module_type);
            string state_meta_fn = Path.Join(state_dir, $"{module_type}.meta.json");

            if (!File.Exists(state_fn)) continue;

            using FileStream state_meta_fs = File.Open(state_meta_fn, FileMode.Open,         FileAccess.Read, FileShare.None);
            using FileStream state_fs      = File.Open(state_fn,      FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);

            _logger.Info($"Loading local state metadata for module {module_type}.");
            FhLocalStateInfo state_meta = JsonSerializer.Deserialize<FhLocalStateInfo>(state_meta_fs, FhUtil.InternalJsonOpts)
                ?? throw new Exception("FH_E_LOCAL_STATE_META_BLOCK_NULL");

            _logger.Info($"Loading local state for module {module_type}.");
            module_context.Module.load_local_state(state_fs, state_meta);
        }
    }

    /// <summary>
    ///     Saves all modules' local state to the selected save <paramref name="slot"/>.
    /// </summary>
    internal void state_save_slot(int slot) {
        foreach (FhModContext mod_context in FhApi.Mods.get_mods()) {
            foreach (FhModuleContext module_context in mod_context.Modules) {
                string module_type   = module_context.Module.ModuleType;
                string state_dir     = _get_state_dir_path(module_context, slot);

                string state_fn      = Path.Join(state_dir, module_type);
                string state_meta_fn = Path.Join(state_dir, $"{module_type}.meta.json");

                FhLocalStateInfo state_meta = new(mod_context.Manifest.Version);

                using FileStream state_meta_fs = File.Open(state_meta_fn, FileMode.Create,       FileAccess.Write, FileShare.None);
                using FileStream state_fs      = File.Open(state_fn,      FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

                _logger.Info($"Saving local state metadata for module {module_type}.");
                state_meta_fs.Write(JsonSerializer.SerializeToUtf8Bytes(state_meta, FhUtil.InternalJsonOpts));

                _logger.Info($"Saving local state for module {module_type} to {state_fn}.");
                module_context.Module.save_local_state(state_fs);
            }
        }
    }
}
