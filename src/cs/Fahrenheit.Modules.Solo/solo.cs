using Fahrenheit.CLRHost;
using Fahrenheit.CoreLib;
using Fahrenheit.CoreLib.FFX;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using static Fahrenheit.CoreLib.FFX.Call;

namespace Fahrenheit.Modules.Solo;

public sealed record SoloModuleConfig : FhModuleConfig {
    public uint chrID { get; }

    [JsonConstructor]
    public SoloModuleConfig(string configName,
                              uint   configVersion,
                              bool   configEnabled,
                              uint   chrID) : base(configName, configVersion, configEnabled) {
        this.chrID = chrID;
    }

    public override bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm) {
        fm = new SoloModule(this);
        return fm.ModuleState == FhModuleState.InitSuccess;
    }
}

public unsafe class SoloModule : FhModule {
    private uint desired_chr_id;
    private readonly SoloModuleConfig _moduleConfig;
    private FhMethodHandle<MsGetSavePlyJoined> _MsGetSavePlyJoined;
    private FhMethodHandle<MsSetSavePlyJoined> _MsSetSavePlyJoined;
    private FhMethodHandle<CallTargetResultInt> _CT_RetInt_00CA_addPartyMember;
    private FhMethodHandle<CallTargetResultInt> _CT_RetInt_00CB_removePartyMember;
    private FhMethodHandle<CallTargetResultInt> _CT_RetInt_00E7_putPartyMemberInSlot;
    private FhMethodHandle<CallTargetInit> _CT_Init_7002_launchBattle;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte MsGetSavePlyJoined(byte chr_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MsSetSavePlyJoined(int chr_id, int enable);

    // I don't think this is necessary?
    // But we want to make *real* sure this thing works lol
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FUN_00786a10(uint frontline_0, uint frontline_1, uint frontline_2);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int MsBattleLabelExe(uint param_1, byte param_2, byte param_3);

    public SoloModule(SoloModuleConfig moduleConfig) : base(moduleConfig) {
        _moduleConfig = moduleConfig;
        desired_chr_id = moduleConfig.chrID;

        _MsGetSavePlyJoined = new FhMethodHandle<MsGetSavePlyJoined>(this, 0x385460, soloMsGetSavePlyJoined);
        _MsSetSavePlyJoined = new FhMethodHandle<MsSetSavePlyJoined>(this, 0x386a70, soloMsSetSavePlyJoined);
        _CT_RetInt_00CA_addPartyMember = new FhMethodHandle<CallTargetResultInt>(this, 0x45b5a0, soloCT_RetInt_00CA_addPartyMember);
        _CT_RetInt_00CB_removePartyMember = new FhMethodHandle<CallTargetResultInt>(this, 0x45b6c0, soloCT_RetInt_00CB_removePartyMember);
        _CT_RetInt_00E7_putPartyMemberInSlot = new FhMethodHandle<CallTargetResultInt>(this, 0x45bc90, soloCT_RetInt_00E7_putPartyMemberInSlot);
        _CT_Init_7002_launchBattle = new FhMethodHandle<CallTargetInit>(this, 0x3a3550, soloCT_Init_7002_launchBattle);

        _moduleState  = FhModuleState.InitSuccess;
    }

    public override FhModuleConfig ModuleConfiguration {
        get { return _moduleConfig; }
    }

    public override bool FhModuleOnError() {
        return true;
    }

    public override bool FhModuleInit() {
        return true;
    }

    public override bool FhModuleStart() {
        return
            _MsGetSavePlyJoined.hook()
         && _CT_RetInt_00CA_addPartyMember.hook()
         && _CT_RetInt_00CB_removePartyMember.hook()
         && _CT_RetInt_00E7_putPartyMemberInSlot.hook()
         && _CT_Init_7002_launchBattle.hook();
    }

    public override bool FhModuleStop() {
        return
            _MsGetSavePlyJoined.unhook()
         && _CT_RetInt_00CA_addPartyMember.unhook()
         && _CT_RetInt_00CB_removePartyMember.unhook()
         && _CT_RetInt_00E7_putPartyMemberInSlot.unhook()
         && _CT_Init_7002_launchBattle.unhook();
    }

    public byte soloMsGetSavePlyJoined(byte idx) {
        return (byte)(idx == desired_chr_id ? 1 : 0);
    }

    public void soloMsSetSavePlyJoined(int chr_id, int enable) { }

    public int soloCT_RetInt_00CA_addPartyMember(AtelBasicWorker* work, nint* storage, AtelStack* stack) {
        int chr_id = stack->pop_int(); // we don't really care
        FhLog.Info($"Call: addPartyMember({chr_id})");
        FhUtil.get_fptr<FUN_00786a10>(0x386a10)(desired_chr_id, 0xff, 0xff); // just making sure...
        return (int)desired_chr_id;
    }

    public int soloCT_RetInt_00CB_removePartyMember(AtelBasicWorker* work, nint* storage, AtelStack* stack) {
        int chr_id = stack->pop_int(); // we don't really care
        FhLog.Info($"Call: removePartyMember({chr_id})");
        FhUtil.get_fptr<FUN_00786a10>(0x386a10)(desired_chr_id, 0xff, 0xff); // just making sure...
        return 0;
    }

    public int soloCT_RetInt_00E7_putPartyMemberInSlot(AtelBasicWorker* work, nint* storage, AtelStack* stack) {
        int chr_id = stack->pop_int(); // we don't really care
        int slot = stack->pop_int(); // we don't really care
        FhLog.Info($"Call: putPartyMemberInSlot({slot}, {chr_id})");
        FhUtil.get_fptr<FUN_00786a10>(0x386a10)(desired_chr_id, 0xff, 0xff); // just making sure...
        return (int)desired_chr_id;
    }

    public void soloCT_Init_7002_launchBattle(AtelBasicWorker* work, nint* storage, AtelStack* stack) {
        int b = stack->pop_int();
        int a = stack->pop_int();
        FhLog.Info($"Call: launchBattle({a}, {b})");
        FhUtil.get_fptr<MsBattleLabelExe>(0x381d60)((uint)a, 1, (byte)b);
    }
}

