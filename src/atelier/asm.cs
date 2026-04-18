// SPDX-License-Identifier: MIT

using System.Runtime.CompilerServices;

using Fahrenheit.Atel;

namespace Fahrenheit.Tools.Atelier;

public class AtelAssembler {

    public int disassemble_insn(ReadOnlySpan<byte> chunk, int offset) {
        byte op_byte    = chunk[offset];
        bool op_has_imm = op_byte.get_bit (7);
        byte op_code    = op_byte.get_bits(0, 7);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(op_code, 0x7A);

        int    op_size = op_has_imm ? 3 : 1;
        AtelOp op      = Unsafe.As<byte, AtelOp>(ref op_byte);

        if (op_has_imm) {
            byte imm_ptr = chunk[offset + 1];

            if (op is AtelOp.PUSHII) {
                ATEL_IMM_PUSHII op_imm = Unsafe.As<byte, ATEL_IMM_PUSHII>(ref imm_ptr);
                Console.WriteLine($"{offset:X4} {op}({op_imm})");
            }
            else {
                ATEL_IMM op_imm = Unsafe.As<byte, ATEL_IMM>(ref imm_ptr);
                Console.WriteLine($"{offset:X4} {op}({op_imm})");
            }
        }
        else {
            Console.WriteLine($"{offset:X4} {op}");
        }

        return offset + op_size;
    }

    public void disassemble(Span<byte> chunk, string name) {
        Console.WriteLine($"--- {name} ---");

        for (int offset = 0; offset < chunk.Length;) {
            offset = disassemble_insn(chunk, offset);
        }
    }
}
