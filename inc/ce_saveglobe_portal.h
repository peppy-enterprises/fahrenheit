#ifndef CE_SAVEGLOBE_PORTAL_H
#define CE_SAVEGLOBE_PORTAL_H

#ifdef __cplusplus
extern "C" {
#endif

#include "ce_wallet.h"
#include "ce_character.h"

// Save Globe (Checkpoint) Portal Context
typedef struct {
    int save_slot_id;
    ce_wallet_t* linked_wallet;
    ce_character_t* current_character;
    bool is_connected;
    float last_interaction_time;
} ce_saveglobe_portal_t;

// Lifecycle
ce_saveglobe_portal_t* ce_saveglobe_create(int slot_id);
void ce_saveglobe_destroy(ce_saveglobe_portal_t* globe);

// Interaction
bool ce_saveglobe_connect(ce_saveglobe_portal_t* globe, ce_character_t* character);
void ce_saveglobe_disconnect(ce_saveglobe_portal_t* globe);

// Wallet Access at Save Points
void ce_saveglobe_open_wallet(ce_saveglobe_portal_t* globe);
void ce_saveglobe_quick_deposit(ce_saveglobe_portal_t* globe, double amount_usd);
void ce_saveglobe_quick_withdraw(ce_saveglobe_portal_t* globe, double amount_ffxt);

// Character Management at Save Points
void ce_saveglobe_view_stats(ce_saveglobe_portal_t* globe);
void ce_saveglobe_change_job(ce_saveglobe_portal_t* globe, const char* job_name);
void ce_saveglobe_equip_asset(ce_saveglobe_portal_t* globe, int asset_id, int slot);

// Save/Load Game State with Crypto Sync
bool ce_saveglobe_save_game(ce_saveglobe_portal_t* globe);
bool ce_saveglobe_load_game(ce_saveglobe_portal_t* globe, int slot_id);

// Marketplace Access (Limited while in-game)
void ce_saveglobe_browse_marketplace(ce_saveglobe_portal_t* globe);
bool ce_saveglobe_purchase_item(ce_saveglobe_portal_t* globe, int listing_id);

#ifdef __cplusplus
}
#endif

#endif // CE_SAVEGLOBE_PORTAL_H
