#ifndef CE_TITLE_PORTAL_H
#define CE_TITLE_PORTAL_H

#include "ce_wallet.h"
#include "ce_character.h"

// Forward declare C++ marketplace class
namespace CrystalEchoes { namespace Marketplace { class MarketplaceManager; }}
typedef CrystalEchoes::Marketplace::MarketplaceManager ce_marketplace_t;

#ifdef __cplusplus
extern "C" {
#endif

// Portal State Enums
typedef enum {
    PORTAL_STATE_IDLE,
    PORTAL_STATE_LOGIN,
    PORTAL_STATE_WALLET,
    PORTAL_STATE_MARKETPLACE,
    PORTAL_STATE_CHARACTER_SELECT,
    PORTAL_STATE_ENTERING_GAME
} ce_portal_state_t;

// Main Portal Context
typedef struct {
    ce_portal_state_t state;
    ce_wallet_t* active_wallet;
    ce_marketplace_t* marketplace;
    ce_character_manager_t* char_manager;
    int selected_profile_index;
    bool is_authenticated;
    char session_token[64];
} ce_title_portal_t;

// Lifecycle
ce_title_portal_t* ce_portal_create();
void ce_portal_destroy(ce_title_portal_t* portal);

// Navigation
void ce_portal_render_main_menu(ce_title_portal_t* portal);
void ce_portal_handle_input(ce_title_portal_t* portal, int input_key);

// Sub-Systems Access
void ce_portal_open_wallet(ce_title_portal_t* portal);
void ce_portal_open_marketplace(ce_title_portal_t* portal);
void ce_portal_open_character_select(ce_title_portal_t* portal);

// Authentication
bool ce_portal_login(ce_title_portal_t* portal, const char* username, const char* pin);
void ce_portal_logout(ce_title_portal_t* portal);

// Game Launch
bool ce_portal_launch_game(ce_title_portal_t* portal, int character_id);

#ifdef __cplusplus
}
#endif

#endif // CE_TITLE_PORTAL_H
