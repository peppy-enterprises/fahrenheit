﻿/* [fkelava 7/5/23 15:37]
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
global using T_XCamMoveType      = System.Byte;   // ids/camera
global using T_XCamSplineType    = System.Byte;   // ids/camera
global using T_XPlySaveId        = System.Int32;  // ids/plysave
global using T_XSeTypeId         = System.Byte;   // ids/setype
global using T_XAtelPurposeId    = System.Byte;   // ids/atel_purpose
global using T_XCamTargetId      = System.Int16;  // ids/camera
global using T_XChrStatId        = System.Int32;  // ids/chr_stat
global using T_XTargetId         = System.Int32;  // ids/target
global using T_XBtlMapId         = System.UInt16; // ids/btl_map
global using T_XModelElementId   = System.Int32;  // ids/mdl_elem
global using T_XMonsterId        = System.UInt16; // ids/mon
global using T_XBtlMotionId      = System.UInt16; // ids/mot_btl
global using T_XNpcId            = System.UInt16; // ids/npc
global using T_XObjId            = System.UInt16; // ids/object
global using T_XPcModelId        = System.UInt16; // ids/mdl_pc
global using T_XCommandId        = System.UInt16; // ids/com_boss, com_mon, com_ply, item
global using T_XSkeletonId       = System.UInt16; // ids/mdl_skl
global using T_XSummonId         = System.UInt16; // ids/summon
global using T_XBtlId            = System.UInt32; // ids/btl
global using T_XBtlVoiceId       = System.UInt32; // ids/btl_voice
global using T_XBtlResultId      = System.Int32;  // ids/btl_result
global using T_XBtlDbgFlagId     = System.Int32;  // ids/btl_dbg
global using T_XMotionId         = System.UInt32; // ids/mot
global using T_XMesWinPosId      = System.Byte;   // ids/meswin
global using T_XDeathAnimationId = System.Byte;   // ids/anim_death
global using T_XSubModelId       = System.Int32;  // ids/mdl_sub

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
