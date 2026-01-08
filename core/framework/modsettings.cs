// SPDX-License-Identifier: MIT

using Hexa.NET.ImGui;

namespace Fahrenheit.Core;

public abstract class FhSetting(string id) {
    internal const float TOOLTIP_SIZE = 40;
    internal const float NAME_WIDTH   = 300;

    internal string id { get; set; } = id;

    internal abstract void save(ref Utf8JsonWriter writer);
    internal abstract void load(ref Utf8JsonReader reader);
    internal abstract void render();

    public string name { get { return FhApi.Localization.localize($"{id}.name"); } }
    public string desc { get { return FhApi.Localization.localize($"{id}.desc"); } }

    public virtual void render_name() {
        ImGui.AlignTextToFramePadding();
        ImGui.Text(name);
    }

    protected static void render_tooltip(string tooltip) {
        if (!ImGui.BeginItemTooltip()) return;

        ImGui.PushTextWrapPos(ImGui.GetFontSize() * TOOLTIP_SIZE);
        ImGui.TextUnformatted(tooltip);
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
    }
}

public abstract class FhSetting<T>(string id, T defval) : FhSetting(id) where T : notnull {
    protected T    _value    = defval;
    protected bool _disabled = true;

    public T get() {
        return _value;
    }

    internal void set(T value) {
        _value = value;
    }
}

/// <summary>
///     An indented group of settings.
/// </summary>
public class FhSettingsCategory : FhSetting {
    protected readonly FhSetting[] settings;

    public FhSettingsCategory(string id, FhSetting[] settings) : base(id) {
        this.settings = settings;
        update_ids();
    }

    public void update_ids(string? prefix = null) {
        prefix ??= id;
        string dot_prefix = prefix + '.';
        foreach (FhSetting setting in settings) {
            // if (!setting.id.StartsWith(prefix))
            setting.id = dot_prefix + setting.id;

            if (setting is FhSettingsCategory category)
                category.update_ids(prefix);
        }
    }

    public bool collapsed;

    internal override void save(ref Utf8JsonWriter writer) {
        writer.WriteBoolean($"{id}.collapsed", collapsed);
    }

    internal override void load(ref Utf8JsonReader reader) {
        // if (reader.get_bool($"{id}.collapsed", out bool value)) collapsed = value;
    }

    public override void render_name() {
        //TODO: Render extra ArrowButton with Down/Right arrow for collapsing the category
        ImGui.SeparatorText(name);
        render_tooltip(desc);
    }

    internal override void render() {
        ImGui.Dummy(new()); // Get rid of the SameLine from modconfig
        if (!collapsed) {
            ImGui.Indent(ImGui.GetTreeNodeToLabelSpacing());
            foreach (FhSetting setting in settings) {
                setting.render_name();
                ImGui.SameLine(FhSetting.NAME_WIDTH);
                setting.render();
            }
            ImGui.Unindent(ImGui.GetTreeNodeToLabelSpacing());
        }
    }
}

/// <summary>
///     An indented group of settings.
///     Automatically syncs inner settings' <c>locked</c> to the opposite its <c>enabled</c> status.
/// </summary>
public class FhSettingsCategoryToggleable(string id, FhSetting[] settings) : FhSettingsCategory(id, settings) {
    private bool _enabled;

    public bool get() => _enabled;
    public void set(bool enabled) {
        _enabled = enabled;
        //foreach (FhSetting setting in settings) setting.locked = !enabled;
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
        //ImGui.BeginDisabled(locked);
        ImGui.Checkbox(name, ref _enabled);
        render_tooltip(desc);
        ImGui.EndDisabled();

        ImGui.SameLine();
        ImGui.Separator();

        ImGui.EndGroup();
    }
}

/// <summary>
///     An ON/OFF toggle setting.
/// </summary>
public sealed class FhSettingToggle(string id, bool def_value) : FhSetting<bool>(id, def_value) {

    internal override void save(ref Utf8JsonWriter writer) {
        writer.WriteBoolean(id, _value);
    }

    internal override void load(ref Utf8JsonReader reader) {
        // TODO: Add FhSettingToggle loading

        //if (reader.get_bool(id, out var value)) _enabled = value;
    }

    internal override void render() {
        // TODO: Consider mimicking the game's button style as a setting

        render_half(false);
        ImGui.SameLine();
        render_half(true);
    }

    private void render_half(bool second_half) {
        Vector2 button_region = ImGui.GetContentRegionAvail();
        if (!second_half) button_region.X /= 2;
        //TODO: Find a better way to do padding, or get rid of magic number
        button_region.Y = ImGui.GetTextLineHeightWithSpacing() + 4f;

        ImGui.BeginDisabled(_disabled);
        if (_value ^ second_half) ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetStyle().Colors[(int)ImGuiCol.NavCursor]);
        if (ImGui.Button(second_half ? "OFF" : "ON", button_region))
            _value = !second_half;
        ImGui.EndDisabled();
        if (_value ^ second_half) ImGui.PopStyleColor();
    }
}

// TODO: Support callbacks. ImGuiNET is annoying with this, and only provides a delegate for an unsafe function for it
// TODO: We should instead provide a C# wrapper over that for interop with imgui.
/// <summary>
///     A text input setting, with flags. Callbacks are not currently supported.
/// </summary>
public class FhSettingText(string id, string def_value, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None) : FhSetting<string>(id, def_value) {
    public const int MAX_LENGTH = 1 << 10;

    private readonly ImGuiInputTextFlags _flags = flags;

    internal override void save(ref Utf8JsonWriter writer) {
        writer.WriteString(id, _value);
    }

    internal override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingText loading

        //if (reader.get_unordered(id, out string? value)) _value = value!;
    }

    internal override void render() {
        ImGui.InputText($"##setting.{id}", ref _value, MAX_LENGTH, _disabled ? _flags | ImGuiInputTextFlags.ReadOnly : _flags);
    }
}

/// <summary>
///     A dropdown of different values. Allows the player to select only one variant of an enum.
///     <para/>
///     In localization, <c>id.values.{i}.name</c> and <c>id.values.{i}.desc</c> should be used
///     to define the variant names and descriptions, where <c>{i}</c> is the value of that variant in the enum.
/// </summary>
public class FhSettingDropdown<T>(string id, T def_value) : FhSetting<T>(id, def_value) where T : struct, Enum {
    protected readonly T[] _values = Enum.GetValues<T>();

    protected string get_value_name(T value) {
        return FhApi.Localization.localize($"{id}.values.{Convert.ToUInt64(value)}.name");
    }

    internal override void save(ref Utf8JsonWriter writer) {
        writer.WriteNumber(id, Convert.ToUInt64(_value));
    }

    internal override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingDropdown loading

        //if (reader.get_unordered(id, out ulong val)) value = (T)Enum.ToObject(typeof(T), val);
    }

    //TODO: FhSettingDropdown rendering
    internal override void render() {
        if (ImGui.BeginCombo($"###{id}", get_value_name(_value))) {
            for (int i = 0; i < _values.Length; i++) {
                bool is_selected = (Convert.ToUInt64(_value) == Convert.ToUInt64(_values[i]));
                if (ImGui.Selectable($"{get_value_name(_values[i])}###{i}", is_selected))
                    _value = _values[i];
                if (is_selected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }
    }
}

/// <summary>
///     A radio button group. Provides the same functionality as an <c>FhSettingDropdown</c>, but renders as radio buttons.
///     <para/>
///     In localization, <c>id.values.{i}.name</c> and <c>id.values.{i}.desc</c> should be used
///     to define the variant names and descriptions, where <c>{i}</c> is the value of that variant in the enum.
/// </summary>
public class FhSettingRadio<T>(string id, T def_value, int row_count) : FhSettingDropdown<T>(id, def_value)
        where T : struct, Enum {
    protected readonly int row_count = row_count;

    //TODO: FhSettingRadio rendering
    internal override void render() {}
}

/// <summary>
/// A bitfield setting, represented with checkboxes.
/// In localization, <c>id.values.{i}.name</c> and <c>id.values.{i}.desc</c> should be used
///     to define the variant names and descriptions, where <c>{i}</c> is the 0-based index the flag is at in the enum.
/// </summary>
public class FhSettingBitfield<T>(string id, T def_value, int row_count) : FhSettingRadio<T>(id, def_value, row_count) where T : unmanaged, Enum {
    private readonly T[] _flags = Enum.GetValues<T>();

    public void set(T flag, bool enabled) {
        // Generic enums aren't very friendly, so we have to do the operations on a numeric value
        ulong int_value = Convert.ToUInt64(_value);
        ulong int_flag  = Convert.ToUInt64(flag);

        if (enabled) int_value |= int_flag;
        else int_value &= ~int_flag;

        _value = (T)Enum.ToObject(typeof(T), int_value);
    }

    internal override void save(ref Utf8JsonWriter writer) {
        writer.WriteNumber(id, Convert.ToUInt64(_value));
    }

    internal override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingBitfield loading

        //if (reader.get_unordered(id, out ulong val)) value = (T)Enum.ToObject(typeof(T), val);
    }

    internal override void render() {
        // Get rid of the default SameLine(FhSetting.NAME_WIDTH)
        ImGui.Dummy(new());

        int per_row = _flags.Length / row_count;
        float width_per_checkbox = ImGui.GetContentRegionAvail().X / per_row;
        int i = 1;

        ImGui.Indent(ImGui.GetTreeNodeToLabelSpacing());
        foreach(T flag in _flags) {
            bool has_flag = (Convert.ToUInt64(_value) & Convert.ToUInt64(flag)) > 0;
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
            => FhApi.Localization.localize($"{id}.values.{get_bit_idx(val)}.name");

    private string get_flag_desc(T val)
            => FhApi.Localization.localize($"{id}.values.{get_bit_idx(val)}.desc");
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
public unsafe class FhSettingNumber<T>(string id, T def_value, T? min, T? max, T? step) : FhSetting<T>(id, def_value)
        where T : unmanaged, INumber<T> {
    private readonly ImGuiDataType _data_type = get_data_type(def_value);
    private readonly T             _step      = step ?? T.One;
    private readonly T             _min       = min  ?? T.Zero;
    private readonly T             _max       = max  ?? T.One;

    public new T set(T value) => _value = T.Clamp(value, _min, _max);

    internal override void save(ref Utf8JsonWriter writer) {
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

    internal override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingNumber loading

        //if (reader.get_unordered(id, out T value)) _value = value;
    }

    //TODO: Fix FhSettingNumber rendering
    internal override void render() {
        fixed(T* value     = &_value)
        fixed(T* increment = &_step) {
            ImGui.InputScalar($"##{id}", _data_type, value, increment);
            set(_value);
        }
    }

    private static ImGuiDataType get_data_type(T value) {
        return value switch {
            byte   => ImGuiDataType.U8,
            ushort => ImGuiDataType.U16,
            uint   => ImGuiDataType.U32,
            ulong  => ImGuiDataType.U64,
            sbyte  => ImGuiDataType.S8,
            short  => ImGuiDataType.S16,
            int    => ImGuiDataType.S32,
            long   => ImGuiDataType.S64,
            float  => ImGuiDataType.Float,
            double => ImGuiDataType.Double,
            _      => throw new NotImplementedException($"FhSettingNumber<T> expected a built-in number type, not {typeof(T).Name}"),
        };
    }
}

/// <summary>
///     A numeric input using a slider.
/// </summary>
/// <param name="id">The setting's identifier</param>
/// <param name="def_value">The default value of the setting</param>
/// <param name="min">The smallest accepted number, defaults to 0</param>
/// <param name="max">The biggest accepted number, defaults to 1</param>
/// <param name="step">The amount the arrows increase/decrease the value, defaults to 1</param>
/// <typeparam name="T">The underlying numeric type for the value</typeparam>
public class FhSettingSlider<T>(string id, T def_value, T? min, T? max, T? step) : FhSettingNumber<T>(id, def_value, min, max, step)
        where T : unmanaged, INumber<T> {
    //TODO: FhSettingSlider rendering
    internal override void render() {}
}


//TODO: Triage whether FhSettingVector{2|3|4|T} is desired

public class FhSettingColor : FhSetting<Vector4> {

    public FhSettingColor(string id, Vector4 defval) : base(id, defval) { }

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

    internal override void save(ref Utf8JsonWriter writer) {
        writer.WriteStartArray(id);
        writer.WriteNumberValue(_value[0]);
        writer.WriteNumberValue(_value[1]);
        writer.WriteNumberValue(_value[2]);
        writer.WriteNumberValue(_value[3]);
        writer.WriteEndArray();
    }

    internal override void load(ref Utf8JsonReader reader) {
        //TODO: Add FhSettingColor loading

        //if (reader.get_unordered(id, out float[]? value)) _value = new(value);
    }

    //TODO: Fix FhSettingColor rendering
    internal override void render() {
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
    internal override void save(ref Utf8JsonWriter writer) {}
    internal override void load(ref Utf8JsonReader reader) {}
    internal override void render() => renderer();
}
