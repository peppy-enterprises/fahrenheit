using Fahrenheit.FFX.Battle;

using static Fahrenheit.FFX.Globals.Battle;

namespace Fahrenheit.Runtime.Battle;

/// <summary>
/// Fahrenheit module used to implement custom text in message cues.
/// </summary>
[FhLoad(FhGameId.FFX)]
public unsafe class CustomMessageCueModule : FhModule {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate byte MsMessageCueProcess();
    private const nint __addr_MsMessageCueProcess = 0x39ce10;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void TOBtlCloseSimpleHelpMes();
    private const nint __addr_TOBtlCloseSimpleHelpMes = 0x490e60;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawStdChrNameMessageWindow(int chr_id, int text_id);
    private const nint __addr_TOBtlDrawStdChrNameMessageWindow = 0x497170;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawFirstStrikePlayerMessageWindow();
    private const nint __addr_TOBtlDrawFirstStrikePlayerMessageWindow = 0x493460;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawFirstStrikeEnemyMessageWindow();
    private const nint __addr_TOBtlDrawFirstStrikeEnemyMessageWindow = 0x493440;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawGetItemMessageWindow(byte* item_name, int amount);
    private const nint __addr_TOBtlDrawGetItemMessageWindow = 0x493480;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawCaptureMonsterMessageWindow(int mon_id, int text_id);
    private const nint __addr_TOBtlDrawCaptureMonsterMessageWindow = 0x4927e0;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawLearningMessageWindow(int ply_id, int com_id);
    private const nint __addr_TOBtlDrawLearningMessageWindow = 0x495290;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawGetLimitTypeMessageWindow(int ply_id, int limit_mode);
    private const nint __addr_TOBtlDrawGetLimitTypeMessageWindow = 0x493560;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int TOBtlDrawGetMoneyMessageWindow(int amount);
    private const nint __addr_TOBtlDrawGetMoneyMessageWindow = 0x4935d0;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint MsRegSEplay(byte p1, int p2);
    private const nint __addr_MsRegSEplay = 0x7a0120;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int FUN_0089db10(int p1, byte* text);
    private const nint __addr_FUN_0089db10 = 0x49db10;

    private FhMethodHandle<MsMessageCueProcess> _MsMessageCueProcess;

    private TOBtlCloseSimpleHelpMes _TOBtlCloseSimpleHelpMes =
            FhUtil.get_fptr<TOBtlCloseSimpleHelpMes>(__addr_TOBtlCloseSimpleHelpMes);

    private TOBtlDrawStdChrNameMessageWindow _TOBtlDrawStdChrNameMessageWindow =
            FhUtil.get_fptr<TOBtlDrawStdChrNameMessageWindow>(__addr_TOBtlDrawStdChrNameMessageWindow);

    private TOBtlDrawFirstStrikePlayerMessageWindow _TOBtlDrawFirstStrikePlayerMessageWindow =
            FhUtil.get_fptr<TOBtlDrawFirstStrikePlayerMessageWindow>(__addr_TOBtlDrawFirstStrikePlayerMessageWindow);

    private TOBtlDrawFirstStrikeEnemyMessageWindow _TOBtlDrawFirstStrikeEnemyMessageWindow =
            FhUtil.get_fptr<TOBtlDrawFirstStrikeEnemyMessageWindow>(__addr_TOBtlDrawFirstStrikeEnemyMessageWindow);

    private TOBtlDrawGetItemMessageWindow _TOBtlDrawGetItemMessageWindow =
            FhUtil.get_fptr<TOBtlDrawGetItemMessageWindow>(__addr_TOBtlDrawGetItemMessageWindow);

    private TOBtlDrawCaptureMonsterMessageWindow _TOBtlDrawCaptureMonsterMessageWindow =
            FhUtil.get_fptr<TOBtlDrawCaptureMonsterMessageWindow>(__addr_TOBtlDrawCaptureMonsterMessageWindow);

    private TOBtlDrawLearningMessageWindow _TOBtlDrawLearningMessageWindow =
            FhUtil.get_fptr<TOBtlDrawLearningMessageWindow>(__addr_TOBtlDrawLearningMessageWindow);

    private TOBtlDrawGetLimitTypeMessageWindow _TOBtlDrawGetLimitTypeMessageWindow =
            FhUtil.get_fptr<TOBtlDrawGetLimitTypeMessageWindow>(__addr_TOBtlDrawGetLimitTypeMessageWindow);

    private TOBtlDrawGetMoneyMessageWindow _TOBtlDrawGetMoneyMessageWindow =
            FhUtil.get_fptr<TOBtlDrawGetMoneyMessageWindow>(__addr_TOBtlDrawGetMoneyMessageWindow);

    private MsRegSEplay _MsRegSEplay =
            FhUtil.get_fptr<MsRegSEplay>(__addr_MsRegSEplay);

    private FUN_0089db10 _FUN_0089db10 =
            FhUtil.get_fptr<FUN_0089db10>(__addr_FUN_0089db10);

    public CustomMessageCueModule() {
        const string GAME = "FFX.exe";
        _MsMessageCueProcess = new(this, GAME, __addr_MsMessageCueProcess, _h_MsMessageCueProcess);
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
