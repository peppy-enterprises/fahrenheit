namespace Fahrenheit.CoreLib.FFX;

[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 0x700)]
public unsafe struct AtelBasicWorker
{
    [FieldOffset(0x0)] public AtelScriptHeader* script_header;
    [FieldOffset(0x4)] public AtelScriptChunk*  script_chunk;
    [FieldOffset(0xC)] public ushort            event_actor_total;

    [FieldOffset(0x2C)] public ushort entry_point_idx;
    [FieldOffset(0x2E)] public ushort worker_idx;

    [FieldOffset(0x32)] public byte   current_thread_priority;
    [FieldOffset(0x34)] public ushort __0x34;
    [FieldOffset(0x36)] public ushort __0x36;

    [FieldOffset(0x44)] public       ushort*     request_table;
    [FieldOffset(0x48)] public fixed int         reg_int[4];
    [FieldOffset(0x58)] public fixed float       reg_float[10];
    [FieldOffset(0x80)] public       AtelSignal* pending_signal_queue;
    [FieldOffset(0x88)] public       AtelSignal* free_signals;

    [FieldOffset(0x90)]  public AtelSignal*     current_signal;
    [FieldOffset(0x94)]  public float           __0x94;
    [FieldOffset(0x98)]  public float           __0x98;
    [FieldOffset(0x9C)]  public Chr*            chr_handle;
    [FieldOffset(0xA8)]  public ushort          event_chr_id;
    [FieldOffset(0xC4)]  public AtelStack       stack;
    [FieldOffset(0x12C)] public AtelWorkThread* threads; // [9]

    public readonly byte*           code_ptr          { get { return (byte*)         ((nint)script_chunk + script_chunk ->offset_code);               } }
    public readonly uint*           table_event_data  { get { return (uint*)         ((nint)script_chunk + script_chunk ->offset_event_data);         } }
    public readonly AtelScriptVar*  table_var         { get { return (AtelScriptVar*)((nint)script_chunk + script_header->offset_var_table);          } }
    public readonly int*            table_int         { get { return (int*)          ((nint)script_chunk + script_header->offset_int_table);          } }
    public readonly float*          table_float       { get { return (float*)        ((nint)script_chunk + script_header->offset_float_table);        } }
    public readonly uint*           table_script      { get { return (uint*)         ((nint)script_chunk + script_header->offset_script_begin_table); } }
    public readonly uint*           table_jump        { get { return (uint*)         ((nint)script_chunk + script_header->offset_jumps_begin_table);  } }
    public readonly uint*           table_data        { get { return (uint*)         ((nint)script_chunk + script_header->offset_data);               } }
    public readonly uint*           table_priv_data   { get { return (uint*)         ((nint)script_chunk + script_header->offset_priv_data);          } }
    public readonly uint*           table_shared_data { get { return (uint*)         ((nint)script_chunk + script_header->offset_shared_data);        } }
    public readonly AtelWorkThread* current_thread    { get { return threads + current_thread_priority;                                               } }

    public int pc_of(AtelWorkThread* thread) => (int)(thread->instruction_ptr - code_ptr);
}