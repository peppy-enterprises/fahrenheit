/* [fkelava 13/9/22 08:11]
 * Based entirely on my own observations of game memory and decompilation.
 */

namespace Fahrenheit.CoreLib;

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto, Pack = 4, Size = 0x880)]
public unsafe struct FhXActor
{
    [FieldOffset(0x0)]   public ushort chr_id;
    [FieldOffset(0x2)]   public ushort chr_enabled;
    [FieldOffset(0x4)]   public string chr_name; // actually a pointer; marshalling automatically corrects this
    [FieldOffset(0xC)]   public Vec4f  chr_pos_vec;
    [FieldOffset(0x1C)]  public Vec4f  chr_pos_vec_bkup;
    [FieldOffset(0x5C)]  public Vec4f  chr_scale_vec;
    [FieldOffset(0x6C)]  public Vec4f  chr_offset_vec;
    [FieldOffset(0x154)] public float  chr_speed;
    [FieldOffset(0x158)] public float  chr_rotation_rad;
    [FieldOffset(0x170)] public float  chr_run_anim_spd_threshold;
    [FieldOffset(0x330)] public LVec3f chr_shade_R_vec;
    [FieldOffset(0x33C)] public LVec3f chr_shade_G_vec;
    [FieldOffset(0x348)] public LVec3f chr_shade_B_vec;
    [FieldOffset(0x354)] public LVec3f chr_transparency_vec;
    [FieldOffset(0x414)] public float  chr_neck_mot_target_pct;
    [FieldOffset(0x418)] public float  chr_neck_mot_actual_pct;
    [FieldOffset(0x41C)] public float  chr_neck_facing_target_pct;
    [FieldOffset(0x420)] public float  chr_neck_speed;
    [FieldOffset(0x424)] public float  chr_neck_rot_limit_l; // in deg
    [FieldOffset(0x428)] public float  chr_neck_rot_limit_h; // in deg
    [FieldOffset(0x42C)] public Mat4f  chr_neck_matrix_0; // battle only during targeting
    [FieldOffset(0x46C)] public Mat4f  chr_neck_matrix_1; // battle only during targeting
    // ? character sub-positions (limbs et al.)
    [FieldOffset(0x524)] public Vec4f  chr_optpos_vec_0;
    [FieldOffset(0x534)] public Vec4f  chr_optpos_vec_1;
    [FieldOffset(0x544)] public Vec4f  chr_optpos_vec_2;
    [FieldOffset(0x554)] public Vec4f  chr_optpos_vec_3;
    [FieldOffset(0x564)] public Vec4f  chr_optpos_vec_4;
    [FieldOffset(0x574)] public Vec4f  chr_optpos_vec_5;
    [FieldOffset(0x584)] public Vec4f  chr_optpos_vec_6;
    [FieldOffset(0x594)] public Vec4f  chr_optpos_vec_7;
    [FieldOffset(0x5A4)] public Vec4f  chr_optpos_vec_8;
    [FieldOffset(0x5B4)] public Vec4f  chr_optpos_vec_9;
    [FieldOffset(0x5C4)] public Vec4f  chr_optpos_vec_10;
    [FieldOffset(0x5D4)] public Vec4f  chr_optpos_vec_11;
    [FieldOffset(0x5F4)] public Vec4f  chr_optpos_vec_12;
    [FieldOffset(0x604)] public Vec4f  chr_optpos_vec_13;
    [FieldOffset(0x614)] public Vec4f  chr_optpos_vec_14;
    [FieldOffset(0x624)] public Vec4f  chr_optpos_vec_15;
    [FieldOffset(0x634)] public Vec4f  chr_optpos_vec_16;
    [FieldOffset(0x644)] public Vec4f  chr_optpos_vec_17;
    [FieldOffset(0x654)] public Vec4f  chr_optpos_vec_18;
    [FieldOffset(0x664)] public Vec4f  chr_optpos_vec_19;
    [FieldOffset(0x674)] public Vec4f  chr_optpos_vec_20;
    [FieldOffset(0x684)] public Vec4f  chr_optpos_vec_21;
    [FieldOffset(0x694)] public Vec4f  chr_optpos_vec_22;
    [FieldOffset(0x6A4)] public Vec4f  chr_optpos_vec_23;
    [FieldOffset(0x6B4)] public Vec4f  chr_optpos_vec_24;
    [FieldOffset(0x6C4)] public Vec4f  chr_optpos_vec_25;
}
