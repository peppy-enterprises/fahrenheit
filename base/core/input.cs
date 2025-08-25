using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fahrenheit.Core; 
public unsafe class FhInput {

    private static readonly nint _address = FhGlobal.game_type == FhGameType.FFX ? 0xF27080 : 0xD94A8C;

    public readonly InputAction l2       = new(0x1);
    public readonly InputAction r2       = new(0x2);
    public readonly InputAction l1       = new(0x4);
    public readonly InputAction r1       = new(0x8);
    public readonly InputAction square   = new(0x10);
    public readonly InputAction confirm  = new(0x20);
    public readonly InputAction cancel   = new(0x40);
    public readonly InputAction triangle = new(0x80);
    public readonly InputAction select   = new(0x100);
    public readonly InputAction start    = new(0x800);
    public readonly InputAction up       = new(0x1000);
    public readonly InputAction right    = new(0x2000);
    public readonly InputAction down     = new(0x4000);
    public readonly InputAction left     = new(0x8000);

    public static ushort* raw { get { return FhUtil.ptr_at<ushort>(_address); } }

    private static ushort previous;

    public void update() {
        previous = *raw;
    }

    public void consume_all() {
        previous = *raw = 0;
    }

    public class InputAction {
        private ushort mask;

        public InputAction(ushort mask) {
            this.mask = mask;
        }

        public (bool held, bool just_pressed, bool just_released) consume() {
            (bool, bool, bool) ret = (held, just_pressed, just_released);
            *raw &= (ushort)~mask;
            return ret;
        }

        public bool held { get { return (*raw & mask) != 0; } }
        public bool just_pressed { get { return (*raw & mask) != 0 && (previous & mask) == 0; } }
        public bool just_released { get { return (*raw & mask) == 0 && (previous & mask) != 0; } }
    }
}
