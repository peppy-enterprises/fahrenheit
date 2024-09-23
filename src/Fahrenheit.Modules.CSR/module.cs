global using System;
global using Fahrenheit.CoreLib.FFX;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Fahrenheit.CoreLib;

namespace Fahrenheit.Modules.CSR;

public sealed record CSRModuleConfig : FhModuleConfig {
    [JsonConstructor]
    public CSRModuleConfig(string configName,
                           uint   configVersion,
                           bool   configEnabled)
            : base(configName, configVersion, configEnabled) {
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new CSRModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe class CSRModule : FhModule {
    private readonly CSRModuleConfig _moduleConfig;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelEventSetUp(u32 event_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate char* AtelGetEventName(u32 event_id);

    public static char* get_event_name(u32 event_id)
        => FhUtil.get_fptr<AtelGetEventName>(0x4796e0)(event_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TOMkpCrossExtMesFontLClutTypeRGBA(
            u32 p1,
            u8 *text,
            f32 x, f32 y,
            u8 color,
            u8 p6,
            u8 tint_r, u8 tint_g, u8 tint_b, u8 tint_a,
            f32 scale,
            f32 _);

    public static void draw_text_rgba(
            u32 p1,
            u8[] text,
            f32 x, f32 y,
            u8 color,
            u8 p6,
            u8 tint_r, u8 tint_g, u8 tint_b, u8 tint_a,
            f32 scale,
            f32 _) {
        fixed (u8 *_text = text)
        FhUtil.get_fptr<TOMkpCrossExtMesFontLClutTypeRGBA>(0x501700)(p1, _text, x, y, color, p6, tint_r, tint_g, tint_b, tint_a, scale, _);
    }

    public static void draw_text(
            u32 p1,
            u8[] text,
            f32 x, f32 y,
            u8 color,
            u8 p6,
            f32 scale,
            f32 _) {
        draw_text_rgba(p1, text, x, y, color, p6, 0x80, 0x80, 0x80, 0x80, scale, _);
    }

    private static FhMethodHandle<AtelEventSetUp> _csr_event;

    public delegate void CsrEvent(u8* code_ptr);

    public static Dictionary<string, CsrEvent> removers = new();
    public static List<KeyValuePair<Func<bool>, Action>> predicates = new();

    public CSRModule(CSRModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;

        _csr_event = new(this, "FFX.exe", 0x472e90, csr_event);

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override bool FhModuleInit() {
        Removers.init();
        return _csr_event.hook();
    }

    public override FhModuleConfig ModuleConfig { get => _moduleConfig; }

    public override void render_game() {
        // Try rendering something I guess?
        draw_text(0, FhCharset.Us.to_bytes("CSR is running!"), x: 460, y: 5, color: 0x00, 0, scale: 0.5f, 0);
    }

    public override void post_update() {
        foreach (var pair in predicates) {
            if (pair.Key()) pair.Value();
        }
    }

    public void csr_event(u32 event_id) {
        _csr_event.original(event_id);

        string event_name = Marshal.PtrToStringAnsi((isize)get_event_name(event_id))!;
        if (removers.TryGetValue(event_name, out CsrEvent? remover)) {
            FhLog.Info($"Remover available for event \"{event_name}\"");
            u8* code_ptr = Globals.Atel.controllers[0].worker(0)->code_ptr;
            remover(code_ptr);
        } else {
            FhLog.Info($"Remover not available for event \"{event_name}\"");
        }
    }
}
