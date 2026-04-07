#include "ce_character.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// Character Manager Implementation
ce_character_manager_t* ce_char_manager_create() {
    ce_character_manager_t* manager = (ce_character_manager_t*)calloc(1, sizeof(ce_character_manager_t));
    if (!manager) return NULL;
    
    manager->capacity = 10; // Max 10 characters per account
    manager->characters = (ce_character_t*)calloc(manager->capacity, sizeof(ce_character_t));
    manager->count = 0;
    
    printf("[CharManager] Created character manager (capacity: %d)\n", manager->capacity);
    return manager;
}

void ce_char_manager_destroy(ce_character_manager_t* manager) {
    if (manager) {
        if (manager->characters) {
            free(manager->characters);
        }
        printf("[CharManager] Destroyed character manager\n");
        free(manager);
    }
}

ce_character_t* ce_char_manager_create_character(ce_character_manager_t* manager, const ce_char_create_t* info) {
    if (!manager || !info) return NULL;
    
    if (manager->count >= manager->capacity) {
        printf("[CharManager] Maximum character limit reached.\n");
        return NULL;
    }
    
    // Find first empty slot
    int slot = -1;
    for (int i = 0; i < manager->capacity; i++) {
        if (manager->characters[i].id == 0) {
            slot = i;
            break;
        }
    }
    
    if (slot < 0) slot = manager->count;
    
    ce_character_t* chara = &manager->characters[slot];
    
    // Initialize character
    chara->id = slot + 1;
    strncpy(chara->name, info->name, sizeof(chara->name) - 1);
    strncpy(chara->current_job, info->job, sizeof(chara->current_job) - 1);
    chara->stats = info->stats;
    chara->level = 1;
    chara->experience = 0;
    chara->is_online = false;
    chara->inventory_size = 0;
    
    // Initialize equipped assets to -1 (empty)
    for (int i = 0; i < 16; i++) {
        chara->equipped_assets[i] = -1;
    }
    
    manager->count++;
    
    printf("[CharManager] Created character '%s' (%s) at slot %d\n", 
           chara->name, chara->current_job, slot);
    
    return chara;
}

ce_character_t* ce_char_manager_get_character(ce_character_manager_t* manager, int index) {
    if (!manager || index < 0 || index >= manager->capacity) return NULL;
    
    if (manager->characters[index].id == 0) return NULL; // Empty slot
    
    return &manager->characters[index];
}

bool ce_char_manager_delete_character(ce_character_manager_t* manager, int index) {
    if (!manager || index < 0 || index >= manager->capacity) return false;
    
    if (manager->characters[index].id == 0) return false; // Already empty
    
    printf("[CharManager] Deleting character '%s'\n", manager->characters[index].name);
    
    // Clear the slot
    memset(&manager->characters[index], 0, sizeof(ce_character_t));
    manager->count--;
    
    return true;
}

int ce_char_manager_get_count(ce_character_manager_t* manager) {
    if (!manager) return 0;
    return manager->count;
}

void ce_char_manager_list_characters(ce_character_manager_t* manager) {
    if (!manager) return;
    
    printf("\n--- YOUR CHARACTERS ---\n");
    
    int displayed = 0;
    for (int i = 0; i < manager->capacity; i++) {
        ce_character_t* c = &manager->characters[i];
        if (c->id > 0) {
            printf("[%d] %s - Level %d %s\n", c->id, c->name, c->level, c->current_job);
            printf("    STR:%d DEX:%d VIT:%d MAG:%d SPR:%d LUK:%d\n",
                   c->stats.str, c->stats.dex, c->stats.vit,
                   c->stats.mag, c->stats.spr, c->stats.luk);
            displayed++;
        }
    }
    
    if (displayed == 0) {
        printf("(No characters created yet)\n");
    }
}

// Job System
bool ce_char_change_job(ce_character_t* character, const char* job_name) {
    if (!character || !job_name) return false;
    
    strncpy(character->current_job, job_name, sizeof(character->current_job) - 1);
    printf("[Character] %s changed job to %s\n", character->name, job_name);
    return true;
}

bool ce_char_update_stats(ce_character_t* character, ce_character_stats_t* new_stats) {
    if (!character || !new_stats) return false;
    
    character->stats = *new_stats;
    printf("[Character] %s stats updated.\n", character->name);
    return true;
}

// Asset Equipment
bool ce_char_equip_asset(ce_character_t* character, int asset_id, int slot) {
    if (!character || slot < 0 || slot >= 16) return false;
    
    character->equipped_assets[slot] = asset_id;
    printf("[Character] %s equipped asset %d in slot %d\n", character->name, asset_id, slot);
    return true;
}

bool ce_char_unequip_asset(ce_character_t* character, int slot) {
    if (!character || slot < 0 || slot >= 16) return false;
    
    character->equipped_assets[slot] = -1;
    printf("[Character] %s unequipped slot %d\n", character->name, slot);
    return true;
}
