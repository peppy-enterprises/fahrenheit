namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x700)]
public unsafe struct AtelBasicWorker {
    [FieldOffset(0x0)] public AtelScriptHeader* script_header;
    [FieldOffset(0x4)] public AtelScriptChunk* script_chunk;
    [FieldOffset(0xC)] public ushort event_actor_total;

    [FieldOffset(0x2C)] public ushort entry_point_idx;
    [FieldOffset(0x2E)] public ushort worker_idx;

    [FieldOffset(0x32)] public byte current_thread_priority;
    [FieldOffset(0x34)] public ushort __0x34;
    [FieldOffset(0x36)] public ushort __0x36;

    [FieldOffset(0x44)] public ushort* request_table;
    [FieldOffset(0x48)] public fixed int reg_int[4];
    [FieldOffset(0x58)] public fixed float reg_float[10];
    [FieldOffset(0x80)] public AtelSignal* pending_signal_queue;
    [FieldOffset(0x88)] public AtelSignal* free_signals;

    [FieldOffset(0x90)] public AtelSignal* current_signal;
    [FieldOffset(0x94)] public float __0x94;
    [FieldOffset(0x98)] public float __0x98;
    [FieldOffset(0x9C)] public Chr* chr_handle;
    [FieldOffset(0xA8)] public ushort event_chr_id;
    [FieldOffset(0xC4)] public AtelStack stack;
    [FieldOffset(0x12C)] public AtelWorkThread* threads; // [9]

    public readonly byte* code_ptr => (byte*)((nint)script_chunk + script_chunk->code_offset);

    public readonly uint* event_data_table => (uint*)((nint)script_chunk + script_chunk->event_data_offset);

    public readonly AtelScriptVar* var_table => (AtelScriptVar*)((nint)script_chunk + script_header->var_table_offset);

    public readonly int* int_table => (int*)((nint)script_chunk + script_header->int_table_offset);

    public readonly float* float_table => (float*)((nint)script_chunk + script_header->float_table_offset);

    public readonly uint* script_table => (uint*)((nint)script_chunk + script_header->script_begin_table_offset);

    public readonly uint* jump_table => (uint*)((nint)script_chunk + script_header->jumps_begin_table_offset);

    public readonly uint* data_table => (uint*)((nint)script_chunk + script_header->data_offset);

    public readonly uint* priv_data_table => (uint*)((nint)script_chunk + script_header->priv_data_offset);

    public readonly uint* shared_data_table => (uint*)((nint)script_chunk + script_header->shared_data_offset);

    public readonly AtelWorkThread* current_thread => threads + current_thread_priority;

    public int pc_of(AtelWorkThread* thread) => (int)(thread->instruction_ptr - code_ptr);
}