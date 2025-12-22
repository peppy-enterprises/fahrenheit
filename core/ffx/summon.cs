namespace Fahrenheit.Core.FFX;

/// <summary>
/// Data detailing the minimum stat boost for an Aeon based on the battle count.
/// In vanilla, this boost increases every 30 battles, starting at 60.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct AeonStatBattleCountBoosts {
    /// <summary>
    /// The boost to the Aeon's HP.
    /// </summary>
    public int hp;

    /// <summary>
    /// The boost to the Aeon's MP.
    /// </summary>
    public int mp;

    /// <summary>
    /// The boost to the Aeon's strength.
    /// </summary>
    public byte strength;

    /// <summary>
    /// The boost to the Aeon's defense.
    /// </summary>
    public byte defense;

    /// <summary>
    /// The boost to the Aeon's magic.
    /// </summary>
    public byte magic;

    /// <summary>
    /// The boost to the Aeon's magic defense.
    /// </summary>
    public byte magic_defense;

    /// <summary>
    /// The boost to the Aeon's accuracy.
    /// </summary>
    public byte accuracy;

    /// <summary>
    /// The boost to the Aeon's evasion.
    /// </summary>
    public byte evasion;

    /// <summary>
    /// The boost to the Aeon's agility.
    /// </summary>
    public byte agility;

    /// <summary>
    /// The boost to the Aeon's luck.
    /// </summary>
    public byte luck;
}

/// <summary>
/// Recipe for teaching an aeon an ability using a set amount of an item.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0x8)]
public struct AeonAbilityRecipe {
    [FieldOffset(0x0)] internal byte __0x0; // Always 0x7F

    /// <summary>
    /// The command to be taught.
    /// </summary>
    [FieldOffset(0x2)] public T_XCommandId command; // not the command id if __0x7 is true

    /// <summary>
    /// The item to be spent in teaching the command.
    /// </summary>
    [FieldOffset(0x4)] public T_XCommandId item;

    /// <summary>
    /// The amount of the item that is needed.
    /// </summary>
    [FieldOffset(0x6)] public byte item_cost;

    [FieldOffset(0x7)] internal bool __0x7;
}
