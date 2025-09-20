// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Runtime;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void TOMkpCrossExtMesFontLClutTypeRGBA(
    uint  p1,
    byte* text,
    float x,
    float y,
    byte  color,
    byte  p6,
    byte  tint_r, byte tint_g, byte tint_b, byte tint_a,
    float scale,
    float _);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate nint TOAdpMesFontLXYZClutTypeRGBAChangeFontType(
    nint  p1,
    uint  p2,
    byte* text,
    float x,
    float y,
    nint  p6,
    nint  p7,
    nint  p8,
    nint  tint_r, nint tint_g, nint tint_b, nint tint_a,
    bool  p13);

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate void TODrawMessageWindow();

/// <summary>
///     Provides the ability to use FF X/X-2's native game UI toolkit.
///     <para/>
///     Do not interface with this module directly. Instead, implement <see cref="FhModule.render_game"/>.
/// </summary>
[FhLoad(FhGameType.FFX | FhGameType.FFX2)]
public unsafe class FhGameUiModule : FhModule {

    private readonly FhMethodHandle<TODrawMessageWindow>         _render_game;

    private readonly TOMkpCrossExtMesFontLClutTypeRGBA?          _draw_delegate_x;
    private readonly TOAdpMesFontLXYZClutTypeRGBAChangeFontType? _draw_delegate_x2;

    public FhGameUiModule() {
        FhMethodLocation location_render_game = new(0x4ABCE0, 0x391D00);

        _render_game = new(this, location_render_game, h_render_game);

        switch (FhGlobal.game_type) {
            case FhGameType.FFX:
                _draw_delegate_x = FhUtil.get_fptr<TOMkpCrossExtMesFontLClutTypeRGBA>(0x501700);
                break;
            case FhGameType.FFX2:
                _draw_delegate_x2 = FhUtil.get_fptr<TOAdpMesFontLXYZClutTypeRGBAChangeFontType>(0x3A7600);
                break;
        }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _render_game.hook();
    }

    private void draw_text_rgba(
        byte[] text,
        float  x,
        float  y,
        byte   color,
        float  scale) {
        fixed (byte* text_ptr = text)
            _draw_delegate_x?.Invoke(0, text_ptr, x, y, color, 0, 0x80, 0x80, 0x80, 0x80, scale, 0);
    }

    private void h_render_game() {
        _render_game.orig_fptr();

        foreach (FhModuleContext module_ctx in FhApi.ModController.get_modules()) {
            module_ctx.Module.render_game();
        }

        string text = $"Fahrenheit v{typeof(FhGlobal).Assembly.GetName().Version}";
        switch (FhGlobal.game_type) {
            case FhGameType.FFX:
                // In the main menu...
                if (*FFX.Globals.event_id == 0x17) {
                    // render some text so that people can't easily hide their use of Fahrenheit
                    draw_text_rgba(FhCharset.Us.to_bytes(text), 5f, 400f, 0x00, 0.65f);
                }
                break;
            case FhGameType.FFX2:
                // Main menu draws over this
                //if (*FFX2.Globals.event_id == 0x17) {
                //    fixed (byte* s = FhCharset.Us.to_bytes(text)) {
                //        _TOAdpMesFontLXYZClutTypeRGBAChangeFontType(0, 0xffffffff, s, 5f, 400f, 0x10, 0, 0, 0x00, 0x80, 0x80, 0x80, false);
                //    }
                //}
                break;
        }
    }
}
