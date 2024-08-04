/* [fkelava 7/5/23 15:37]
 * Many primitive IDs and enumerations in this game are given merely as `#define`s. What the true underlying type is is unknown.
 *
 * Fahrenheit introduces strongly typed constants for all such IDs and enumerations. Because I have to guess the underlying type,
 * odds are I got it wrong. For this reason I introduce type aliases for each; this way you can change the underlying datatype
 * of any primitive declaration here, in one spot. You should follow this approach when adding new constants.
 */

// Common
global using T_FhDialogueOptionCount = System.Byte;
global using T_FhDialoguePos         = System.UInt16;

// X
global using T_XCamMoveType   = System.Byte;   // x_idcam
global using T_XCamSplineType = System.Byte;   // x_idcam
global using T_XPlySaveId     = System.Byte;   // x_idplysave
global using T_XSeTypeId      = System.Byte;   // x_idsetype
global using T_XCamTargetId   = System.Int16;  // x_idcam
global using T_XChrStatId     = System.Int32;  // x_idchrstat
global using T_XTargetId      = System.Int32;  // x_idtgt
global using T_XBtlMapId      = System.UInt16; // x_idbtlmap
global using T_XItemId        = System.UInt16; // x_iditem
global using T_XMdlElemId     = System.Int32;  // x_idmdlelem
global using T_XMonId         = System.UInt16; // x_idmon
global using T_XStdMotId      = System.UInt16; // x_idmotstd
global using T_XNpcId         = System.UInt16; // x_idnpc
global using T_XObjId         = System.UInt16; // x_idobj
global using T_XPcId          = System.UInt16; // x_idpc
global using T_XPComId        = System.UInt16; // x_idpcom
global using T_XSklId         = System.UInt16; // x_idskl
global using T_XSumId         = System.UInt16; // x_idsum
global using T_XBtlId         = System.UInt32; // x_idbtl
global using T_XBtlVoiceId    = System.UInt32; // x_idbtlvo
global using T_XMotId         = System.UInt32; // x_idmot-*

// X-2
global using T_X2PComId    = System.UInt16; // x2_idpcom
global using T_X2BtlId     = System.UInt32; // x2_idbtl
global using T_X2ChrStatId = System.Int32;  // x2_idchrstat

// Rust-like aliases for those that prefer them.
global using u8    = System.Byte;
global using u16   = System.UInt16;
global using u32   = System.UInt32;
global using u64   = System.UInt64;
global using u128  = System.UInt128;
global using usize = System.UIntPtr;

global using i8    = System.SByte;
global using i16   = System.Int16;
global using i32   = System.Int32;
global using i64   = System.Int64;
global using i128  = System.Int128;
global using isize = System.IntPtr;

global using f32   = System.Single;
global using f64   = System.Double;
