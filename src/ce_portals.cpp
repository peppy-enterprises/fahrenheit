#include "ce_title_portal.h"
#include "ce_saveglobe_portal.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

// Internal Helpers
static void ce_portal_render_login(ce_title_portal_t* portal);
static void ce_portal_render_wallet_ui(ce_title_portal_t* portal);
static void ce_portal_render_marketplace_ui(ce_title_portal_t* portal);
static void ce_portal_render_char_select_ui(ce_title_portal_t* portal);

// Lifecycle
ce_title_portal_t* ce_portal_create() {
    ce_title_portal_t* portal = (ce_title_portal_t*)calloc(1, sizeof(ce_title_portal_t));
    if (!portal) return NULL;

    portal->state = PORTAL_STATE_IDLE;
    portal->active_wallet = NULL;
    portal->marketplace = nullptr; // C++ marketplace - initialize later if needed
    portal->char_manager = ce_char_manager_create();
    portal->selected_profile_index = -1;
    portal->is_authenticated = false;
    
    printf("[Portal] Title Portal initialized.\n");
    return portal;
}

void ce_portal_destroy(ce_title_portal_t* portal) {
    if (!portal) return;
    
    if (portal->active_wallet) {
        ce_wallet_destroy(portal->active_wallet);
    }
    // Marketplace is C++ object, skip for now
    if (portal->char_manager) {
        ce_char_manager_destroy(portal->char_manager);
    }
    
    free(portal);
    printf("[Portal] Title Portal destroyed.\n");
}

// Navigation & Rendering
void ce_portal_render_main_menu(ce_title_portal_t* portal) {
    if (!portal) return;

    printf("\n========================================\n");
    printf("       CRYSTAL ECHOES ONLINE\n");
    printf("========================================\n");
    printf("FFX-T Token Supply: 100,000,000,000\n");
    printf("----------------------------------------\n");
    
    if (portal->is_authenticated) {
        printf("[1] Enter Game (Character Select)\n");
        printf("[2] Open Wallet (Balance: %.2f FFX-T)\n", 
               portal->active_wallet ? ce_wallet_get_balance(portal->active_wallet, "FFX-T") : 0.0);
        printf("[3] Browse Marketplace\n");
        printf("[4] Logout\n");
        printf("[5] Exit\n");
    } else {
        printf("[1] Login / Create Account\n");
        printf("[2] Browse Marketplace (Guest)\n");
        printf("[3] Exit\n");
    }
    printf("========================================\n");
    printf("Select Option: ");
}

void ce_portal_handle_input(ce_title_portal_t* portal, int input_key) {
    if (!portal) return;

    switch (portal->state) {
        case PORTAL_STATE_IDLE:
            if (input_key == 1) {
                ce_portal_open_wallet(portal); // Simplified: goes to login/wallet
            } else if (input_key == 2 && !portal->is_authenticated) {
                ce_portal_open_marketplace(portal);
            } else if (input_key == 3 && !portal->is_authenticated) {
                exit(0);
            } else if (input_key == 5 && portal->is_authenticated) {
                exit(0);
            }
            break;

        case PORTAL_STATE_LOGIN:
            // Handled in ce_portal_login
            break;

        case PORTAL_STATE_WALLET:
            if (input_key == 0) { // Back
                portal->state = PORTAL_STATE_IDLE;
            }
            break;

        case PORTAL_STATE_MARKETPLACE:
            if (input_key == 0) { // Back
                portal->state = PORTAL_STATE_IDLE;
            }
            break;

        case PORTAL_STATE_CHARACTER_SELECT:
            if (input_key == 0) { // Back
                portal->state = PORTAL_STATE_IDLE;
            } else if (input_key > 0) {
                ce_portal_launch_game(portal, input_key - 1);
            }
            break;

        default:
            break;
    }
}

// Sub-Systems Access
void ce_portal_open_wallet(ce_title_portal_t* portal) {
    if (!portal) return;

    if (!portal->is_authenticated) {
        printf("\n--- LOGIN REQUIRED ---\n");
        char user[64], pin[16];
        printf("Username: ");
        scanf("%63s", user);
        printf("PIN: ");
        scanf("%15s", pin);
        
        if (ce_portal_login(portal, user, pin)) {
            portal->state = PORTAL_STATE_WALLET;
        } else {
            printf("Login failed.\n");
        }
    } else {
        portal->state = PORTAL_STATE_WALLET;
    }

    if (portal->state == PORTAL_STATE_WALLET && portal->active_wallet) {
        ce_wallet_display_status(portal->active_wallet);
        
        printf("\n[1] Deposit Funds\n");
        printf("[2] Withdraw Funds\n");
        printf("[3] Swap Currency\n");
        printf("[0] Back\n");
    }
}

void ce_portal_open_marketplace(ce_title_portal_t* portal) {
    if (!portal) return;

    portal->state = PORTAL_STATE_MARKETPLACE;
    printf("\n--- GLOBAL MARKETPLACE ---\n");
    printf("(Marketplace integration pending C++ wrapper)\n");
    printf("Demo items available:\n");
    printf("  [1] Warrior Helm - 500 FFX-T (Divine Quality)\n");
    printf("  [2] Chocobo Mount - 1200 FFX-T (Epic Quality)\n");
    
    printf("\n[1] Buy Item\n");
    printf("[2] Sell Item\n");
    printf("[0] Back\n");
}

void ce_portal_open_character_select(ce_title_portal_t* portal) {
    if (!portal || !portal->char_manager) return;

    portal->state = PORTAL_STATE_CHARACTER_SELECT;
    printf("\n--- SELECT CHARACTER ---\n");
    
    // Demo: Create a dummy character if none exist
    if (ce_char_manager_get_count(portal->char_manager) == 0) {
        ce_char_create_t create_info;
        strcpy(create_info.name, "Warrior One");
        strcpy(create_info.job, "Warrior");
        create_info.stats.str = 20;
        create_info.stats.dex = 15;
        ce_char_manager_create_character(portal->char_manager, &create_info);
    }

    ce_char_manager_list_characters(portal->char_manager);
    printf("\nEnter Character ID to play (0 to cancel): ");
}

// Authentication
bool ce_portal_login(ce_title_portal_t* portal, const char* username, const char* pin) {
    if (!portal) return false;

    // Simulate authentication
    printf("Authenticating %s...\n", username);
    
    // Create or load wallet
    if (!portal->active_wallet) {
        portal->active_wallet = ce_wallet_create(username, pin);
    }
    
    if (portal->active_wallet) {
        portal->is_authenticated = true;
        strncpy(portal->session_token, "SESSION_XXXX_YYYY", 63);
        printf("Login successful! Session started.\n");
        return true;
    }
    
    return false;
}

void ce_portal_logout(ce_title_portal_t* portal) {
    if (!portal) return;
    
    if (portal->active_wallet) {
        ce_wallet_destroy(portal->active_wallet);
        portal->active_wallet = NULL;
    }
    
    portal->is_authenticated = false;
    portal->state = PORTAL_STATE_IDLE;
    memset(portal->session_token, 0, sizeof(portal->session_token));
    printf("Logged out successfully.\n");
}

// Game Launch
bool ce_portal_launch_game(ce_title_portal_t* portal, int character_id) {
    if (!portal || !portal->char_manager) return false;

    ce_character_t* chara = ce_char_manager_get_character(portal->char_manager, character_id);
    if (!chara) {
        printf("Invalid character selected.\n");
        return false;
    }

    printf("\nLoading Crystal Echoes World...\n");
    printf("Character: %s (%s)\n", chara->name, chara->current_job);
    printf("Syncing Wallet... Done.\n");
    printf("Loading Assets... Done.\n");
    printf("Entering Server Shard #1...\n");
    
    portal->state = PORTAL_STATE_ENTERING_GAME;
    // In real implementation, this would hand off control to the game engine
    return true;
}

// --- Save Globe Implementation ---

ce_saveglobe_portal_t* ce_saveglobe_create(int slot_id) {
    ce_saveglobe_portal_t* globe = (ce_saveglobe_portal_t*)calloc(1, sizeof(ce_saveglobe_portal_t));
    if (!globe) return NULL;

    globe->save_slot_id = slot_id;
    globe->linked_wallet = NULL;
    globe->current_character = NULL;
    globe->is_connected = false;
    
    printf("[SaveGlobe] Slot %d initialized.\n", slot_id);
    return globe;
}

void ce_saveglobe_destroy(ce_saveglobe_portal_t* globe) {
    if (!globe) return;
    ce_saveglobe_disconnect(globe);
    free(globe);
    printf("[SaveGlobe] Destroyed.\n");
}

bool ce_saveglobe_connect(ce_saveglobe_portal_t* globe, ce_character_t* character) {
    if (!globe || !character) return false;

    globe->current_character = character;
    
    // Link wallet from character data or create temp access
    // In full impl, this loads the user's persistent wallet
    if (!globe->linked_wallet) {
        globe->linked_wallet = ce_wallet_create(character->name, "0000"); // Demo PIN
    }
    
    globe->is_connected = true;
    globe->last_interaction_time = (float)time(NULL);
    
    printf("[SaveGlobe] Connected to %s.\n", character->name);
    return true;
}

void ce_saveglobe_disconnect(ce_saveglobe_portal_t* globe) {
    if (!globe) return;
    
    if (globe->linked_wallet) {
        ce_wallet_destroy(globe->linked_wallet);
        globe->linked_wallet = NULL;
    }
    
    globe->current_character = NULL;
    globe->is_connected = false;
    printf("[SaveGlobe] Disconnected.\n");
}

void ce_saveglobe_open_wallet(ce_saveglobe_portal_t* globe) {
    if (!globe || !globe->is_connected) {
        printf("Save Globe not connected.\n");
        return;
    }
    
    printf("\n--- SAVE GLOBE WALLET ACCESS ---\n");
    ce_wallet_display_status(globe->linked_wallet);
    
    printf("\nQuick Actions:\n");
    printf("[1] Deposit $10 (Credit Card)\n");
    printf("[2] Withdraw 100 FFX-T\n");
    printf("[3] Swap ETH -> FFX-T\n");
    printf("[0] Close\n");
}

void ce_saveglobe_quick_deposit(ce_saveglobe_portal_t* globe, double amount_usd) {
    if (!globe || !globe->is_connected) return;
    
    printf("Processing $%.2f deposit via Stripe...\n", amount_usd);
    // Simulate purchase
    double eth_amount = amount_usd / 2000.0; // Fake rate
    double ffxt_amount = eth_amount * 5000.0; // Fake rate
    
    ce_wallet_deposit(globe->linked_wallet, "FFX-T", ffxt_amount);
    printf("Deposit complete! Received %.2f FFX-T.\n", ffxt_amount);
}

void ce_saveglobe_quick_withdraw(ce_saveglobe_portal_t* globe, double amount_ffxt) {
    if (!globe || !globe->is_connected) return;
    
    if (ce_wallet_withdraw(globe->linked_wallet, "FFX-T", amount_ffxt)) {
        printf("Withdrawal of %.2f FFX-T successful.\n", amount_ffxt);
    } else {
        printf("Withdrawal failed (Insufficient funds).\n");
    }
}

void ce_saveglobe_view_stats(ce_saveglobe_portal_t* globe) {
    if (!globe || !globe->current_character) return;
    
    ce_character_t* c = globe->current_character;
    printf("\n--- CHARACTER STATS ---\n");
    printf("Name: %s\n", c->name);
    printf("Job: %s\n", c->current_job);
    printf("STR: %d | DEX: %d | VIT: %d\n", c->stats.str, c->stats.dex, c->stats.vit);
    printf("Level: %d\n", c->level);
}

void ce_saveglobe_change_job(ce_saveglobe_portal_t* globe, const char* job_name) {
    if (!globe || !globe->current_character) return;
    
    strncpy(globe->current_character->current_job, job_name, 31);
    printf("Job changed to %s.\n", job_name);
}

void ce_saveglobe_equip_asset(ce_saveglobe_portal_t* globe, int asset_id, int slot) {
    if (!globe || !globe->current_character) return;
    printf("Equipping Asset ID %d to slot %d.\n", asset_id, slot);
    // Logic to equip from inventory
}

bool ce_saveglobe_save_game(ce_saveglobe_portal_t* globe) {
    if (!globe || !globe->is_connected) return false;
    
    printf("Saving game state to Slot %d...\n", globe->save_slot_id);
    printf("Syncing wallet balance to blockchain...\n");
    printf("Game Saved Successfully!\n");
    return true;
}

bool ce_saveglobe_load_game(ce_saveglobe_portal_t* globe, int slot_id) {
    printf("Loading game from Slot %d...\n", slot_id);
    // Logic to load
    return true;
}

void ce_saveglobe_browse_marketplace(ce_saveglobe_portal_t* globe) {
    if (!globe || !globe->is_connected) return;
    printf("Browsing marketplace (In-Game Mode)...\n");
    // Limited marketplace view
}

bool ce_saveglobe_purchase_item(ce_saveglobe_portal_t* globe, int listing_id) {
    if (!globe || !globe->is_connected) return false;
    printf("Attempting to purchase Listing %d...\n", listing_id);
    // Purchase logic
    return true;
}
