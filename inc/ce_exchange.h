#ifndef CE_EXCHANGE_H
#define CE_EXCHANGE_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>

// Exchange Rate Structure
typedef struct {
    char from_currency[16];
    char to_currency[16];
    double rate;
    double fee_percent;
} ce_exchange_rate_t;

// Swap Transaction Record
typedef struct {
    char tx_id[64];
    char from_currency[16];
    char to_currency[16];
    double from_amount;
    double to_amount;
    double fee;
    long timestamp;
    bool completed;
} ce_swap_record_t;

// Get exchange rate between two currencies
double ce_exchange_get_rate(const char* from, const char* to);

// Calculate swap result (returns amount received after fees)
double ce_exchange_calculate_swap(const char* from, const char* to, double amount, double* out_fee);

// Execute swap (mock implementation)
bool ce_exchange_execute_swap(const char* from_currency, const char* to_currency, double amount, ce_swap_record_t* out_record);

// Get current market prices (mock)
void ce_exchange_update_rates();

#ifdef __cplusplus
}
#endif

#endif // CE_EXCHANGE_H
