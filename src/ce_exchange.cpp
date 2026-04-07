#include "ce_exchange.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

// Mock exchange rates (USD base)
static struct {
    const char* currency;
    double rate;
} g_rates[] = {
    {"USD", 1.0},
    {"FFX-T", 0.0002},  // 1 FFX-T = $0.0002 (5000 per $1)
    {"ETH", 2000.0},
    {"BTC", 45000.0},
    {"SOL", 100.0}
};
static const int g_rate_count = 5;

static double get_rate_for_currency(const char* currency) {
    for (int i = 0; i < g_rate_count; i++) {
        if (strcmp(g_rates[i].currency, currency) == 0) {
            return g_rates[i].rate;
        }
    }
    return 1.0; // Default
}

double ce_exchange_get_rate(const char* from, const char* to) {
    double from_rate = get_rate_for_currency(from);
    double to_rate = get_rate_for_currency(to);
    
    // Convert from -> USD -> to
    return to_rate > 0 ? from_rate / to_rate : 0.0;
}

double ce_exchange_calculate_swap(const char* from, const char* to, double amount, double* out_fee) {
    double rate = ce_exchange_get_rate(from, to);
    double gross_amount = amount * rate;
    
    // Fee: 0.5% for swaps
    double fee_percent = 0.005;
    *out_fee = gross_amount * fee_percent;
    
    double net_amount = gross_amount - *out_fee;
    
    printf("[Exchange] Rate: 1 %s = %.6f %s\n", from, rate, to);
    printf("[Exchange] Amount: %.2f %s -> %.2f %s (Fee: %.4f)\n", 
           amount, from, net_amount, to, *out_fee);
    
    return net_amount;
}

bool ce_exchange_execute_swap(const char* from_currency, const char* to_currency, double amount, ce_swap_record_t* out_record) {
    if (!out_record) return false;
    
    double fee = 0.0;
    double received = ce_exchange_calculate_swap(from_currency, to_currency, amount, &fee);
    
    if (received <= 0) return false;
    
    // Fill record
    snprintf(out_record->tx_id, sizeof(out_record->tx_id), "SWAP_%lu", (unsigned long)time(NULL));
    strncpy(out_record->from_currency, from_currency, sizeof(out_record->from_currency) - 1);
    strncpy(out_record->to_currency, to_currency, sizeof(out_record->to_currency) - 1);
    out_record->from_amount = amount;
    out_record->to_amount = received;
    out_record->fee = fee;
    out_record->timestamp = time(NULL);
    out_record->completed = true;
    
    return true;
}

void ce_exchange_update_rates() {
    // In real implementation, this would fetch from API
    // For now, just add some randomness to simulate market movement
    double variance = ((double)(rand() % 100) - 50) / 1000.0; // +/- 5%
    
    for (int i = 1; i < g_rate_count; i++) {
        g_rates[i].rate *= (1.0 + variance);
    }
    
    printf("[Exchange] Rates updated with variance: %.3f%%\n", variance * 100);
}
