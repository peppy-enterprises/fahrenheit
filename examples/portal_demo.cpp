#include "ce_title_portal.h"
#include "ce_saveglobe_portal.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <cstring>
#include <time.h>

int main() {
    printf("=== CRYSTAL ECHOES: PORTAL SYSTEM DEMO ===\n\n");

    // 1. Initialize Title Portal
    ce_title_portal_t* title_portal = ce_portal_create();
    
    int running = 1;
    while (running) {
        ce_portal_render_main_menu(title_portal);
        
        int choice;
        if (scanf("%d", &choice) != 1) {
            printf("Invalid input.\n");
            continue;
        }

        switch (choice) {
            case 0: // Back/Cancel in submenus
                ce_portal_handle_input(title_portal, 0);
                break;
            case 1:
                if (title_portal->is_authenticated && title_portal->state == PORTAL_STATE_IDLE) {
                    ce_portal_open_character_select(title_portal);
                    int char_id;
                    scanf("%d", &char_id);
                    if (char_id > 0) {
                        if (ce_portal_launch_game(title_portal, char_id - 1)) {
                            printf("\nGame Launched! (Simulation End)\n");
                            running = 0;
                        }
                    }
                } else {
                    ce_portal_handle_input(title_portal, 1);
                }
                break;
            case 2:
                ce_portal_handle_input(title_portal, 2);
                if (title_portal->state == PORTAL_STATE_WALLET && title_portal->active_wallet) {
                    int wallet_choice;
                    scanf("%d", &wallet_choice);
                    if (wallet_choice == 1) {
                        double amount;
                        printf("Enter USD amount: ");
                        scanf("%lf", &amount);
                        ce_wallet_deposit(title_portal->active_wallet, "USD", amount);
                    } else if (wallet_choice == 3) {
                        ce_wallet_swap(title_portal->active_wallet, "USD", "FFX-T", 100.0);
                    }
                } else if (title_portal->state == PORTAL_STATE_MARKETPLACE) {
                    int market_choice;
                    scanf("%d", &market_choice);
                    if (market_choice == 1) {
                        printf("Buying item (Demo)...\n");
                    }
                }
                break;
            case 3:
                if (title_portal->is_authenticated) {
                    ce_portal_handle_input(title_portal, 3); // Logout
                } else {
                    running = 0;
                }
                break;
            case 4:
                if (title_portal->is_authenticated) {
                    ce_portal_logout(title_portal);
                }
                break;
            case 5:
                running = 0;
                break;
            default:
                printf("Unknown option.\n");
        }
    }

    // 2. Demonstrate Save Globe functionality
    printf("\n\n=== SAVE GLOBE DEMO ===\n");
    
    // Create a dummy character for the save globe
    ce_character_t demo_char;
    strcpy(demo_char.name, "Hero");
    strcpy(demo_char.current_job, "Knight");
    demo_char.stats.str = 25;
    demo_char.stats.dex = 10;
    demo_char.stats.vit = 20;
    demo_char.level = 10;

    ce_saveglobe_portal_t* save_globe = ce_saveglobe_create(1);
    ce_saveglobe_connect(save_globe, &demo_char);

    int globe_running = 1;
    while (globe_running) {
        printf("\n--- SAVE GLOBE MENU ---\n");
        printf("[1] Open Wallet\n");
        printf("[2] View Stats\n");
        printf("[3] Change Job\n");
        printf("[4] Save Game\n");
        printf("[5] Disconnect\n");
        printf("[0] Exit Demo\n");
        printf("Choice: ");

        int choice;
        scanf("%d", &choice);

        switch (choice) {
            case 1:
                ce_saveglobe_open_wallet(save_globe);
                int w_choice;
                scanf("%d", &w_choice);
                if (w_choice == 1) ce_saveglobe_quick_deposit(save_globe, 10.0);
                if (w_choice == 2) ce_saveglobe_quick_withdraw(save_globe, 50.0);
                break;
            case 2:
                ce_saveglobe_view_stats(save_globe);
                break;
            case 3:
                {
                    char job[32];
                    printf("New Job Name: ");
                    scanf("%31s", job);
                    ce_saveglobe_change_job(save_globe, job);
                }
                break;
            case 4:
                ce_saveglobe_save_game(save_globe);
                break;
            case 5:
                ce_saveglobe_disconnect(save_globe);
                globe_running = 0;
                break;
            case 0:
                globe_running = 0;
                break;
        }
    }

    ce_saveglobe_destroy(save_globe);
    ce_portal_destroy(title_portal);

    printf("\nDemo Finished.\n");
    return 0;
}
