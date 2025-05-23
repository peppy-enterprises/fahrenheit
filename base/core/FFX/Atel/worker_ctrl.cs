namespace Fahrenheit.Core.FFX.Atel;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x238)]
public unsafe struct AtelWorkerController {
    [FieldOffset(0xC)]   public  ushort           runnable_script_count;
    [FieldOffset(0x14)]  public  ushort           count_0x5b8;
    [FieldOffset(0x16)]  public  ushort           count_0x558;
    [FieldOffset(0x18)]  public  ushort           count_0x2e8;
    [FieldOffset(0x1C)]  private AtelBasicWorker* _workers;
    [FieldOffset(0x1E8)] public  short            __0x1E8; // `npc_last_interaction`, probably the index of the last-interacted-with worker
    [FieldOffset(0x1F8)] public  AtelScriptChunk* script_chunk;
    [FieldOffset(0x1FC)] private byte*            __0x1FC;

    public readonly ushort count_0xb58 => (ushort)(runnable_script_count - count_0x5b8 - count_0x558 - count_0x2e8);

    public readonly AtelBasicWorker* worker(int idx) {
        int missed = 0;

        if (idx < count_0xb58) return (AtelBasicWorker*)((nint)_workers + (idx * 0xb58));

        idx    -= count_0xb58;
        missed += count_0xb58 * 0xb58;

        if (idx < count_0x558) return (AtelBasicWorker*)((nint)_workers + (idx * 0x558) + missed);

        idx    -= count_0x558;
        missed += count_0x558 * 0x558;

        if (idx < count_0x5b8) return (AtelBasicWorker*)((nint)_workers + (idx * 0x5b8) + missed);

        idx    -= count_0x5b8;
        missed += count_0x5b8 * 0x5b8;

        if (idx < count_0x2e8) return (AtelBasicWorker*)((nint)_workers + (idx * 0x2e8) + missed);

        idx    -= count_0x2e8;
        missed += count_0x2e8 * 0x2e8;

        return (AtelBasicWorker*)((nint)_workers + (idx * 0x30) + missed);
    }

    public readonly AtelBasicWorker* worker_b58(int idx) {
        if (idx < count_0xb58) return (AtelBasicWorker*)((nint)_workers + (idx * 0xb58));

        throw new IndexOutOfRangeException($"Index {idx} is out of range for {count_0xb58} B58 workers.");
    }
}
