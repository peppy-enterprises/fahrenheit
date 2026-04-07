// ce_shop_integration.h
// Integration of Crypto Payments into Existing Game Shops
// Allows hybrid currency (Gil + FFX-T) transactions

#pragma once

#include "ce_token.h"
#include "ce_marketplace.h"
#include <string>
#include <vector>
#include <unordered_map>

namespace CrystalEchoes {
namespace Shop {

    enum class CurrencyType {
        Gil,        // Traditional in-game currency
        FFXToken,   // Crypto currency
        Hybrid      // Both accepted
    };

    struct ShopItem {
        std::string itemId;
        std::string name;
        std::string description;
        Marketplace::AssetType type;
        uint64_t gilPrice;
        uint64_t tokenPrice; // In FFX-T satoshis
        CurrencyType acceptedCurrency;
        int quantity; // -1 for infinite
        int maxPurchasePerPlayer;
        std::vector<std::string> requiredQuests;
        std::vector<std::string> requiredFactions;
        int minPlayerLevel;
        bool isLimitedEdition;
        uint64_t availableUntil; // Timestamp
    };

    struct ShopNPC {
        std::string npcId;
        std::string name;
        std::string location;
        std::string zone;
        std::vector<ShopItem> items;
        std::vector<std::string> dialogueGreetings;
        std::vector<std::string> dialogueFarewells;
        bool isPremiumVendor; // Only accepts FFX-T
    };

    struct PlayerPurchase {
        std::string playerId;
        std::string itemId;
        std::string shopId;
        uint64_t price;
        CurrencyType currencyUsed;
        uint64_t timestamp;
        std::string transactionHash; // For crypto purchases
    };

    class ShopIntegrationManager {
    public:
        static ShopIntegrationManager& Instance();

        // Initialization
        void Initialize();

        // Shop Management
        bool RegisterShop(const ShopNPC& shop);
        ShopNPC* GetShop(const std::string& shopId);
        std::vector<ShopNPC*> GetAllShops();
        std::vector<ShopNPC*> GetShopsByZone(const std::string& zone);
        bool AddItemToShop(const std::string& shopId, const ShopItem& item);
        bool RemoveItemFromShop(const std::string& shopId, const std::string& itemId);
        bool UpdateItemPrice(const std::string& shopId, const std::string& itemId, 
                            uint64_t gilPrice, uint64_t tokenPrice);

        // Purchases
        bool PurchaseWithGil(const std::string& playerId, const std::string& shopId, 
                            const std::string& itemId);
        bool PurchaseWithToken(const std::string& playerId, const std::string& shopId,
                              const std::string& itemId);
        bool PurchaseHybrid(const std::string& playerId, const std::string& shopId,
                           const std::string& itemId, float gilPercentage); // 0.0-1.0

        // Player Inventory (Shop Items)
        std::vector<ShopItem> GetPlayerPurchases(const std::string& playerId);
        bool HasPurchasedItem(const std::string& playerId, const std::string& itemId);
        int GetPurchaseCount(const std::string& playerId, const std::string& itemId);

        // Special Shops
        std::vector<ShopItem> GetPremiumItems(); // FFX-T only
        std::vector<ShopItem> GetLimitedEditionItems();
        std::vector<ShopItem> GetDailySpecials();

        // Analytics
        uint64_t GetTotalGilRevenue();
        uint64_t GetTotalTokenRevenue();
        std::vector<PlayerPurchase> GetSalesHistory(const std::string& shopId);
        std::vector<ShopItem> GetBestSellers(uint32_t limit = 10);

        // Conversion Utilities
        uint64_t GilToTokens(uint64_t gil);
        uint64_t TokensToGil(uint64_t tokens);
        void SetExchangeRate(uint64_t gilPerToken);

    private:
        ShopIntegrationManager() = default;
        std::unordered_map<std::string, ShopNPC> m_shops;
        std::unordered_map<std::string, std::vector<PlayerPurchase>> m_playerPurchases;
        uint64_t m_gilExchangeRate; // How much gil per 1 FFX-T
        uint64_t m_totalGilRevenue;
        uint64_t m_totalTokenRevenue;
        std::mutex m_mutex;

        bool ValidatePurchase(const std::string& playerId, const ShopItem& item, CurrencyType currency);
        void DeductPayment(const std::string& playerId, uint64_t amount, CurrencyType currency);
        void AddItemToPlayerInventory(const std::string& playerId, const ShopItem& item);
        std::string GenerateTransactionId();
    };

} // namespace Shop
} // namespace CrystalEchoes
