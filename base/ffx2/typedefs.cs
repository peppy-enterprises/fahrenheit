// SPDX-License-Identifier: MIT

/* [fkelava 7/5/23 15:37]
 * Many primitive IDs and enumerations in this game are given merely as `#define`s. What the true underlying type is is unknown.
 *
 * Fahrenheit introduces strongly typed constants for all such IDs and enumerations. Because I have to guess the underlying type,
 * odds are I got it wrong. For this reason I introduce type aliases for each; this way you can change the underlying datatype
 * of any primitive declaration here, in one spot. You should follow this approach when adding new constants.
 */

global using T_X2CommandId         = System.UInt16; // x2_idpcom
global using T_X2BtlId             = System.UInt32; // x2_idbtl
global using T_X2ChrStatId         = System.Int32;  // x2_idchrstat
global using T_X2SeTypeId          = System.Byte;   // ids/setype
global using T_X2BtlSeTypeId       = System.Byte;   // ids/btl_setype
global using T_X2BtlRequestTagId   = System.Int16;  // ids/btl_req_tags
global using T_X2BtlRequestActorId = System.Int16;  // ids/btl_req_tags
global using T_X2BtlVoiceId        = System.UInt32; // ids/btl_voice
global using T_X2PlySaveId         = System.Int32;  // ids/plysave
global using T_X2JobId             = System.Int32;  // ids/job
