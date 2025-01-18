/* [fkelava 7/5/23 17:25]
 * source: MS Store ver.
 * header: __CAMERA_ATEL__
 *
 * /ffx_ps2/ffx/proj2/chr/ath/battle/camera.ath
 *
 * partial
 */
namespace Fahrenheit.Core.FFX.Ids;

public enum CamTargetId : T_XCamTargetId {
    CAM_CHR_NOP         = -1,
    CAM_CHR_ACTIVE      = -2,
    CAM_CHR_TARGET      = -3,
    CAM_CHR_TARGET_NOW  = -4,
    CAM_CHR_ALL         = -5,
    CAM_CHR_PARTY1      = -6,
    CAM_CHR_PARTY2      = -7,
    CAM_CHR_PARTY3      = -8,
    CAM_CHR_PARTY4      = -9,
    CAM_CHR_PARTY5      = -10,
    CAM_CHR_PARTY6      = -11,
    CAM_CHR_PARTY7      = -12,
    CAM_CHR_OWN         = -13,
    CAM_CHR_ALL_PLY     = -14,
    CAM_CHR_ALL_MON     = -15,
    CAM_CHR_OWN_TARGET  = -16,
    CAM_CHR_REACTION    = -17,
    CAM_CHR_INPUT       = -18,
    CAM_CHR_OWN_TARGET0 = -26,
}

public enum CamMoveType : T_XCamMoveType {
    CAM_MOVE_X      = 1,
    CAM_MOVE_Y      = 2,
    CAM_MOVE_Z      = 4,
    CAM_MOVE_THETA  = CAM_MOVE_X,
    CAM_MOVE_PHI    = CAM_MOVE_Y,
    CAM_MOVE_LEN    = CAM_MOVE_Z,
    CAM_MOVE_POS    = (CAM_MOVE_X + CAM_MOVE_Y + CAM_MOVE_Z),
    CAM_MOVE_ROLL   = 8,
    CAM_MOVE_SCRDPT = 16,
}

public enum FhXCamSplineType : T_XCamSplineType {
    CAM_SPLINE_2       = 2,
    CAM_SPLINE_3       = 3,
    CAM_SPLINE_TANGENT = 4,
}