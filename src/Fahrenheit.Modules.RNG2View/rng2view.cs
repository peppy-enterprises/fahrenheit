using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Fahrenheit.CoreLib;
using Fahrenheit.CoreLib.FFX;

namespace Fahrenheit.Modules.RNGFix;

public sealed record RNG2ViewModuleConfig : FhModuleConfig {
    [JsonConstructor]
    public RNG2ViewModuleConfig(string configName,
                              uint   configVersion,
                              bool   configEnabled) : base(configName, configVersion, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new RNG2ViewModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe class RNG2ViewModule : FhModule {
    public override RNG2ViewModuleConfig ModuleConfig { get; }
    private static FhMethodHandle<brnd> _brnd;
    private static FhMethodHandle<render> _render;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint brnd(nint rng_seed_idx);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void render();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TOMkpCrossExtMesFontLClutTypeRGBADelegate(
            u32 p1,
            u8 *text,
            f32 x, f32 y,
            u8 color,
            u8 p6,
            u8 tint_r, u8 tint_g, u8 tint_b, u8 tint_a,
            f32 scale,
            f32 _);

    public static void TOMkpCrossExtMesFontLClutTypeRGBA(
            u32 p1,
            u8[] text,
            f32 x, f32 y,
            u8 color,
            u8 p6,
            u8 tint_r, u8 tint_g, u8 tint_b, u8 tint_a,
            f32 scale,
            f32 _) {
        fixed (u8 *_text = text)
        FhUtil.get_fptr<TOMkpCrossExtMesFontLClutTypeRGBADelegate>(0x501700)(p1, _text, x, y, color, p6, tint_r, tint_g, tint_b, tint_a, scale, _);
    }

    public static void TOMkpCrossExtMesFontLClut(
            u32 p1,
            u8[] text,
            f32 x, f32 y,
            u8 color,
            u8 p6,
            f32 scale,
            f32 _) {
        TOMkpCrossExtMesFontLClutTypeRGBA(p1, text, x, y, color, p6, 0x80, 0x80, 0x80, 0x80, scale, _);
    }

    private nint[] p = new nint[20];

    public RNG2ViewModule(RNG2ViewModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;

        _brnd = new(this, "FFX.exe", 0x398900, h_brnd);
        _render = new(this, "FFX.exe", 0x4963C0, h_render);

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override bool FhModuleStart() {
        return _brnd.hook();
    }

    public nint h_brnd(nint rng_seed_idx) {
        nint ret_value = _brnd.original!.Invoke(rng_seed_idx);
        if (rng_seed_idx == 2) update_predictions();
        return ret_value;
    }

    private unsafe void update_predictions() {
        int seed1 = *(int*)(FhGlobal.base_addr + 0x8421F8 + 0x8);
        int seed2 = *(int*)(FhGlobal.base_addr + 0x842308 + 0x8);
        int array = *(int*)(FhGlobal.base_addr + 0xD35ED8 + 0x8);

        for (int i = 0; i < p.Length; i++) {
            int r = seed1 * array ^ seed2;
            r = (r >> 0x10) + r * 0x10000;
            array = r;
            p[i] = r & 0x7FFFFFFF;
        }
    }

    private unsafe void h_render() {
        _render.original!.Invoke();

        nint[] pm2 = new nint[20];
        nint[] pm3 = new nint[20];
        nint[] pm4 = new nint[20];

        for (int i = 0; i < p.Length; i++) {
            pm2[i] = p[i] % 2;
            pm3[i] = p[i] % 3;
            pm4[i] = p[i] % 4;
        }

        byte[] bpm2 = FhCharset.Us.to_bytes(String.Join(' ', pm2));
        byte[] bpm3 = FhCharset.Us.to_bytes(String.Join(' ', pm3));
        byte[] bpm4 = FhCharset.Us.to_bytes(String.Join(' ', pm4));

        TOMkpCrossExtMesFontLClut(0, bpm2, 60f, 60f, 0x00, 0, 0.69f, 0);
    }
}
