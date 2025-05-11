/* [fkelava 11/5/25 08:11]
 * source: Switch ver.
 * header: _ms_header_battle_actor
 *
 * /ffx_ps2/ffx2/master/jppc/battle/header/btlactor.h
 */

namespace Fahrenheit.Core.FFX2.Ids;

// システムアクター一覧
public static class BtlRequestActorId {
    public const T_X2BtlRequestActorId ACTOR_SYSTEM                           =   0; //   0:動き:システム
    public const T_X2BtlRequestActorId ACTOR_SCENE                            =   1; //   1:動き:シーン
    public const T_X2BtlRequestActorId ACTOR_MAP                              =   2; //   2:動き:マップ
    public const T_X2BtlRequestActorId ACTOR_SYSTEM_CHECK                     =   3; //   3:判定:システムチェック
    public const T_X2BtlRequestActorId ACTOR_SCENE_CHECK                      =   4; //   4:判定:シーンチェック

    public const T_X2BtlRequestActorId ACTOR_CAMERA1                          =   5; //   5:動き:カメラ１
    public const T_X2BtlRequestActorId ACTOR_CAMERA2                          =   6; //   6:動き:カメラ２
    public const T_X2BtlRequestActorId ACTOR_CAMERA3                          =   7; //   7:動き:カメラ３

    public const T_X2BtlRequestActorId ACTOR_COMMAND_CAMERA1                  =   8; //   8:動き:コマンドカメラ１
    public const T_X2BtlRequestActorId ACTOR_COMMAND_CAMERA2                  =   9; //   9:動き:コマンドカメラ２
    public const T_X2BtlRequestActorId ACTOR_COMMAND_CAMERA3                  =  10; //  10:動き:コマンドカメラ３

    public const T_X2BtlRequestActorId ACTOR_ITEM_CAMERA1                     =  11; //  11:動き:アイテムカメラ１
    public const T_X2BtlRequestActorId ACTOR_ITEM_CAMERA2                     =  12; //  12:動き:アイテムカメラ２
    public const T_X2BtlRequestActorId ACTOR_ITEM_CAMERA3                     =  13; //  13:動き:アイテムカメラ３

    public const T_X2BtlRequestActorId ACTOR_MONMAGIC_CAMERA1                 =  14; //  14:動き:モンマジックカメラ１
    public const T_X2BtlRequestActorId ACTOR_MONMAGIC_CAMERA2                 =  15; //  15:動き:モンマジックカメラ２
    public const T_X2BtlRequestActorId ACTOR_MONMAGIC_CAMERA3                 =  16; //  16:動き:モンマジックカメラ３

    public const T_X2BtlRequestActorId ACTOR_PLAYER_VOICE                     =  17; //  17:動き:全プレイヤーボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER_VOICE                    =  18; //  18:動き:全モンスターボイス

    public const T_X2BtlRequestActorId ACTOR_PLAYER_MOTION                    =  19; //  19:動き:全プレイヤーモーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER_MOTION                   =  20; //  20:動き:全モンスターモーション

    public const T_X2BtlRequestActorId ACTOR_EFFECT_MOTION                    =  21; //  21:動き:エフェクトモーション

    public const T_X2BtlRequestActorId ACTOR_YUNA_MOTION                      =  22; //  22:動き:ユウナモーション
    public const T_X2BtlRequestActorId ACTOR_RIKKU_MOTION                     =  23; //  23:動き:リュックモーション
    public const T_X2BtlRequestActorId ACTOR_PAIN_MOTION                      =  24; //  24:動き:パインモーション
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL1_MOTION             =  25; //  25:動き:ユウナスペシャル１モーション
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL2_MOTION             =  26; //  26:動き:ユウナスペシャル２モーション
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL1_MOTION            =  27; //  27:動き:リュックスペシャル１モーション
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL2_MOTION            =  28; //  28:動き:リュックスペシャル２モーション
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL1_MOTION             =  29; //  29:動き:パインスペシャル１モーション
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL2_MOTION             =  30; //  30:動き:パインスペシャル２モーション
    public const T_X2BtlRequestActorId ACTOR_YUNA_WEAPON_MOTION               =  31; //  31:動き:ユウナ武器モーション
    public const T_X2BtlRequestActorId ACTOR_RIKKU_WEAPON_MOTION              =  32; //  32:動き:リュック武器モーション
    public const T_X2BtlRequestActorId ACTOR_PAIN_WEAPON_MOTION               =  33; //  33:動き:パイン武器モーション
    public const T_X2BtlRequestActorId ACTOR_NUDE_MOTION                      =  34; //  34:動き:はだかモーション
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_MOTION             =  35; //  35:動き:変身前モーション
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_WEAPON_MOTION      =  36; //  36:動き:変身前武器モーション

    public const T_X2BtlRequestActorId ACTOR_MONSTER1_MOTION                  =  37; //  37:動き:モンスター１モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER2_MOTION                  =  38; //  38:動き:モンスター２モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER3_MOTION                  =  39; //  39:動き:モンスター３モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER4_MOTION                  =  40; //  40:動き:モンスター４モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER5_MOTION                  =  41; //  41:動き:モンスター５モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER6_MOTION                  =  42; //  42:動き:モンスター６モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER7_MOTION                  =  43; //  43:動き:モンスター７モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER8_MOTION                  =  44; //  44:動き:モンスター８モーション

    public const T_X2BtlRequestActorId ACTOR_MONSTER9_MOTION                  =  45; //  45:動き:モンスター９モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER10_MOTION                 =  46; //  46:動き:モンスター１０モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER11_MOTION                 =  47; //  47:動き:モンスター１１モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER12_MOTION                 =  48; //  48:動き:モンスター１２モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER13_MOTION                 =  49; //  49:動き:モンスター１３モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER14_MOTION                 =  50; //  50:動き:モンスター１４モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER15_MOTION                 =  51; //  51:動き:モンスター１５モーション
    public const T_X2BtlRequestActorId ACTOR_MONSTER16_MOTION                 =  52; //  52:動き:モンスター１６モーション

    public const T_X2BtlRequestActorId ACTOR_YUNA                             =  53; //  53:判定:ユウナ
    public const T_X2BtlRequestActorId ACTOR_RIKKU                            =  54; //  54:判定:リュック
    public const T_X2BtlRequestActorId ACTOR_PAIN                             =  55; //  55:判定:パイン
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL1                    =  56; //  56:判定:ユウナスペシャル１
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL2                    =  57; //  57:判定:ユウナスペシャル２
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL1                   =  58; //  58:判定:リュックスペシャル１
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL2                   =  59; //  59:判定:リュックスペシャル２
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL1                    =  60; //  60:判定:パインスペシャル１
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL2                    =  61; //  61:判定:パインスペシャル２
    public const T_X2BtlRequestActorId ACTOR_YUNA_WEAPON                      =  62; //  62:判定:ユウナ武器
    public const T_X2BtlRequestActorId ACTOR_RIKKU_WEAPON                     =  63; //  63:判定:リュック武器
    public const T_X2BtlRequestActorId ACTOR_PAIN_WEAPON                      =  64; //  64:判定:パイン武器
    public const T_X2BtlRequestActorId ACTOR_NUDE                             =  65; //  65:判定:はだか
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE                    =  66; //  66:判定:変身前
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_WEAPON             =  67; //  67:判定:変身前武器

    public const T_X2BtlRequestActorId ACTOR_MONSTER                          =  68; //  68:判定:モンスター
    public const T_X2BtlRequestActorId ACTOR_OVERSOUL                         =  69; //  69:判定:オーバーソウル

    public const T_X2BtlRequestActorId ACTOR_YUNA_VOICE                       =  70; //  70:動き:ユウナボイス
    public const T_X2BtlRequestActorId ACTOR_RIKKU_VOICE                      =  71; //  71:動き:リュックボイス
    public const T_X2BtlRequestActorId ACTOR_PAIN_VOICE                       =  72; //  72:動き:パインボイス
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL1_VOICE              =  73; //  73:動き:ユウナスペシャル１ボイス
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL2_VOICE              =  74; //  74:動き:ユウナスペシャル２ボイス
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL1_VOICE             =  75; //  75:動き:リュックスペシャル１ボイス
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL2_VOICE             =  76; //  76:動き:リュックスペシャル２ボイス
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL1_VOICE              =  77; //  77:動き:パインスペシャル１ボイス
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL2_VOICE              =  78; //  78:動き:パインスペシャル２ボイス
    public const T_X2BtlRequestActorId ACTOR_YUNA_WEAPON_VOICE                =  79; //  79:動き:ユウナ武器ボイス
    public const T_X2BtlRequestActorId ACTOR_RIKKU_WEAPON_VOICE               =  80; //  80:動き:リュック武器ボイス
    public const T_X2BtlRequestActorId ACTOR_PAIN_WEAPON_VOICE                =  81; //  81:動き:パイン武器ボイス
    public const T_X2BtlRequestActorId ACTOR_NUDE_VOICE                       =  82; //  82:動き:はだかボイス
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_VOICE              =  83; //  83:動き:変身前ボイス
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_WEAPON_VOICE       =  84; //  84:動き:変身前武器ボイス

    public const T_X2BtlRequestActorId ACTOR_MONSTER1_VOICE                   =  85; //  85:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER2_VOICE                   =  86; //  86:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER3_VOICE                   =  87; //  87:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER4_VOICE                   =  88; //  88:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER5_VOICE                   =  89; //  89:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER6_VOICE                   =  90; //  90:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER7_VOICE                   =  91; //  91:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER8_VOICE                   =  92; //  92:動き:モンスターボイス

    public const T_X2BtlRequestActorId ACTOR_MONSTER9_VOICE                   =  93; //  93:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER10_VOICE                  =  94; //  94:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER11_VOICE                  =  95; //  95:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER12_VOICE                  =  96; //  96:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER13_VOICE                  =  97; //  97:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER14_VOICE                  =  98; //  98:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER15_VOICE                  =  99; //  99:動き:モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_MONSTER16_VOICE                  = 100; // 100:動き:モンスターボイス

    public const T_X2BtlRequestActorId ACTOR_YUNA_VOICE_CHECK                 = 101; // 101:動き:ユウナボイスチェック
    public const T_X2BtlRequestActorId ACTOR_RIKKU_VOICE_CHECK                = 102; // 102:動き:リュックボイスチェック
    public const T_X2BtlRequestActorId ACTOR_PAIN_VOICE_CHECK                 = 103; // 103:動き:パインボイスチェック
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL1_VOICE_CHECK        = 104; // 104:動き:ユウナスペシャル１ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_YUNA_SPECIAL2_VOICE_CHECK        = 105; // 105:動き:ユウナスペシャル２ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL1_VOICE_CHECK       = 106; // 106:動き:リュックスペシャル１ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_RIKKU_SPECIAL2_VOICE_CHECK       = 107; // 107:動き:リュックスペシャル２ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL1_VOICE_CHECK        = 108; // 108:動き:パインスペシャル１ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_PAIN_SPECIAL2_VOICE_CHECK        = 109; // 109:動き:パインスペシャル２ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_YUNA_WEAPON_VOICE_CHECK          = 110; // 110:動き:ユウナ武器ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_RIKKU_WEAPON_VOICE_CHECK         = 111; // 111:動き:リュック武器ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_PAIN_WEAPON_VOICE_CHECK          = 112; // 112:動き:パイン武器ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_NUDE_VOICE_CHECK                 = 113; // 113:動き:リュック武器ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_VOICE_CHECK        = 114; // 114:動き:変身前ボイスチェック
    public const T_X2BtlRequestActorId ACTOR_BEFORE_CHANGE_WEAPON_VOICE_CHECK = 115; // 115:動き:変身前武器ボイスチェック

    public const T_X2BtlRequestActorId ACTOR_MONSTER1_VOICE_CHECK             = 116; // 116:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER2_VOICE_CHECK             = 117; // 117:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER3_VOICE_CHECK             = 118; // 118:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER4_VOICE_CHECK             = 119; // 119:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER5_VOICE_CHECK             = 120; // 120:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER6_VOICE_CHECK             = 121; // 121:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER7_VOICE_CHECK             = 122; // 122:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER8_VOICE_CHECK             = 123; // 123:動き:モンスターボイスチェック

    public const T_X2BtlRequestActorId ACTOR_MONSTER9_VOICE_CHECK             = 124; // 124:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER10_VOICE_CHECK            = 125; // 125:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER11_VOICE_CHECK            = 126; // 126:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER12_VOICE_CHECK            = 127; // 127:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER13_VOICE_CHECK            = 128; // 128:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER14_VOICE_CHECK            = 129; // 129:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER15_VOICE_CHECK            = 130; // 130:動き:モンスターボイスチェック
    public const T_X2BtlRequestActorId ACTOR_MONSTER16_VOICE_CHECK            = 131; // 131:動き:モンスターボイスチェック

    public const T_X2BtlRequestActorId ACTOR_FRIEND_MONSTER_VOICE             = 132; // 132:動き:仲間モンスターボイス
    public const T_X2BtlRequestActorId ACTOR_FRIEND_MONSTER_VOICE_CHECK       = 133; // 133:動き:仲間モンスターボイスチェック

    public const T_X2BtlRequestActorId ACTOR_NOT_REGIST                       = 0xFF;
}

// カメラタグ
public static class BtlRequestCameraTagId {
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT0 =  0; //  0:イベント０
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT1 =  1; //  1:イベント１
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT2 =  2; //  2:イベント２
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT3 =  3; //  3:イベント３
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT4 =  4; //  4:イベント４
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT5 =  5; //  5:イベント５
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT6 =  6; //  6:イベント６
    public const T_X2BtlRequestTagId TAG_CAMERA_EVENT7 =  7; //  7:イベント７

    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT0 =  8; //  8:ボイスイベント０
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT1 =  9; //  9:ボイスイベント１
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT2 = 10; // 10:ボイスイベント２
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT3 = 11; // 11:ボイスイベント３
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT4 = 12; // 12:ボイスイベント４
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT5 = 13; // 13:ボイスイベント５
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT6 = 14; // 14:ボイスイベント６
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT7 = 15; // 15:ボイスイベント７

    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT8  = 16; // 16:ボイスイベント８
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT9  = 17; // 17:ボイスイベント９
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT10 = 18; // 18:ボイスイベント１０
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT11 = 19; // 19:ボイスイベント１１
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT12 = 20; // 20:ボイスイベント１２
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT13 = 21; // 21:ボイスイベント１３
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT14 = 22; // 22:ボイスイベント１４
    public const T_X2BtlRequestTagId TAG_CAMERA_VOICE_EVENT15 = 23; // 23:ボイスイベント１５

    public const T_X2BtlRequestTagId TAG_CAMERA_START       = 24; // 24:突入
    public const T_X2BtlRequestTagId TAG_CAMERA_ATTACK      = 25; // 25:攻撃
    public const T_X2BtlRequestTagId TAG_CAMERA_MAGIC_START = 26; // 26:魔法開始
    public const T_X2BtlRequestTagId TAG_CAMERA_STANDARD    = 27; // 27:通常

    public const T_X2BtlRequestTagId TAG_CAMERA_ATTACK_YUNA  = 28; // 28:攻撃ユウナ
    public const T_X2BtlRequestTagId TAG_CAMERA_ATTACK_RIKKU = 29; // 29:攻撃リュック
    public const T_X2BtlRequestTagId TAG_CAMERA_ATTACK_PAIN  = 30; // 30:攻撃パイン

    public const T_X2BtlRequestTagId TAG_CAMERA_MONMAGIC_START = 31; // 31:モンスター魔法開始
    public const T_X2BtlRequestTagId TAG_CAMERA_MONMAGIC_THROW = 32; // 32:モンスター魔法発射
    public const T_X2BtlRequestTagId TAG_CAMERA_MONITEM_START  = 33; // 33:モンスターアイテム開始
    public const T_X2BtlRequestTagId TAG_CAMERA_MONITEM_THROW  = 34; // 34:モンスターアイテム発射
    public const T_X2BtlRequestTagId TAG_CAMERA_ATTACK_MONSTER = 35; // 35:攻撃モンスター

    public const T_X2BtlRequestTagId TAG_CAMERA_ITEM_START  = 36; // 36:アイテム
    public const T_X2BtlRequestTagId TAG_CAMERA_ITEM_THROW  = 37; // 37:アイテム発射
    public const T_X2BtlRequestTagId TAG_CAMERA_MAGIC_THROW = 38; // 38:魔法発射
    public const T_X2BtlRequestTagId TAG_CAMERA_ESCAPE      = 39; // 39:逃げる

    public const T_X2BtlRequestTagId TAG_CAMERA_SP1 = 40; // 40:特殊処理１
    public const T_X2BtlRequestTagId TAG_CAMERA_SP2 = 41; // 41:特殊処理２
    public const T_X2BtlRequestTagId TAG_CAMERA_SP3 = 42; // 42:特殊処理３
    public const T_X2BtlRequestTagId TAG_CAMERA_SP4 = 43; // 43:特殊処理４
    public const T_X2BtlRequestTagId TAG_CAMERA_SP5 = 44; // 44:特殊処理５
    public const T_X2BtlRequestTagId TAG_CAMERA_SP6 = 45; // 45:特殊処理６
    public const T_X2BtlRequestTagId TAG_CAMERA_SP7 = 46; // 46:特殊処理７
    public const T_X2BtlRequestTagId TAG_CAMERA_SP8 = 47; // 47:特殊処理８

    public const T_X2BtlRequestTagId TAG_CAMERA_SKILL_START  = 48; // 48:スキル開始
    public const T_X2BtlRequestTagId TAG_CAMERA_SKILL_ACTION = 49; // 49:スキル発動

    public const T_X2BtlRequestTagId TAG_CAMERA_ACTIVE = 50; // 50:アクティブ
    public const T_X2BtlRequestTagId TAG_CAMERA_DEATH  = 51; // 51:死亡

    public const T_X2BtlRequestTagId TAG_CAMERA_STEAL = 52; // 52:ぬすむ

    public const T_X2BtlRequestTagId TAG_CAMERA_WIN           = 53; // 53:勝利
    public const T_X2BtlRequestTagId TAG_CAMERA_LOSE          = 54; // 54:敗北
    public const T_X2BtlRequestTagId TAG_CAMERA_PLAYER_ESCAPE = 55; // 55:プレイヤー逃亡
    public const T_X2BtlRequestTagId TAG_CAMERA_ENEMY_ESCAPE  = 56; // 56:モンスター逃亡

    public const T_X2BtlRequestTagId TAG_CAMERA_PREPARE = 57; // 57:調合

    public const T_X2BtlRequestTagId TAG_CAMERA_START_PLAYER  = 58; // 58:突入プレイヤー開始
    public const T_X2BtlRequestTagId TAG_CAMERA_END_PLAYER    = 59; // 59:突入プレイヤー終了
    public const T_X2BtlRequestTagId TAG_CAMERA_START_MONSTER = 60; // 60:突入モンスター開始
    public const T_X2BtlRequestTagId TAG_CAMERA_END_MONSTER   = 61; // 61:突入モンスター終了

    public const T_X2BtlRequestTagId TAG_CAMERA_BATTLE_START    = 62; // 62:バトル開始
    public const T_X2BtlRequestTagId TAG_CAMERA_MONSKILL_START  = 63; // 63:モンスキル開始
    public const T_X2BtlRequestTagId TAG_CAMERA_MONSKILL_ACTION = 64; // 64:モンスキル終了
    public const T_X2BtlRequestTagId TAG_CAMERA_EFFECT_END      = 65; // 65:エフェクト終了

    public const T_X2BtlRequestTagId TAG_CAMERA_PARTY_IN  = 66; // 66:パーティーイン
    public const T_X2BtlRequestTagId TAG_CAMERA_PARTY_OUT = 67; // 67:パーティーアウト

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
}

// モーションタグ
public static class BtlRequestMotionTagId {
    public const T_X2BtlRequestTagId TAG_MOTION_INIT         =  0; //  0:初期化
    public const T_X2BtlRequestTagId TAG_MOTION_BATTLE_START =  1; //  1:バトル開始
    public const T_X2BtlRequestTagId TAG_MOTION_WAIT         =  2; //  2:待機
    public const T_X2BtlRequestTagId TAG_MOTION_WAKEUP       =  3; //  3:起きる

    public const T_X2BtlRequestTagId TAG_MOTION_ATTACK         =  4; //  4:攻撃
    public const T_X2BtlRequestTagId TAG_MOTION_ATTACK2        =  5; //  5:攻撃２
    public const T_X2BtlRequestTagId TAG_MOTION_STAY_ATTACK    =  6; //  6:その場攻撃
    public const T_X2BtlRequestTagId TAG_MOTION_ATTACK_TRAINER =  7; //  7:攻撃調教師
    public const T_X2BtlRequestTagId TAG_MOTION_ATTACK_END     =  8; //  8:攻撃終了
    public const T_X2BtlRequestTagId TAG_MOTION_RETURN         =  9; //  9:戻る

    public const T_X2BtlRequestTagId TAG_MOTION_STEAL = 10; // 10:ぬすむ
    public const T_X2BtlRequestTagId TAG_MOTION_MOVE  = 11; // 11:移動

    public const T_X2BtlRequestTagId TAG_MOTION_MAGIC            = 12; // 12:魔法
    public const T_X2BtlRequestTagId TAG_MOTION_MAGIC_THROW      = 13; // 13:魔法発射
    public const T_X2BtlRequestTagId TAG_MOTION_WAIT_MAGIC       = 14; // 14:待機魔法
    public const T_X2BtlRequestTagId TAG_MOTION_WAIT_MAGIC_THROW = 15; // 15:待機魔法：発射
    public const T_X2BtlRequestTagId TAG_MOTION_ITEM             = 16; // 16:アイテム
    public const T_X2BtlRequestTagId TAG_MOTION_ITEM_THROW       = 17; // 17:アイテム発射
    public const T_X2BtlRequestTagId TAG_MOTION_SKILL            = 18; // 18:スキル
    public const T_X2BtlRequestTagId TAG_MOTION_SKILL_ACTION     = 19; // 19:スキル発動
    public const T_X2BtlRequestTagId TAG_MOTION_DEATH_MAGIC      = 20; // 20:死亡魔法
    public const T_X2BtlRequestTagId TAG_MOTION_DEATH            = 21; // 21:死亡

    public const T_X2BtlRequestTagId TAG_MOTION_RECOVER    = 22; // 22:回復
    public const T_X2BtlRequestTagId TAG_MOTION_MAGIC_INV  = 23; // 23:魔法無効
    public const T_X2BtlRequestTagId TAG_MOTION_PHYSIC_INV = 24; // 24:物理無効

    public const T_X2BtlRequestTagId TAG_MOTION_DEFENSE_DAMAGE  = 25; // 25:防御ダメージ
    public const T_X2BtlRequestTagId TAG_MOTION_DAMAGE_NORMAL   = 26; // 26:通常ダメージ
    public const T_X2BtlRequestTagId TAG_MOTION_DAMAGE_CRITICAL = 27; // 27:クリティカルダメージ

    public const T_X2BtlRequestTagId TAG_MOTION_APPEAR  = 28; // 28:出現
    public const T_X2BtlRequestTagId TAG_MOTION_DEFENSE = 29; // 29:防御
    public const T_X2BtlRequestTagId TAG_MOTION_AVOID   = 30; // 30:避け

    public const T_X2BtlRequestTagId TAG_MOTION_IN     = 31; // 31:入る
    public const T_X2BtlRequestTagId TAG_MOTION_OUT    = 32; // 32:出る
    public const T_X2BtlRequestTagId TAG_MOTION_ESCAPE = 33; // 33:逃げる

    public const T_X2BtlRequestTagId TAG_MOTION_STONE      = 34; // 34:石化
    public const T_X2BtlRequestTagId TAG_MOTION_BLOW       = 35; // 35:吹き飛ばし
    public const T_X2BtlRequestTagId TAG_MOTION_STONE_BLOW = 36; // 36:石化吹き飛ばし

    public const T_X2BtlRequestTagId TAG_MOTION_SP1 = 37; // 37:特殊処理１
    public const T_X2BtlRequestTagId TAG_MOTION_SP2 = 38; // 38:特殊処理２
    public const T_X2BtlRequestTagId TAG_MOTION_SP3 = 39; // 39:特殊処理３
    public const T_X2BtlRequestTagId TAG_MOTION_SP4 = 40; // 40:特殊処理４
    public const T_X2BtlRequestTagId TAG_MOTION_SP5 = 41; // 41:特殊処理５
    public const T_X2BtlRequestTagId TAG_MOTION_SP6 = 42; // 42:特殊処理６
    public const T_X2BtlRequestTagId TAG_MOTION_SP7 = 43; // 43:特殊処理７
    public const T_X2BtlRequestTagId TAG_MOTION_SP8 = 44; // 44:特殊処理８

    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL0 = 45; // 45:ＳＰ０
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL1 = 46; // 46:ＳＰ１
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL2 = 47; // 47:ＳＰ２
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL3 = 48; // 48:ＳＰ３
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL4 = 49; // 49:ＳＰ４
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL5 = 50; // 50:ＳＰ５
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL6 = 51; // 51:ＳＰ６
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL7 = 52; // 52:ＳＰ７

    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL0_THROW = 53; // 53:ＳＰ０発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL1_THROW = 54; // 54:ＳＰ１発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL2_THROW = 55; // 55:ＳＰ２発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL3_THROW = 56; // 56:ＳＰ３発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL4_THROW = 57; // 57:ＳＰ４発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL5_THROW = 58; // 58:ＳＰ５発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL6_THROW = 59; // 59:ＳＰ６発射
    public const T_X2BtlRequestTagId TAG_MOTION_SPECIAL7_THROW = 60; // 60:ＳＰ７発射

    public const T_X2BtlRequestTagId TAG_MOTION_WIN     = 61; // 61:勝利
    public const T_X2BtlRequestTagId TAG_MOTION_NOTHING = 62; // 62:なし
    public const T_X2BtlRequestTagId TAG_MOTION_SLEEP   = 63; // 63:寝る

    public const T_X2BtlRequestTagId TAG_MOTION_GUN_RETURN          = 64; // 64:銃戻す１
    public const T_X2BtlRequestTagId TAG_MOTION_GUN_RETURN_WAIT     = 65; // 65:銃戻す２
    public const T_X2BtlRequestTagId TAG_MOTION_RUN                 = 66; // 66:走る
    public const T_X2BtlRequestTagId TAG_MOTION_DANCING_OFF         = 67; // 67:踊り解除
    public const T_X2BtlRequestTagId TAG_MOTION_APPEAR_RUN          = 68; // 68:出現走り
    public const T_X2BtlRequestTagId TAG_MOTION_RESIDENT_MAGIC      = 69; // 69:常駐魔法
    public const T_X2BtlRequestTagId TAG_MOTION_EXCHANGE_WAIT       = 70; // 70:着替え待機
    public const T_X2BtlRequestTagId TAG_MOTION_ATTACK_TRAINER2     = 71; // 71:攻撃調教師２
    public const T_X2BtlRequestTagId TAG_MOTION_EXCHANGE_WAIT_PET   = 72; // 72:着替え待機ペット
    public const T_X2BtlRequestTagId TAG_MOTION_APPEAR2             = 73; // 73:出現２
    public const T_X2BtlRequestTagId TAG_MOTION_MONSTER_INIT        = 74; // 74:仲間モンスター初期化
    public const T_X2BtlRequestTagId TAG_MOTION_RESIDENT_MAGIC_WAIT = 75; // 75:常駐待機魔法
    public const T_X2BtlRequestTagId TAG_MOTION_GETA                = 76; // 76:下駄

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
};

// 行動言語タグ
public static class BtlRequestActionTagId {
    public const T_X2BtlRequestTagId TAG_ACTION_TURN          = 0; //  0:アクション
    public const T_X2BtlRequestTagId TAG_ACTION_MENU          = 1; //  1:メニュー
    public const T_X2BtlRequestTagId TAG_ACTION_TARGET        = 2; //  2:被ターゲット
    public const T_X2BtlRequestTagId TAG_ACTION_REACTION      = 3; //  3:リアクション
    public const T_X2BtlRequestTagId TAG_ACTION_DEATH         = 4; //  4:死亡
    public const T_X2BtlRequestTagId TAG_ACTION_MODELINIT     = 5; //  5:モデル設定
    public const T_X2BtlRequestTagId TAG_ACTION_TURN_COMPLETE = 6; //  6:アクション終了
    public const T_X2BtlRequestTagId TAG_ACTION_TURN_START    = 7; //  7:アクション開始

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
}

// ボイスタグ
public static class BtlRequestVoiceTagId {
    public const T_X2BtlRequestTagId TAG_VOICE_RES1         =  0; //  0:予約１
    public const T_X2BtlRequestTagId TAG_VOICE_RES2         =  1; //  1:予約２
    public const T_X2BtlRequestTagId TAG_VOICE_RES3         =  2; //  2:予約３
    public const T_X2BtlRequestTagId TAG_VOICE_RES4         =  3; //  3:予約４
    public const T_X2BtlRequestTagId TAG_VOICE_RES5         =  4; //  4:予約５
    public const T_X2BtlRequestTagId TAG_VOICE_RES6         =  5; //  5:予約６
    public const T_X2BtlRequestTagId TAG_VOICE_RES7         =  6; //  6:予約７
    public const T_X2BtlRequestTagId TAG_VOICE_RES8         =  7; //  7:予約８
    public const T_X2BtlRequestTagId TAG_VOICE_ATTACK       =  8; //  8:攻撃
    public const T_X2BtlRequestTagId TAG_VOICE_AFTER_ATTACK =  9; //  9:攻撃後
    public const T_X2BtlRequestTagId TAG_VOICE_DAMAGE       = 10; // 10:ダメージ
    public const T_X2BtlRequestTagId TAG_VOICE_ATTACK_GETA  = 11; // 11:下駄

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
}

// ボイスチェックタグ
public static class BtlRequestVoiceCheckTagId {
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_TURN                  =  0; //  0:アクション
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_MENU                  =  1; //  1:メニュー
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_TARGET                =  2; //  2:被ターゲット
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_REACTION              =  3; //  3:リアクション
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_DEATH                 =  4; //  4:死亡
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_MODELINIT             =  5; //  5:モデル設定
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_TURN_COMPLETE         =  6; //  6:アクション終了
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_TURN_START            =  7; //  7:アクション開始
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_MAGIC_START           =  8; //  8:魔法開始
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_DEATH_RECOVER         =  9; //  9:死亡回復
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_CLOTH_CHANGE_COMPLETE = 10; // 10:着替え終了
    public const T_X2BtlRequestTagId TAG_VOICE_CHECK_WIN_START             = 11; // 11:勝利開始

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
}

// システムタグ
public static class BtlRequestEventTagId {
    public const T_X2BtlRequestTagId TAG_SYS_EVENT0        =  0; //  0:イベント０
    public const T_X2BtlRequestTagId TAG_SYS_EVENT1        =  1; //  1:イベント１
    public const T_X2BtlRequestTagId TAG_SYS_EVENT2        =  2; //  2:イベント２
    public const T_X2BtlRequestTagId TAG_SYS_EVENT3        =  3; //  3:イベント３
    public const T_X2BtlRequestTagId TAG_SYS_EVENT4        =  4; //  4:イベント４
    public const T_X2BtlRequestTagId TAG_SYS_EVENT5        =  5; //  5:イベント５
    public const T_X2BtlRequestTagId TAG_SYS_EVENT6        =  6; //  6:イベント６
    public const T_X2BtlRequestTagId TAG_SYS_EVENT7        =  7; //  7:イベント７

    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT0  =  8; //  8:ボイスイベント０
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT1  =  9; //  9:ボイスイベント１
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT2  = 10; // 10:ボイスイベント２
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT3  = 11; // 11:ボイスイベント３
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT4  = 12; // 12:ボイスイベント４
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT5  = 13; // 13:ボイスイベント５
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT6  = 14; // 14:ボイスイベント６
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT7  = 15; // 15:ボイスイベント７
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT8  = 16; // 16:ボイスイベント８
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT9  = 17; // 17:ボイスイベント９
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT10 = 18; // 18:ボイスイベント１０
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT11 = 19; // 19:ボイスイベント１１
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT12 = 20; // 20:ボイスイベント１２
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT13 = 21; // 21:ボイスイベント１３
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT14 = 22; // 22:ボイスイベント１４
    public const T_X2BtlRequestTagId TAG_SYS_VOICE_EVENT15 = 23; // 23:ボイスイベント１５

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
}

// システムチェックタグ
public static class BtlRequestSysCheckTagId {
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_WIN                    = 0; //  0:プレイヤー勝利
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_LOSE                   = 1; //  1:プレイヤー敗北
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_PLAYER_ESCAPE          = 2; //  2:プレイヤー逃亡
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_ENEMY_ESCAPE           = 3; //  3:モンスター逃亡
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_BATTLE_FINISH          = 4; //  4:バトル終了
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_BATTLE_START           = 5; //  5:バトル開始
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_BATTLE_START_BEFORE    = 6; //  6:バトル開始直前
    public const T_X2BtlRequestTagId TAG_SYS_CHECK_BATTLE_FINISH_COMPLETE = 7; //  7:バトル完全終了

    public const T_X2BtlRequestTagId TAG_NOT_REGIST = -1;
}

// I have no idea what this actually is nor its primitive type.
public static class BtlRequestBinHdrId {
    public const byte BTL_BIN_MONSTER1 = 0; // モンスター１データ
    public const byte BTL_BIN_MONSTER2 = 1; // モンスター２データ
    public const byte BTL_BIN_MONSTER3 = 2; // モンスター３データ
    public const byte BTL_BIN_MONSTER4 = 3; // モンスター４データ
    public const byte BTL_BIN_MONSTER5 = 4; // モンスター５データ
    public const byte BTL_BIN_MONSTER6 = 5; // モンスター６データ
    public const byte BTL_BIN_MONSTER7 = 6; // モンスター７データ
    public const byte BTL_BIN_MONSTER8 = 7; // モンスター８データ
}
