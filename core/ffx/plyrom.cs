namespace Fahrenheit.Core.FFX;

/// <summary>
/// Factors in the calculation of aeon stat boosts based on yuna's stats.
/// The "individual" factors affect only the boost from the same stat. That is, the Aeon's HP being based on Yuna's HP.
/// The "total" factors affect the boost from all of Yuna's stats, except Luck.
/// In vanilla, this total is calculated with the formula <c>hp/100 + mp/10 + strength + defense + magic + magic_defense + agility + evasion + accuracy</c>.
/// </summary>
public struct PlyRomYunaFactors {
    /// <summary>
    /// Contributes <c>yuna_total * hp_total</c> to the Aeon's HP.
    /// </summary>
    public byte hp_total;

    /// <summary>
    /// Contributes <c>yuna_hp * hp_individual / 100</c> to the Aeon's HP.
    /// </summary>
    public byte hp_individual;

    /// <summary>
    /// Contributes <c>yuna_total * mp_total / 10</c> to the Aeon's MP.
    /// </summary>
    public byte mp_total;

    /// <summary>
    /// Contributes <c>yuna_mp * mp_individual / 100</c> to the Aeon's MP.
    /// </summary>
    public byte mp_individual;

    /// <summary>
    /// Contributes <c>yuna_total / strength_total</c> to the Aeon's strength.
    /// </summary>
    public byte strength_total;

    /// <summary>
    /// Contributes <c>yuna_strength * strength_individual / 10</c> to the Aeon's strength.
    /// </summary>
    public byte strength_individual;

    /// <summary>
    /// Contributes <c>yuna_total / defense_total</c> to the Aeon's defense.
    /// </summary>
    public byte defense_total;

    /// <summary>
    /// Contributes <c>yuna_defense * defense_individual / 10</c> to the Aeon's defense.
    /// </summary>
    public byte defense_individual;

    /// <summary>
    /// Contributes <c>yuna_total / magic_total</c> to the Aeon's magic.
    /// </summary>
    public byte magic_total;

    /// <summary>
    /// Contributes <c>yuna_magic * magic_individual / 10</c> to the Aeon's magic.
    /// </summary>
    public byte magic_individual;

    /// <summary>
    /// Contributes <c>yuna_total / magic_defense_total</c> to the Aeon's magic_defense.
    /// </summary>
    public byte magic_defense_total;

    /// <summary>
    /// Contributes <c>yuna_magic_defense * magic_defense_individual / 10</c> to the Aeon's magic_defense.
    /// </summary>
    public byte magic_defense_individual;

    /// <summary>
    /// Contributes <c>yuna_total / agility_total</c> to the Aeon's agility.
    /// </summary>
    public byte agility_total;

    /// <summary>
    /// Contributes <c>yuna_agility * agility_individual / 10</c> to the Aeon's agility.
    /// </summary>
    public byte agility_individual;

    /// <summary>
    /// Contributes <c>yuna_total / evasion_total</c> to the Aeon's evasion.
    /// </summary>
    public byte evasion_total;

    /// <summary>
    /// Contributes <c>yuna_evasion * evasion_individual / 10</c> to the Aeon's evasion.
    /// </summary>
    public byte evasion_individual;

    /// <summary>
    /// Contributes <c>yuna_total / accuracy_total</c> to the Aeon's accuracy.
    /// </summary>
    public byte accuracy_total;

    /// <summary>
    /// Contributes <c>yuna_accuracy * accuracy_individual / 10</c> to the Aeon's accuracy.
    /// </summary>
    public byte accuracy_individual;
}

/// <summary>
/// As the name <c>PlyRom</c> (Player Read-Only Memory) indicates,
/// this struct contains a few constants player characters use.
/// </summary>
public struct PlyRom {
    /// <summary>
    /// The help text displayed when switching to the character.
    /// </summary>
    public ExcelString2 switch_text;

    /// <summary>
    ///The text displayed when scanning a character.
    /// </summary>
    public ExcelString2 scan_text;

    /// <summary>
    /// The cubic multiplier in the formula for the next level requirement.
    /// The formula is ax<sup>3</sup>/100 + bx<sup>2</sup>/10 + c(x + 1),
    /// where x is the total amount of gained sphere levels.
    /// </summary>
    public byte slv_req_mult_a;

    /// <summary>
    /// The quadratic multiplier in the formula for the next level requirement.
    /// The formula is ax<sup>3</sup>/100 + bx<sup>2</sup>/10 + c(x + 1),
    /// where x is the total amount of gained sphere levels.
    /// </summary>
    public byte slv_req_mult_b;

    /// <summary>
    /// The linear multiplier in the formula for the next level requirement.
    /// The formula is ax<sup>3</sup>/100 + bx<sup>2</sup>/10 + c(x + 1),
    /// where x is the total amount of gained sphere levels.
    /// </summary>
    public byte slv_req_mult_c;

    /// <summary>
    /// The maximum amount of AP required to reach the next level.
    /// In vanilla, applies from the 101st sphere level onwards.
    /// </summary>
    public int slv_req_max;

    /// <summary>
    /// Factors in the calculation of aeon stat boosts based on yuna's stats.
    /// </summary>
    public PlyRomYunaFactors yuna_stat_boost_factors;

    /// <summary>
    /// The number of turns that Doom takes to kill the character.
    /// </summary>
    public byte doom_duration;

    // chr.ram.__0x199 is set to this
    private byte __0x2B;
}
