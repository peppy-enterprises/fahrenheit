#ifndef CE_WALLET_H
#define CE_WALLET_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>
#include <stdint.h>

// Supported Currencies
typedef enum {
    CURRENCY_USD,
    CURRENCY_FFX_T,
    CURRENCY_ETH,
    CURRENCY_BTC,
    CURRENCY_SOL
} ce_currency_t;

// Wallet Balance Entry
typedef struct {
    char currency_code[16];
    double balance;
    double locked; // For pending transactions
} ce_balance_entry_t;

// Main Wallet Structure
typedef struct {
    char wallet_id[64];
    char owner_name[64];
    char pin_hash[64];
    bool is_2fa_enabled;
    ce_balance_entry_t balances[8];
    int balance_count;
    char deposit_addresses[3][64]; // ETH, BTC, SOL
    uint64_t created_at;
} ce_wallet_t;

// Lifecycle
ce_wallet_t* ce_wallet_create(const char* owner, const char* pin);
void ce_wallet_destroy(ce_wallet_t* wallet);

// Balances
double ce_wallet_get_balance(ce_wallet_t* wallet, const char* currency_code);
bool ce_wallet_deposit(ce_wallet_t* wallet, const char* currency_code, double amount);
bool ce_wallet_withdraw(ce_wallet_t* wallet, const char* currency_code, double amount);

// Swapping
bool ce_wallet_swap(ce_wallet_t* wallet, const char* from_currency, const char* to_currency, double amount);

// Security
bool ce_wallet_verify_pin(ce_wallet_t* wallet, const char* pin);
void ce_wallet_enable_2fa(ce_wallet_t* wallet);
bool ce_wallet_validate_2fa(ce_wallet_t* wallet, const char* code);

// Deposit Addresses
const char* ce_wallet_get_deposit_address(ce_wallet_t* wallet, const char* currency_code);

// Display
void ce_wallet_display_status(ce_wallet_t* wallet);

#ifdef __cplusplus
}
#endif

#endif // CE_WALLET_H
