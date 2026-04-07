#ifndef CE_PAYMENT_GATEWAY_H
#define CE_PAYMENT_GATEWAY_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>

// Payment Gateway Types
typedef enum {
    GATEWAY_STRIPE,      // Credit Card / ACH
    GATEWAY_COINBASE,    // Crypto
    GATEWAY_BITPAY       // Crypto
} ce_gateway_type_t;

// Payment Method
typedef enum {
    PAYMENT_CREDIT_CARD,
    PAYMENT_ACH,
    PAYMENT_ETH,
    PAYMENT_BTC,
    PAYMENT_SOL
} ce_payment_method_t;

// Transaction Status
typedef enum {
    TX_PENDING,
    TX_COMPLETED,
    TX_FAILED,
    TX_CANCELLED
} ce_tx_status_t;

// Payment Transaction Record
typedef struct {
    char tx_id[64];
    ce_gateway_type_t gateway;
    ce_payment_method_t method;
    double amount_usd;
    double amount_crypto;
    char currency_code[16];
    ce_tx_status_t status;
    long timestamp;
    char error_message[256];
} ce_payment_tx_t;

// Initialize payment gateway (mock)
bool ce_gateway_init(ce_gateway_type_t type);

// Process fiat deposit (Credit Card / ACH)
bool ce_gateway_process_fiat_deposit(ce_gateway_type_t gateway, double amount_usd, ce_payment_method_t method, ce_payment_tx_t* out_tx);

// Process crypto purchase
bool ce_gateway_process_crypto_purchase(ce_gateway_type_t gateway, double amount_usd, const char* crypto_currency, ce_payment_tx_t* out_tx);

// Get deposit address for crypto
const char* ce_gateway_get_crypto_deposit_address(const char* currency);

// Verify transaction completion
bool ce_gateway_verify_transaction(const char* tx_id);

#ifdef __cplusplus
}
#endif

#endif // CE_PAYMENT_GATEWAY_H
