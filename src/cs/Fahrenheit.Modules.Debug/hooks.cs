using Fahrenheit.CLRHost;
using static Fahrenheit.Modules.Debug.Delegates;

namespace Fahrenheit.Modules.Debug;

public unsafe partial class DebugModule {
    public static FhMethodHandle<FUN_00a594c0> _FUN_00a594c0;
    public static FhMethodHandle<FUN_00a5a640> _FUN_00a5a640;

    public void init_hooks() {
        const string game = "FFX.exe";

        // Sphere Grid Editor
        _FUN_00a594c0 = new FhMethodHandle<FUN_00a594c0>(this, game, 0x6594c0, render_sphere_grid);
        _FUN_00a5a640 = new FhMethodHandle<FUN_00a5a640>(this, game, 0x65a640, update_node_type_early);
    }

    public bool hook() {
        return _FUN_00a594c0.hook()
            && _FUN_00a5a640.hook()
                ;
    }

    public bool unhook() {
        return _FUN_00a594c0.unhook()
            && _FUN_00a5a640.unhook()
                ;
    }

    private static void render_sphere_grid(u8* text, i32 p2, i32 p3) {
        if (_FUN_00a594c0.try_get_original_fptr(out FUN_00a594c0? fptr)) {
            fptr.Invoke(text, p2, p3);
        }

        //SphereGridEditor.render();
    }

    private static void update_node_type_early(i32 new_node_type, i32 node_idx) {
        if (_FUN_00a5a640.try_get_original_fptr(out FUN_00a5a640? fptr)) {
            fptr.Invoke(new_node_type, node_idx);
        }

        SphereGridEditor.update_node_type();
    }
}
