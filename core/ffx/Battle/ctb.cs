namespace Fahrenheit.Core.FFX.Battle;

//TODO: Find a better name for this
/// <summary>
/// CTB constants for a given agility value.
/// </summary>
public struct CtbBaseData {
    /// <summary>
    /// How much delay a rank 1 command incurs.
    /// </summary>
    public byte tick_speed;

    /// <summary>
    /// How much the initial ctb value assigned at the beginning of battle vary.
    /// The initial ctb value is equal to <c>chr.ram.default_turn_delay - (rng % (initial_ctb_variance + 1))</c>.
    /// </summary>
    public byte initial_ctb_variance;
}
