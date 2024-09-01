namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x238)]
public unsafe struct AtelWorkerController {
    [FieldOffset(0x1F8)] public  AtelScriptChunk* script_chunk;
    [FieldOffset(0x1FC)] private byte*            __0x1FC;
}
