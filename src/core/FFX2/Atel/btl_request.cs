/* [fkelava 11/5/25 08:11]
 * source: Switch ver.
 * header: _ms_header_battle_actor
 *
 * /ffx_ps2/ffx2/master/jppc/battle/header/btlactor.h
 */

namespace Fahrenheit.Core.FFX2.Atel;

[StructLayout(LayoutKind.Sequential)]
public readonly struct AtelBtlRequestData {
    public readonly short space;
    public readonly short binhdr;
    public readonly short actor;
    public readonly short tag;
}
