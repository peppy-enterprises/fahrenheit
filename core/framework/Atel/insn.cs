// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core.Atel;

public record struct AtelInst(byte instruction, ushort? operand) {
    public byte[] to_bytes() {
        byte[] bytes = new byte[operand.HasValue ? 3 : 1];

        bytes[0] = instruction;
        if (operand.HasValue) {
            bytes[1] = (byte)operand.Value;
            bytes[2] = (byte)(operand.Value >> 8);
        }

        return bytes;
    }
}

public enum AtelOp : byte {
    NOP        = 0x0,
    LOR        = 0x1,
    LAND       = 0x2,
    OR         = 0x3,
    EOR        = 0x4,
    AND        = 0x5,
    EQ         = 0x6,
    NE         = 0x7,
    GTU        = 0x8,
    LSU        = 0x9,
    GT         = 0xA,
    LS         = 0xB,
    GTEU       = 0xC,
    LSEU       = 0xD,
    GTE        = 0xE,
    LSE        = 0xF,
    BON        = 0x10,
    BOFF       = 0x11,
    SLL        = 0x12,
    SRL        = 0x13,
    ADD        = 0x14,
    SUB        = 0x15,
    MUL        = 0x16,
    DIV        = 0x17,
    MOD        = 0x18,
    NOT        = 0x19,
    UMINUS     = 0x1A,
    FIXADRS    = 0x1B,
    BNOT       = 0x1C,
    LABEL      = 0x9D,
    TAG        = 0x9E,
    PUSHV      = 0x9F,
    POPV       = 0xA0,
    POPVL      = 0xA1,
    PUSHAR     = 0xA2,
    POPAR      = 0xA3,
    POPARL     = 0xA4,
    POPA       = 0x25,
    PUSHA      = 0x26,
    PUSHARP    = 0xA7,
    PUSHX      = 0x28,
    PUSHY      = 0x29,
    POPX       = 0x2A,
    REPUSH     = 0x2B,
    POPY       = 0x2C,
    PUSHI      = 0xAD,
    PUSHII     = 0xAE,
    PUSHF      = 0xAF,
    JMP        = 0xB0,
    CJMP       = 0xB1,
    NCJMP      = 0xB2,
    JSR        = 0xB3,
    RTS        = 0x34,
    CALL       = 0xB5,
    REQ        = 0x36, // Request execution
    REQSW      = 0x37, // Request execution (wait for start)
    REQEW      = 0x38, // Request execution (wait for end)
    PREQ       = 0x39, // "Player" request execution
    PREQSW     = 0x3A, // "Player" request execution (wait for start)
    PREQEW     = 0x3B, // "Player" request execution (wait for end)
    RET        = 0x3C,
    RETN       = 0x3D,
    RETT       = 0x3E,
    RETTN      = 0x3F,
    HALT       = 0x40,
    PUSHN      = 0xC1,
    PUSHT      = 0xC2,
    PUSHVP     = 0xC3,
    PUSHFIX    = 0xC4,
    FREQ       = 0x45, // Request execution (Force)
    TREQ       = 0x46, // Request execution (if it is not already requested)
    BREQ       = 0x47, // Request execution (Branching)
    BFREQ      = 0x48, // Request execution (Branching) (Force)
    BTREQ      = 0x49, // Request execution (Branching) (if it is not already requestedd)
    FREQSW     = 0x4A, // Request execution (Force) (wait for start)
    TREQSW     = 0x4B, // Request execution (if it is not already requested) (wait for start)
    BREQSW     = 0x4C, // Request execution (Branching) (wait for start)
    BFREQSW    = 0x4D, // Request execution (Branching) (Force) (wait for start)
    BTREQSW    = 0x4E, // Request execution (Branching) (if it is not already requested) (wait for start)
    FREQEW     = 0x4F, // Request execution (Force) (wait for end)
    TREQEW     = 0x50, // Request execution (if it is not already requested) (wait for end)
    BREQEW     = 0x51, // Request execution (Branching) (wait for end)
    BFREQEW    = 0x52, // Request execution (Branching) (Force) (wait for end)
    BTREQEW    = 0x53, // Request execution (Branching) (if it is not already requested) (wait for end)
    DRET       = 0x54,
    POPXJMP    = 0xD5,
    POPXCJMP   = 0xD6,
    POPXNCJMP  = 0xD7,
    CALLPOPA   = 0xD8,
    POPI0      = 0x59,
    POPI1      = 0x5A,
    POPI2      = 0x5B,
    POPI3      = 0x5C,
    POPF0      = 0x5D,
    POPF1      = 0x5E,
    POPF2      = 0x5F,
    POPF3      = 0x60,
    POPF4      = 0x61,
    POPF5      = 0x62,
    POPF6      = 0x63,
    POPF7      = 0x64,
    POPF8      = 0x65,
    POPF9      = 0x66,
    PUSHI0     = 0x67,
    PUSHI1     = 0x68,
    PUSHI2     = 0x69,
    PUSHI3     = 0x6A,
    PUSHF0     = 0x6B,
    PUSHF1     = 0x6C,
    PUSHF2     = 0x6D,
    PUSHF3     = 0x6E,
    PUSHF4     = 0x6F,
    PUSHF5     = 0x70,
    PUSHF6     = 0x71,
    PUSHF7     = 0x72,
    PUSHF8     = 0x73,
    PUSHF9     = 0x74,
    PUSHAINTER = 0xF5,
    SYSTEM     = 0xF6,
    REQWAIT    = 0x77,
    PREQWAIT   = 0x78,
    REQCHG     = 0x79,
    ACTREQ     = 0x7A,
}

public static class AtelOpExt {
    public static bool has_operand(this AtelOp inst) {
        return ((byte)inst & 0x80) != 0;
    }

    public static AtelInst build(this AtelOp inst, ushort? operand = null) {
        if (!inst.has_operand() && operand.HasValue) throw new ArgumentException($"Tried to build an AtelOpCode with an operand and instruction that doesn't take an operand.");
        return new AtelInst { instruction = (byte)inst, operand = operand };
    }
}
