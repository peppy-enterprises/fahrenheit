using Fahrenheit.FFX;
using Fahrenheit.FFX.Battle;

using static Fahrenheit.FFX.Globals.Battle;

namespace Fahrenheit.Runtime.Battle;

/// <summary>
/// Fahrenheit module used to implement custom text in message cues.
/// </summary>
[FhLoad(FhGameId.FFX)]
public unsafe class CustomMessageCueModule : FhModule {
    private FhMethodHandle<FFX.FhCall.MsMessageCueProcess> _MsMessageCueProcess;

    private FFX.FhCall.TOBtlCloseSimpleHelpMes _TOBtlCloseSimpleHelpMes =
            FhUtil.get_fptr<FFX.FhCall.TOBtlCloseSimpleHelpMes>(FFX.FhCall.__addr_TOBtlCloseSimpleHelpMes);

    private FFX.FhCall.TOBtlDrawStdChrNameMessageWindow _TOBtlDrawStdChrNameMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawStdChrNameMessageWindow>(FFX.FhCall.__addr_TOBtlDrawStdChrNameMessageWindow);

    private FFX.FhCall.TOBtlDrawFirstStrikePlayerMessageWindow _TOBtlDrawFirstStrikePlayerMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawFirstStrikePlayerMessageWindow>(FFX.FhCall.__addr_TOBtlDrawFirstStrikePlayerMessageWindow);

    private FFX.FhCall.TOBtlDrawFirstStrikeEnemyMessageWindow _TOBtlDrawFirstStrikeEnemyMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawFirstStrikeEnemyMessageWindow>(FFX.FhCall.__addr_TOBtlDrawFirstStrikeEnemyMessageWindow);

    private FFX.FhCall.TOBtlDrawGetItemMessageWindow _TOBtlDrawGetItemMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawGetItemMessageWindow>(FFX.FhCall.__addr_TOBtlDrawGetItemMessageWindow);

    private FFX.FhCall.TOBtlDrawCaptureMonsterMessageWindow _TOBtlDrawCaptureMonsterMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawCaptureMonsterMessageWindow>(FFX.FhCall.__addr_TOBtlDrawCaptureMonsterMessageWindow);

    private FFX.FhCall.TOBtlDrawLearningMessageWindow _TOBtlDrawLearningMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawLearningMessageWindow>(FFX.FhCall.__addr_TOBtlDrawLearningMessageWindow);

    private FFX.FhCall.TOBtlDrawGetLimitTypeMessageWindow _TOBtlDrawGetLimitTypeMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawGetLimitTypeMessageWindow>(FFX.FhCall.__addr_TOBtlDrawGetLimitTypeMessageWindow);

    private FFX.FhCall.TOBtlDrawGetMoneyMessageWindow _TOBtlDrawGetMoneyMessageWindow =
            FhUtil.get_fptr<FFX.FhCall.TOBtlDrawGetMoneyMessageWindow>(FFX.FhCall.__addr_TOBtlDrawGetMoneyMessageWindow);

    private FFX.FhCall.MsRegSEplay _MsRegSEplay =
            FhUtil.get_fptr<FFX.FhCall.MsRegSEplay>(FFX.FhCall.__addr_MsRegSEplay);

    private FFX.FhCall.FUN_0089db10 _FUN_0089db10 =
            FhUtil.get_fptr<FFX.FhCall.FUN_0089db10>(FFX.FhCall.__addr_FUN_0089db10);

    public CustomMessageCueModule() {
        const string GAME = "FFX.exe";
        _MsMessageCueProcess = new(this, GAME, FFX.FhCall.__addr_MsMessageCueProcess, _h_MsMessageCueProcess);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        return _MsMessageCueProcess.hook();
    }

    private int _draw_custom_message_window(byte* message) {
        _FUN_0089db10(0, message);
        return 7;
    }

    [UnmanagedCallConv(CallConvs = [ typeof(CallConvCdecl) ])]
    public byte _h_MsMessageCueProcess() {
        if (btl->__0x2076 > 0) {
            btl->__0x2076 -= 1;
        }

        byte bVar2 = btl->__0x2076;

        if (btl->message_cue_count == 0) {
            return 0;
        }

        byte bVar3 = btl->__0x2077;
        while (true) {
            if (btl->__0x2040 != 0) {
                if (btl->__0x2040 > -1) {
                    if ( bVar2 != 0
                      && ((FhUtil.get_at<byte>((nint)btl + 0x2034) & 0x20) == 0
                       || (FhUtil.get_at<byte>((nint)btl + 0x2038) & 0x20) == 0
                       ||  bVar3 <= bVar2)) {
                        return btl->message_cue_count;
                    }
                    _TOBtlCloseSimpleHelpMes();
                }

                bVar2 = 0;
                bVar3 = 0;
                btl->__0x2040 = 0;
                btl->__0x2076 = 0;
                btl->__0x2077 = 0;
                btl->__0x2075 = (byte)((btl->__0x2075 + 1) & 3);
                btl->message_cue_count -= 1;

                if (btl->message_cue_count == 0) {
                    return 0;
                }

                continue;
            }

            int cue_idx = btl->__0x2075 & 3;
            MessageCue cue = btl->message_cues[cue_idx];

            if (cue.type is MessageCueType.LEARN_COMMAND or MessageCueType.LEARN_LIMIT_TYPE) {
                _MsRegSEplay(0xFF, 0x39);
            }

            int draw_ret = cue.type switch {
                MessageCueType.PLY_NAME         => _TOBtlDrawStdChrNameMessageWindow(cue.arg1, cue.arg2),
                MessageCueType.PREEMPTIVE       => _TOBtlDrawFirstStrikePlayerMessageWindow(),
                MessageCueType.AMBUSH           => _TOBtlDrawFirstStrikeEnemyMessageWindow(),
                MessageCueType.STEAL_ITEM       => _TOBtlDrawGetItemMessageWindow((byte*)cue.arg1, cue.arg2),
                MessageCueType.CAPTURE_MONSTER  => _TOBtlDrawCaptureMonsterMessageWindow(cue.arg1, cue.arg2),
                MessageCueType.LEARN_COMMAND    => _TOBtlDrawLearningMessageWindow(cue.arg1, cue.arg2),
                MessageCueType.LEARN_LIMIT_TYPE => _TOBtlDrawGetLimitTypeMessageWindow(cue.arg1, cue.arg2),
                MessageCueType.STEAL_MONEY      => _TOBtlDrawGetMoneyMessageWindow(cue.arg1),
                MessageCueType.FH_CUSTOM        => _draw_custom_message_window((byte*)cue.arg1),
                _                               => -1,
            };

            bVar2 = cue.__0x2;
            bVar3 = cue.__0x1;
            btl->__0x2040 = draw_ret;
            btl->__0x2076 = bVar2;
            btl->__0x2077 = bVar3;
        }
    }
}
