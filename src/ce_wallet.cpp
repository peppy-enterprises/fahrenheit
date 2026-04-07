#include "ce_wallet.h"
#include "ce_exchange.h"
#include "ce_payment_gateway.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

// Helper to find balance index
static int find_balance_index(ce_wallet_t* wallet, const char* currency_code) {
    for (int i = 0; i < wallet->balance_count; i++) {
        if (strcmp(wallet->balances[i].currency_code, currency_code) == 0) {
            return i;
        }
    }
    return -1;
}

// Lifecycle
ce_wallet_t* ce_wallet_create(const char* owner, const char* pin) {
    ce_wallet_t* wallet = (ce_wallet_t*)calloc(1, sizeof(ce_wallet_t));
    if (!wallet) return NULL;

    // Generate wallet ID
    snprintf(wallet->wallet_id, sizeof(wallet->wallet_id), "WALLET_%lu_%s", (unsigned long)time(NULL), owner);
    
    // Store owner
    strncpy(wallet->owner_name, owner, sizeof(wallet->owner_name) - 1);
    
    // Simple PIN hash simulation
    snprintf(wallet->pin_hash, sizeof(wallet->pin_hash), "HASH_%s", pin);
    
    wallet->is_2fa_enabled = false;
    wallet->balance_count = 0;
    wallet->created_at = time(NULL);

    // Initialize balances with zero for supported currencies
    const char* currencies[] = {"USD", "FFX-T", "ETH", "BTC", "SOL"};
    for (int i = 0; i < 5; i++) {
        strcpy(wallet->balances[i].currency_code, currencies[i]);
        wallet->balances[i].balance = 0.0;
        wallet->balances[i].locked = 0.0;
        wallet->balance_count++;
    }

    // Generate deposit addresses
    snprintf(wallet->deposit_addresses[0], sizeof(wallet->deposit_addresses[0]), "ETH_0x%s_%lu", owner, (unsigned long)rand());
    snprintf(wallet->deposit_addresses[1], sizeof(wallet->deposit_addresses[1]), "BTC_bc1q%s_%lu", owner, (unsigned long)rand());
    snprintf(wallet->deposit_addresses[2], sizeof(wallet->deposit_addresses[2]), "SOL_%s_%lu", owner, (unsigned long)rand());

    printf("[Wallet] Created wallet %s for %s\n", wallet->wallet_id, owner);
    return wallet;
}

void ce_wallet_destroy(ce_wallet_t* wallet) {
    if (wallet) {
        printf("[Wallet] Destroying wallet %s\n", wallet->wallet_id);
        free(wallet);
    }
}

// Balances
double ce_wallet_get_balance(ce_wallet_t* wallet, const char* currency_code) {
    if (!wallet) return 0.0;
    
    int idx = find_balance_index(wallet, currency_code);
    if (idx >= 0) {
        return wallet->balances[idx].balance;
    }
    return 0.0;
}

bool ce_wallet_deposit(ce_wallet_t* wallet, const char* currency_code, double amount) {
    if (!wallet || amount <= 0) return false;

    int idx = find_balance_index(wallet, currency_code);
    if (idx < 0) {
        // Add new currency if space available
        if (wallet->balance_count >= 8) return false;
        
        idx = wallet->balance_count;
        strcpy(wallet->balances[idx].currency_code, currency_code);
        wallet->balances[idx].balance = 0.0;
        wallet->balances[idx].locked = 0.0;
        wallet->balance_count++;
    }

    wallet->balances[idx].balance += amount;
    printf("[Wallet] Deposited %.2f %s. New balance: %.2f\n", amount, currency_code, wallet->balances[idx].balance);
    return true;
}

bool ce_wallet_withdraw(ce_wallet_t* wallet, const char* currency_code, double amount) {
    if (!wallet || amount <= 0) return false;

    int idx = find_balance_index(wallet, currency_code);
    if (idx < 0) return false;

    if (wallet->balances[idx].balance < amount) {
        printf("[Wallet] Insufficient %s balance.\n", currency_code);
        return false;
    }

    wallet->balances[idx].balance -= amount;
    printf("[Wallet] Withdrew %.2f %s. New balance: %.2f\n", amount, currency_code, wallet->balances[idx].balance);
    return true;
}

// Swapping
bool ce_wallet_swap(ce_wallet_t* wallet, const char* from_currency, const char* to_currency, double amount) {
    if (!wallet) return false;

    // Check source balance
    int from_idx = find_balance_index(wallet, from_currency);
    if (from_idx < 0 || wallet->balances[from_idx].balance < amount) {
        printf("[Wallet] Cannot swap: insufficient %s.\n", from_currency);
        return false;
    }

    // Calculate swap
    double fee = 0.0;
    double received = ce_exchange_calculate_swap(from_currency, to_currency, amount, &fee);
    
    if (received <= 0) {
        printf("[Wallet] Swap calculation failed.\n");
        return false;
    }

    // Execute
    wallet->balances[from_idx].balance -= amount;
    
    int to_idx = find_balance_index(wallet, to_currency);
    if (to_idx < 0) {
        if (wallet->balance_count >= 8) return false;
        to_idx = wallet->balance_count;
        strcpy(wallet->balances[to_idx].currency_code, to_currency);
        wallet->balances[to_idx].balance = 0.0;
        wallet->balances[to_idx].locked = 0.0;
        wallet->balance_count++;
    }
    
    wallet->balances[to_idx].balance += received;

    printf("[Wallet] Swapped %.2f %s -> %.2f %s (Fee: %.2f)\n", 
           amount, from_currency, received, to_currency, fee);
    return true;
}

// Security
bool ce_wallet_verify_pin(ce_wallet_t* wallet, const char* pin) {
    if (!wallet) return false;
    
    char test_hash[64];
    snprintf(test_hash, sizeof(test_hash), "HASH_%s", pin);
    
    return strcmp(wallet->pin_hash, test_hash) == 0;
}

void ce_wallet_enable_2fa(ce_wallet_t* wallet) {
    if (wallet) {
        wallet->is_2fa_enabled = true;
        printf("[Wallet] 2FA enabled for %s\n", wallet->wallet_id);
    }
}

bool ce_wallet_validate_2fa(ce_wallet_t* wallet, const char* code) {
    if (!wallet || !wallet->is_2fa_enabled) return true; // Skip if not enabled
    
    // Mock validation - accept any 6-digit code
    if (strlen(code) == 6) return true;
    
    return false;
}

// Deposit Addresses
const char* ce_wallet_get_deposit_address(ce_wallet_t* wallet, const char* currency_code) {
    if (!wallet) return NULL;
    
    if (strcmp(currency_code, "ETH") == 0) return wallet->deposit_addresses[0];
    if (strcmp(currency_code, "BTC") == 0) return wallet->deposit_addresses[1];
    if (strcmp(currency_code, "SOL") == 0) return wallet->deposit_addresses[2];
    
    return NULL;
}

// Display
void ce_wallet_display_status(ce_wallet_t* wallet) {
    if (!wallet) return;

    printf("\n========== WALLET STATUS ==========\n");
    printf("Wallet ID: %s\n", wallet->wallet_id);
    printf("Owner: %s\n", wallet->owner_name);
    printf("2FA Enabled: %s\n", wallet->is_2fa_enabled ? "Yes" : "No");
    printf("-----------------------------------\n");
    printf("Balances:\n");
    
    for (int i = 0; i < wallet->balance_count; i++) {
        printf("  %s: %.2f (Locked: %.2f)\n", 
               wallet->balances[i].currency_code,
               wallet->balances[i].balance,
               wallet->balances[i].locked);
    }
    
    printf("-----------------------------------\n");
    printf("Deposit Addresses:\n");
    printf("  ETH: %s\n", wallet->deposit_addresses[0]);
    printf("  BTC: %s\n", wallet->deposit_addresses[1]);
    printf("  SOL: %s\n", wallet->deposit_addresses[2]);
    printf("===================================\n");
}
