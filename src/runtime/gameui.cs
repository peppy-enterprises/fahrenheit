// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit.Runtime;

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

/// <summary>
///     Provides the ability to use FF X/X-2's native game UI toolkit.
///     <para/>
///     Do not interface with this module directly. Instead, implement <see cref="FhModule.render_game"/>.
/// </summary>
[FhLoad(FhGameId.FFX | FhGameId.FFX2)]
public unsafe sealed class FhGameUiModule : FhModule {

    private readonly TOMkpCrossExtMesFontLClutTypeRGBA?          _draw_delegate_x;
    private readonly TOAdpMesFontLXYZClutTypeRGBAChangeFontType? _draw_delegate_x2;

    public FhGameUiModule() {
        switch (FhGlobal.game_id) {
            case FhGameId.FFX:
                _draw_delegate_x  = FhUtil.get_fptr<TOMkpCrossExtMesFontLClutTypeRGBA>(0x501700);
                break;
            case FhGameId.FFX2:
                _draw_delegate_x2 = FhUtil.get_fptr<TOAdpMesFontLXYZClutTypeRGBAChangeFontType>(0x3A7600);
                break;
        }
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return FhCall.h_TODrawMessageWindow.hook(this, h_render_game);
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

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ] )]
    private void h_render_game() {
        FhCall.h_TODrawMessageWindow.chain_from(h_render_game).fnptr!();

        foreach (FhModuleContext module_ctx in FhApi.Mods.get_modules()) {
            module_ctx.Module.render_game();
        }

        // TODO: Render some text so that people can't easily hide their use of Fahrenheit
    }
}
