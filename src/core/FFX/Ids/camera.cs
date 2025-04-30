/* [fkelava 7/5/23 17:25]
 * source: MS Store ver.
 * header: __CAMERA_ATEL__
 *
 * /ffx_ps2/ffx/proj2/chr/ath/battle/camera.ath
 *
 * partial
 */
namespace Fahrenheit.Core.FFX.Ids;

public static class CamTargetId {
    public const T_XCamTargetId CAM_CHR_NOP         = -1;
    public const T_XCamTargetId CAM_CHR_ACTIVE      = -2;
    public const T_XCamTargetId CAM_CHR_TARGET      = -3;
    public const T_XCamTargetId CAM_CHR_TARGET_NOW  = -4;
    public const T_XCamTargetId CAM_CHR_ALL         = -5;
    public const T_XCamTargetId CAM_CHR_PARTY1      = -6;
    public const T_XCamTargetId CAM_CHR_PARTY2      = -7;
    public const T_XCamTargetId CAM_CHR_PARTY3      = -8;
    public const T_XCamTargetId CAM_CHR_PARTY4      = -9;
    public const T_XCamTargetId CAM_CHR_PARTY5      = -10;
    public const T_XCamTargetId CAM_CHR_PARTY6      = -11;
    public const T_XCamTargetId CAM_CHR_PARTY7      = -12;
    public const T_XCamTargetId CAM_CHR_OWN         = -13;
    public const T_XCamTargetId CAM_CHR_ALL_PLY     = -14;
    public const T_XCamTargetId CAM_CHR_ALL_MON     = -15;
    public const T_XCamTargetId CAM_CHR_OWN_TARGET  = -16;
    public const T_XCamTargetId CAM_CHR_REACTION    = -17;
    public const T_XCamTargetId CAM_CHR_INPUT       = -18;
    public const T_XCamTargetId CAM_CHR_OWN_TARGET0 = -26;
}

public static class CamMoveType {
    public const T_XCamMoveType CAM_MOVE_X      = 1;
    public const T_XCamMoveType CAM_MOVE_Y      = 2;
    public const T_XCamMoveType CAM_MOVE_Z      = 4;
    public const T_XCamMoveType CAM_MOVE_THETA  = CAM_MOVE_X;
    public const T_XCamMoveType CAM_MOVE_PHI    = CAM_MOVE_Y;
    public const T_XCamMoveType CAM_MOVE_LEN    = CAM_MOVE_Z;
    public const T_XCamMoveType CAM_MOVE_POS    = CAM_MOVE_X | CAM_MOVE_Y | CAM_MOVE_Z;
    public const T_XCamMoveType CAM_MOVE_ROLL   = 8;
    public const T_XCamMoveType CAM_MOVE_SCRDPT = 16;
}

public static class CamSplineType {
    public const T_XCamSplineType CAM_SPLINE_2       = 2;
    public const T_XCamSplineType CAM_SPLINE_3       = 3;
    public const T_XCamSplineType CAM_SPLINE_TANGENT = 4;
}
