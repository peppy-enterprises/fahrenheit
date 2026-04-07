#ifndef CE_CHARACTER_H
#define CE_CHARACTER_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>
#include <stdint.h>

// Character Stats (Universal)
typedef struct {
    int str;  // Strength
    int dex;  // Dexterity
    int vit;  // Vitality
    int mag;  // Magic
    int spr;  // Spirit
    int luk;  // Luck
} ce_character_stats_t;

// Job Types
typedef enum {
    JOB_WARRIOR,
    JOB_MAGE,
    JOB_THIEF,
    JOB_DRAGOON,
    JOB_BLACK_MAGE,
    JOB_WHITE_MAGE,
    JOB_CUSTOM
} ce_job_type_t;

// Asset Types for Marketplace
typedef enum {
    ASSET_TYPE_WEAPON,
    ASSET_TYPE_ARMOR,
    ASSET_TYPE_ACCESSORY,
    ASSET_TYPE_MOUNT,
    ASSET_TYPE_PET,
    ASSET_TYPE_SKIN,
    ASSET_TYPE_EMOTE
} ce_asset_type_t;

// Quality Levels
typedef enum {
    QUALITY_COMMON,
    QUALITY_UNCOMMON,
    QUALITY_RARE,
    QUALITY_EPIC,
    QUALITY_LEGENDARY,
    QUALITY_DIVINE
} ce_quality_level_t;

// Character Structure
typedef struct {
    int id;
    char name[64];
    char current_job[32];
    ce_character_stats_t stats;
    int level;
    uint64_t experience;
    int equipped_assets[16]; // Asset IDs for 16 slots
    int inventory_size;
    bool is_online;
} ce_character_t;

// Character Creation Info
typedef struct {
    char name[64];
    char job[32];
    ce_character_stats_t stats;
} ce_char_create_t;

// Character Manager (Handles multiple characters)
typedef struct {
    ce_character_t* characters;
    int count;
    int capacity;
} ce_character_manager_t;

// Lifecycle
ce_character_manager_t* ce_char_manager_create();
void ce_char_manager_destroy(ce_character_manager_t* manager);

// Character Operations
ce_character_t* ce_char_manager_create_character(ce_character_manager_t* manager, const ce_char_create_t* info);
ce_character_t* ce_char_manager_get_character(ce_character_manager_t* manager, int index);
bool ce_char_manager_delete_character(ce_character_manager_t* manager, int index);
int ce_char_manager_get_count(ce_character_manager_t* manager);

// Display
void ce_char_manager_list_characters(ce_character_manager_t* manager);

// Job System
bool ce_char_change_job(ce_character_t* character, const char* job_name);
bool ce_char_update_stats(ce_character_t* character, ce_character_stats_t* new_stats);

// Asset Equipment
bool ce_char_equip_asset(ce_character_t* character, int asset_id, int slot);
bool ce_char_unequip_asset(ce_character_t* character, int slot);

#ifdef __cplusplus
}
#endif

#endif // CE_CHARACTER_H
