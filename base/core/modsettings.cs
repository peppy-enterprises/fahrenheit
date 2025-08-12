using Hexa.NET.ImGui;

using static Fahrenheit.Core.FhLocalizationManager;

namespace Fahrenheit.Core;

public abstract class FhSetting(string id) {
    public const float TOOLTIP_SIZE = 40f;
    public const float NAME_WIDTH = 300f;

    public string id { get; set; } = id;
    public bool locked;
    public string name { get { return FhApi.LocalizationManager.localize($"{id}.name"); } }
    public string desc { get { return FhApi.LocalizationManager.localize($"{id}.desc"); } }

    public abstract void save(ref Utf8JsonWriter writer);
    public abstract void load(ref Utf8JsonReader reader);
    public abstract void render();

    public virtual void render_name() {
        ImGui.AlignTextToFramePadding();
        ImGui.Text(name);
    }

    protected static void render_tooltip(string tooltip) {
        if (ImGui.BeginItemTooltip()) {
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * TOOLTIP_SIZE);
            ImGui.TextUnformatted(tooltip);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }
}

/// <summary>
/// An indented group of settings.
/// </summary>
public class FhSettingsCategory : FhSetting {
    protected readonly FhSetting[] settings;

    public FhSettingsCategory(string id, FhSetting[] settings) : base(id) {
        string id_prefix = id + '.';
        foreach (var setting in settings) {
            setting.id = id_prefix + setting.id;
        }

        this.settings = settings;
    }

    public bool collapsed;

    public override void save(ref Utf8JsonWriter writer) {
        writer.WriteBoolean($"{id}.collapsed", collapsed);
    }

    public override void load(ref Utf8JsonReader reader) {
        // if (reader.get_bool($"{id}.collapsed", out bool value)) collapsed = value;
    }

    public override void render_name() {
        //TODO: Render extra ArrowButton with Down/Right arrow for collapsing the category
        ImGui.SeparatorText(name);
        render_tooltip(desc);
    }

    public override void render() {
        ImGui.Dummy(new()); // Get rid of the SameLine from modconfig
        if (!collapsed) {
            ImGui.Indent(ImGui.GetTreeNodeToLabelSpacing());
            foreach (var setting in settings) {
                setting.render_name();
                ImGui.SameLine(FhSetting.NAME_WIDTH);
                setting.render();
            }
            ImGui.Unindent(ImGui.GetTreeNodeToLabelSpacing());
        }
    }
}

/// <summary>
/// An indented group of settings.
/// Automatically syncs inner settings' <c>locked</c> to the opposite its <c>enabled</c> status.
/// </summary>
public class FhSettingsCategoryToggleable(string id, FhSetting[] settings) : FhSettingsCategory(id, settings) {
    private bool _enabled;

    public bool get() => _enabled;
    public void set(bool enabled) {
        _enabled = enabled;
        foreach (var setting in settings) setting.locked = !enabled;
    }

    public override void render_name() {
        ImGui.BeginGroup();

        //TODO: Render extra ArrowButton with Down/Right arrow for collapsing the category

        //NOTE: Separator doesn't accept item width (https://github.com/ocornut/imgui/issues/7255)
        //TODO: Fix FhSettingsCategoryToggleable separator rendering to match FhSettingsCategory's
        ImGui.PushClipRect(
                new(ImGui.GetCursorPosX(), ImGui.GetCursorPosY() + ImGui.GetTextLineHeightWithSpacing()/2f - 1f),
                new(ImGui.GetCursorPosX() + ImGui.GetTreeNodeToLabelSpacing()/2f, ImGui.GetCursorPosY() + ImGui.GetTextLineHeightWithSpacing()/2f + 1f),
                false
            );
        ImGui.Separator();
        ImGui.PopClipRect();

        ImGui.SameLine();
        ImGui.BeginDisabled(locked);
        ImGui.Checkbox(name, ref _enabled);
        ImGui.EndDisabled();

        ImGui.SameLine();
        ImGui.Separator();

        ImGui.EndGroup();
    }
}

/// <summary>
/// An ON/OFF toggle setting.
/// </summary>
public class FhSettingToggle(string id, bool def_value) : FhSetting(id) {
    public const uint BUTTON_BG         = 0x10ffffff;
    public const uint BUTTON_BG_LOCKED  = 0x18ffffff;
    public const uint BUTTON_BG_HOVERED = 0x20ffffff;
    public const uint BUTTON_BG_ACTIVE  = 0x60ffffff;
    public const uint UNDERLINE_GRADIENT_START      = 0xff0098ff;
    public const uint UNDERLINE_GRADIENT_SIDE_END   = 0xff00c9ff;
    public const uint UNDERLINE_GRADIENT_CENTER_END = 0xffffffff;
    public const uint UNDERLINE_GRADIENT_START_LOCKED      = 0xff0098ff;
    public const uint UNDERLINE_GRADIENT_SIDE_END_LOCKED   = 0xff00c9ff;

    private bool _enabled = def_value;

    public bool get() => _enabled;
    public void set(bool enabled) => _enabled = enabled;

    public override void save(ref Utf8JsonWriter writer) {
        writer.WriteBoolean(id, _enabled);
    }

    public override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingToggle loading

        //if (reader.get_bool(id, out var value)) _enabled = value;
    }

    public override void render() {
        // Mimic the game's config style
        ImGui.PushStyleColor(ImGuiCol.Button, locked ? BUTTON_BG_LOCKED : BUTTON_BG);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, locked ? BUTTON_BG_LOCKED : BUTTON_BG_HOVERED);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, locked ? BUTTON_BG_LOCKED : BUTTON_BG_ACTIVE);

        render_half(false);
        ImGui.SameLine();
        render_half(true);

        ImGui.PopStyleColor(3);
    }

    private void render_half(bool second_half) {
        Vector2 button_region = ImGui.GetContentRegionAvail();
        if (!second_half) button_region.X /= 2;
        //TODO: Find a better way to do padding, or get rid of magic number
        button_region.Y = ImGui.GetTextLineHeightWithSpacing() + 4f;

        ImGui.BeginDisabled(locked);
        if (ImGui.Button(second_half ? "OFF" : "ON", button_region))
            _enabled = !second_half;
        ImGui.EndDisabled();
        if (_enabled ^ second_half) render_underline();
    }

    private void render_underline() {
        Vector2 min = ImGui.GetItemRectMin() + new Vector2(0, 2f);
        Vector2 max = ImGui.GetItemRectMax() + new Vector2(0, 2f);
        Vector2 center = min + (max - min) / 2;
        uint start = ImGui.GetColorU32(locked ? UNDERLINE_GRADIENT_START_LOCKED : UNDERLINE_GRADIENT_START, 0f);
        uint side_end = ImGui.GetColorU32(locked ? UNDERLINE_GRADIENT_SIDE_END_LOCKED : UNDERLINE_GRADIENT_SIDE_END, 1f);
        uint center_end = ImGui.GetColorU32(UNDERLINE_GRADIENT_CENTER_END, 1f);

        ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

        draw_list.AddRectFilledMultiColor(min with { Y = max.Y - 1 }, center with { Y = max.Y },
                start, side_end, side_end, start);
        draw_list.AddRectFilledMultiColor(max with { Y = max.Y - 1 }, center with { Y = max.Y },
                start, side_end, side_end, start);

        draw_list.AddRectFilledMultiColor(min with { Y = max.Y + 1 }, center with { Y = max.Y },
                start, center_end, center_end, start);
        draw_list.AddRectFilledMultiColor(max with { Y = max.Y + 1 }, center with { Y = max.Y },
                start, center_end, center_end, start);

        draw_list.AddRectFilledMultiColor(min with { Y = max.Y + 2 }, center with { Y = max.Y + 1 },
                start, side_end, side_end, start);
        draw_list.AddRectFilledMultiColor(max with { Y = max.Y + 2 }, center with { Y = max.Y + 1 },
                start, side_end, side_end, start);
    }
}

//TODO: Support callbacks. ImGuiNET is annoying with this, and only provides a delegate for an unsafe function for it
//TODO: We should instead provide a C# wrapper over that for interop with imgui.
/// <summary>
/// A text input setting, with flags. Callbacks are not currently supported.
/// </summary>
public class FhSettingText(string id, string def_value, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None) : FhSetting(id) {
    public const int MAX_LENGTH = 1 << 10;

    private string _value = def_value;
    private readonly ImGuiInputTextFlags _flags = flags;

    public string get() => _value;
    public void set(string text) => _value = text;

    public override void save(ref Utf8JsonWriter writer) {
        writer.WriteString(id, _value);
    }

    public override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingText loading

        //if (reader.get_unordered(id, out string? value)) _value = value!;
    }

    public override void render() {
        ImGui.InputText($"##setting.{id}", ref _value, MAX_LENGTH, locked ? _flags | ImGuiInputTextFlags.ReadOnly : _flags);
    }
}

/// <summary>
/// A dropdown of different values.
/// Allows the player to select only one variant of an enum.
/// In localization, <c>id.values.{i}.name</c> and <c>id.values.{i}.desc</c> should be used
///     to define the variant names and descriptions, where <c>{i}</c> is the value of that variant in the enum.
/// </summary>
public class FhSettingDropdown<T>(string id, T def_value) : FhSetting(id)
        where T : Enum {
    protected T value = def_value;

    public T get() => value;
    public void set(T val) => value = val;

    public override void save(ref Utf8JsonWriter writer) {
        writer.WriteNumber(id, Convert.ToUInt64(value));
    }

    public override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingDropdown loading

        //if (reader.get_unordered(id, out ulong val)) value = (T)Enum.ToObject(typeof(T), val);
    }

    //TODO: FhSettingDropdown rendering
    public override void render() {}
}

/// <summary>
/// A radio button group. Provides the same functionality as an <c>FhSettingDropdown</c>, but renders as radio buttons.
/// In localization, <c>id.values.{i}.name</c> and <c>id.values.{i}.desc</c> should be used
///     to define the variant names and descriptions, where <c>{i}</c> is the value of that variant in the enum.
/// </summary>
public class FhSettingRadio<T>(string id, T def_value, int row_count) : FhSettingDropdown<T>(id, def_value)
        where T : Enum {
    protected readonly int row_count = row_count;

    //TODO: FhSettingRadio rendering
    public override void render() {}
}

/// <summary>
/// A bitfield setting, represented with checkboxes.
/// In localization, <c>id.values.{i}.name</c> and <c>id.values.{i}.desc</c> should be used
///     to define the variant names and descriptions, where <c>{i}</c> is the 0-based index the flag is at in the enum.
/// </summary>
public class FhSettingBitfield<T>(string id, T def_value, int row_count) : FhSettingRadio<T>(id, def_value, row_count)
        where T : struct, Enum {
    private readonly T[] _flags = Enum.GetValues<T>();

    public void set(T flag, bool enabled) {
        // Generic enums aren't very friendly, so we have to do the operations on a numeric value
        ulong int_value = Convert.ToUInt64(value);
        ulong int_flag = Convert.ToUInt64(flag);

        if (enabled) int_value |= int_flag;
        else int_value &= ~int_flag;

        value = (T)Enum.ToObject(typeof(T), int_value);
    }

    public override void save(ref Utf8JsonWriter writer) {
        writer.WriteNumber(id, Convert.ToUInt64(value));
    }

    public override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingBitfield loading

        //if (reader.get_unordered(id, out ulong val)) value = (T)Enum.ToObject(typeof(T), val);
    }

    public override void render() {
        // Get rid of the default SameLine(FhSetting.NAME_WIDTH)
        ImGui.Dummy(new());

        int per_row = _flags.Length / row_count;
        float width_per_checkbox = ImGui.GetContentRegionAvail().X / per_row;
        int i = 1;

        ImGui.Indent(ImGui.GetTreeNodeToLabelSpacing());
        foreach(T flag in _flags) {
            bool has_flag = (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) > 0;
            ImGui.Checkbox(get_flag_name(flag), ref has_flag);
            set(flag, has_flag);
            render_tooltip(get_flag_desc(flag));

            if (i == per_row) i = 0;
            else ImGui.SameLine(i * width_per_checkbox);
            i++;
        }
        ImGui.Unindent(ImGui.GetTreeNodeToLabelSpacing());
    }

    private int get_bit_idx(T val) {
        int idx = -1;
        ulong num_value = Convert.ToUInt64(val);
        while (num_value > 0) {
            num_value >>= 1;
            idx++;
        }
        return idx;
    }

    private string get_flag_name(T val)
            => FhApi.LocalizationManager.localize($"{id}.values.{get_bit_idx(val)}.name");

    private string get_flag_desc(T val)
            => FhApi.LocalizationManager.localize($"{id}.values.{get_bit_idx(val)}.desc");
}

/// <summary>
/// A numeric/spinbox input.
/// </summary>
/// <param name="id">The setting's identifier</param>
/// <param name="def_value">The default value of the setting</param>
/// <param name="min">The smallest accepted number, defaults to 0</param>
/// <param name="max">The biggest accepted number, defaults to 1</param>
/// <param name="step">The amount the arrows increase/decrease the value, defaults to 1. Set to 0 to disable the arrows.</param>
/// <typeparam name="T">The underlying numeric type for the value</typeparam>
public unsafe class FhSettingNumber<T>(string id, T def_value, T? min, T? max, T? step) : FhSetting(id)
        where T : unmanaged, INumber<T> {
    private readonly ImGuiDataType _data_type = get_data_type(def_value);
    private T _value = def_value;
    private T _step = step ?? T.One;
    public T min  { get; set; } = min  ?? T.Zero;
    public T max  { get; set; } = max  ?? T.One;
    public T step { get { return _step; } set { _step = value; } }

    public T get() => _value;
    public T set(T value) => _value = T.Clamp(value, min, max);

    public override void save(ref Utf8JsonWriter writer) {
        switch (_value) {
            case byte   n: writer.WriteNumber(id, n); break;
            case ushort n: writer.WriteNumber(id, n); break;
            case uint   n: writer.WriteNumber(id, n); break;
            case ulong  n: writer.WriteNumber(id, n); break;
            case sbyte  n: writer.WriteNumber(id, n); break;
            case short  n: writer.WriteNumber(id, n); break;
            case int    n: writer.WriteNumber(id, n); break;
            case long   n: writer.WriteNumber(id, n); break;
            case float  n: writer.WriteNumber(id, n); break;
            case double n: writer.WriteNumber(id, n); break;
        }
    }

    public override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingNumber loading

        //if (reader.get_unordered(id, out T value)) _value = value;
    }

    //TODO: Fix FhSettingNumber rendering
    public override void render() {
        fixed(T* value = &_value)
        fixed(T* increment = &_step) {
            ImGui.InputScalar($"##{id}", _data_type, (void*)value, (void*)increment);
            _value = T.Clamp(_value, min, max);
        }
    }

    private static ImGuiDataType get_data_type(T example_value) {
        return example_value switch {
            byte   _ => ImGuiDataType.U8,
            ushort _ => ImGuiDataType.U16,
            uint   _ => ImGuiDataType.U32,
            ulong  _ => ImGuiDataType.U64,
            sbyte  _ => ImGuiDataType.S8,
            short  _ => ImGuiDataType.S16,
            int    _ => ImGuiDataType.S32,
            long   _ => ImGuiDataType.S64,
            float  _ => ImGuiDataType.Float,
            double _ => ImGuiDataType.Double,
            // unreachable; I wish there was a way to notate as such
            _ => throw new NotImplementedException($"FhSettingNumber<T> expected a built-in number type, not {typeof(T).Name}"),
        };
    }
}

/// <summary>
/// A numeric input using a slider.
/// </summary>
/// <param name="id">The setting's identifier</param>
/// <param name="def_value">The default value of the setting</param>
/// <param name="min">The smallest accepted number, defaults to 0</param>
/// <param name="max">The biggest accepted number, defaults to 1</param>
/// <param name="step">The amount the arrows increase/decrease the value, defaults to 1</param>
/// <typeparam name="T">The underlying numeric type for the value</typeparam>
public class FhSettingSlider<T>(string id, T def_value, T? min, T? max, T? step)
        : FhSettingNumber<T>(id, def_value, min, max, step)
        where T : unmanaged, INumber<T> {
    //TODO: FhSettingSlider rendering
    public override void render() {}
}

public class FhSettingVector2<T> : FhSetting where T : unmanaged, INumber<T> {
    public FhSettingNumber<T> x { get; }
    public FhSettingNumber<T> y { get; }

    public FhSettingVector2(string id, FhSettingNumber<T> x, FhSettingNumber<T> y) : base(id) {
        x.id = id + ".x";
        y.id = id + ".y";

        this.x = x;
        this.y = y;
    }

    public (T x, T y) get() {
        return (x.get(), y.get());
    }

    public void set(T new_x, T new_y) {
        x.set(new_x);
        y.set(new_y);
    }

    public override void save(ref Utf8JsonWriter writer) {
        x.save(ref writer);
        y.save(ref writer);
    }

    public override void load(ref Utf8JsonReader reader) {
        x.load(ref reader);
        y.load(ref reader);
    }

    //TODO: Fix FhSettingVector2 rendering
    public override void render() {
        float part_width = (ImGui.GetContentRegionAvail().X - NAME_WIDTH) / 2 - 4f;

        ImGui.SetNextItemWidth(part_width);
        x.render();
        render_tooltip("X");

        ImGui.SameLine(NAME_WIDTH + part_width + 8f);

        ImGui.SetNextItemWidth(part_width);
        y.render();
        render_tooltip("Y");
    }
}

public class FhSettingVector3<T> : FhSetting where T : unmanaged, INumber<T> {
    public FhSettingNumber<T> x { get; }
    public FhSettingNumber<T> y { get; }
    public FhSettingNumber<T> z { get; }

    public FhSettingVector3(string id, FhSettingNumber<T> x, FhSettingNumber<T> y, FhSettingNumber<T> z) : base(id) {
        x.id = id + ".x";
        y.id = id + ".y";
        z.id = id + ".z";

        this.x = x;
        this.y = y;
        this.z = z;
    }

    public (T x, T y, T z) get() {
        return (x.get(), y.get(), z.get());
    }

    public void set(T new_x, T new_y, T new_z) {
        x.set(new_x);
        y.set(new_y);
        z.set(new_z);
    }

    public override void save(ref Utf8JsonWriter writer) {
        x.save(ref writer);
        y.save(ref writer);
        z.save(ref writer);
    }

    public override void load(ref Utf8JsonReader reader) {
        x.load(ref reader);
        y.load(ref reader);
        z.load(ref reader);
    }

    //TODO: FhSettingVector3 rendering
    public override void render() {}
}

//TODO: Triage whether FhSettingVector4 is desired

public class FhSettingColor : FhSetting {
    private Vector4 _value;

    public FhSettingColor(string id, uint rgba8) : base(id) {
        set_rgba8(rgba8);
    }

    public Vector4 get() => _value;
    public void set(Vector4 value) => _value = value;

    public void set_rgba8(uint value) => set_rgba8(
            r: (byte)(value & 0xff),
            g: (byte)((value >>  0x8) & 0xff),
            b: (byte)((value >> 0x10) & 0xff),
            a: (byte)((value >> 0x18) & 0xff)
        );

    public void set_rgba8(byte? r = null, byte? g = null, byte? b = null, byte? a = null) {
        if (r != null) _value[0] = (byte)r/255f;
        if (g != null) _value[1] = (byte)g/255f;
        if (b != null) _value[2] = (byte)b/255f;
        if (a != null) _value[3] = (byte)a/255f;
    }

    public void set_rgba(float? r = null, float? g = null, float? b = null, float? a = null) {
        if (r != null) _value[0] = (float)r/255f;
        if (g != null) _value[1] = (float)g/255f;
        if (b != null) _value[2] = (float)b/255f;
        if (a != null) _value[3] = (float)a/255f;
    }

    public override void save(ref Utf8JsonWriter writer) {
        writer.WriteStartArray(id);
        writer.WriteNumberValue(_value[0]);
        writer.WriteNumberValue(_value[1]);
        writer.WriteNumberValue(_value[2]);
        writer.WriteNumberValue(_value[3]);
        writer.WriteEndArray();
    }

    public override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingColor loading

        //if (reader.get_unordered(id, out float[]? value)) _value = new(value);
    }

    //TODO: Fix FhSettingColor rendering
    public override void render() {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        ImGui.ColorEdit4($"##{id}", ref _value, ImGuiColorEditFlags.Float
                                                | ImGuiColorEditFlags.AlphaPreviewHalf
                                                | ImGuiColorEditFlags.AlphaBar
                                                | ImGuiColorEditFlags.DisplayHsv);
    }
}

/// <summary>
/// A setting that doesn't store any data itself. Useful for rendering custom previews etc.
/// </summary>
public class FhSettingCustomRenderer(string id, Action renderer) : FhSetting(id) {
    public override void save(ref Utf8JsonWriter writer) {}
    public override void load(ref Utf8JsonReader reader) {}
    public override void render() => renderer();
}
