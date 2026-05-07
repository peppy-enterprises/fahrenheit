// SPDX-License-Identifier: MIT

namespace Fahrenheit.FFX.Battle;

/// <summary>The type of the message cued for by a MessageCue.</summary>
public enum MessageCueType : byte {
	/// <summary>Displays a battle text formatted with a character name.</summary>
    PLY_NAME = 1,

    /// <summary>Displays the text "Preemptive strike!"</summary>
    PREEMPTIVE = 2,

    /// <summary>Displays the text "Ambushed!"</summary>
    AMBUSH = 3,

    /// <summary>
    ///     Displays text informing the player of the results of stealing items.<br/>
    ///     The specific text depends on the amount provided:
    ///     <ul>
    ///         <li>When <c>amount == -1</c>, displays "Couldn't steal anything."</li>
    ///         <li>When <c>amount == 0</c>, displays "Nothing to steal."</li>
    ///         <li>When <c>amount == 1</c>, displays "Stole {item_name}!"</li>
    ///         <li>When <c>amount > 1</c>, displays "Stole {item_name} x{amount}!"</li>
    ///     </ul>
    /// </summary>
    STEAL_ITEM = 4,

    /// <summary>
    ///     Displays text informing the player of the results of capturing.<br/>
    ///	    The specific text depends on the text id provided:
	///	    <ul>
	///         <li>When <c>text_id == 0x300A</c>, displays "{monster_name} captured!"</li>
	///         <li>When <c>text_id == 0x300B</c>, displays "{monster_name} capture limit already reached."</li>
	///         <li>When <c>text_id == 0x300C</c>, displays "{monster_name} cannot be captured."</li>
	///     </ul>
    /// </summary>
    CAPTURE_MONSTER = 5,

    /// <summary>Displays the text "{player_name} has learned {command_name}!"</summary>
    LEARN_COMMAND = 6,

    /// <summary>Display the text "{player_name} has learned Overdrive mode {limit_mode_name}!"</summary>
    LEARN_LIMIT_TYPE = 7,

    /// <summary>
    ///     Displays text informing the player of the results of stealing gil.<br/>
    ///     The specific text depends on the amount provided:
    ///     <ul>
    ///        <li>When <c>amount == -1</c>, displays "Couldn't steal any gil!"</li>
    ///        <li>When <c>amount == 0</c>, displays "Out of gil!"</li>
    ///        <li>When <c>amount > 0</c>, displays "Stole {amount} gil!"</li>
    ///     </ul>
    /// </summary>
    STEAL_MONEY = 8,

    /// <summary>Displays custom text.</summary>
	FH_CUSTOM = 9,
}

/// <summary>A cue for the game to show a message at the top of the screen in battle.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct MessageCue {
	/// <summary>The type of the message.</summary>
    public MessageCueType type;

    public byte __0x1;
	public byte __0x2;

	/// <summary>
	///     The first argument for the message.<br/>
	/// 	If this argument is not needed, it should be 0.
	/// </summary>
	public int arg1;

	/// <summary>
	///     The second argument for the message.<br/>
    /// 	If this argument is not needed, it should be 0.
    /// </summary>
	public int arg2;
}
