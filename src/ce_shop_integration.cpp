// ce_shop_integration.cpp
// Shop Integration with Crypto Payments Implementation

#include "ce_shop_integration.h"
#include <chrono>
#include <random>
#include <sstream>
#include <iomanip>
#include <algorithm>

namespace CrystalEchoes {
namespace Shop {

ShopIntegrationManager& ShopIntegrationManager::Instance() {
    static ShopIntegrationManager instance;
    return instance;
}

void ShopIntegrationManager::Initialize() {
    std::lock_guard<std::mutex> lock(m_mutex);
    m_gilExchangeRate = 1000000; // 1 FFX-T = 1,000,000 Gil (default)
    m_totalGilRevenue = 0;
    m_totalTokenRevenue = 0;
    
    // Initialize crypto engine
    Crypto::FFXTokenEngine::Instance().Initialize();
}

bool ShopIntegrationManager::RegisterShop(const ShopNPC& shop) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    if (m_shops.find(shop.npcId) != m_shops.end()) {
        return false; // Shop already exists
    }
    
    m_shops[shop.npcId] = shop;
    return true;
}

ShopNPC* ShopIntegrationManager::GetShop(const std::string& shopId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    auto it = m_shops.find(shopId);
    if (it != m_shops.end()) {
        return &it->second;
    }
    return nullptr;
}

std::vector<ShopNPC*> ShopIntegrationManager::GetAllShops() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopNPC*> result;
    for (auto& pair : m_shops) {
        result.push_back(&pair.second);
    }
    return result;
}

std::vector<ShopNPC*> ShopIntegrationManager::GetShopsByZone(const std::string& zone) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopNPC*> result;
    for (auto& pair : m_shops) {
        if (pair.second.zone == zone) {
            result.push_back(&pair.second);
        }
    }
    return result;
}

bool ShopIntegrationManager::AddItemToShop(const std::string& shopId, const ShopItem& item) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto shopIt = m_shops.find(shopId);
    if (shopIt == m_shops.end()) {
        return false;
    }
    
    shopIt->second.items.push_back(item);
    return true;
}

bool ShopIntegrationManager::RemoveItemFromShop(const std::string& shopId, const std::string& itemId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto shopIt = m_shops.find(shopId);
    if (shopIt == m_shops.end()) {
        return false;
    }
    
    auto& items = shopIt->second.items;
    auto it = std::find_if(items.begin(), items.end(),
                           [&itemId](const ShopItem& item) { return item.itemId == itemId; });
    
    if (it == items.end()) {
        return false;
    }
    
    items.erase(it);
    return true;
}

bool ShopIntegrationManager::UpdateItemPrice(const std::string& shopId, const std::string& itemId,
                                             uint64_t gilPrice, uint64_t tokenPrice) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto shopIt = m_shops.find(shopId);
    if (shopIt == m_shops.end()) {
        return false;
    }
    
    auto& items = shopIt->second.items;
    auto it = std::find_if(items.begin(), items.end(),
                           [&itemId](const ShopItem& item) { return item.itemId == itemId; });
    
    if (it == items.end()) {
        return false;
    }
    
    it->gilPrice = gilPrice;
    it->tokenPrice = tokenPrice;
    return true;
}

bool ShopIntegrationManager::PurchaseWithGil(const std::string& playerId, const std::string& shopId,
                                             const std::string& itemId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto shopIt = m_shops.find(shopId);
    if (shopIt == m_shops.end()) {
        return false;
    }
    
    auto itemIt = std::find_if(shopIt->second.items.begin(), shopIt->second.items.end(),
                               [&itemId](const ShopItem& item) { return item.itemId == itemId; });
    
    if (itemIt == shopIt->second.items.end()) {
        return false;
    }
    
    if (!ValidatePurchase(playerId, *itemIt, CurrencyType::Gil)) {
        return false;
    }
    
    DeductPayment(playerId, itemIt->gilPrice, CurrencyType::Gil);
    AddItemToPlayerInventory(playerId, *itemIt);
    
    // Record purchase
    PlayerPurchase record;
    record.playerId = playerId;
    record.itemId = itemId;
    record.shopId = shopId;
    record.price = itemIt->gilPrice;
    record.currencyUsed = CurrencyType::Gil;
    record.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    record.transactionHash = GenerateTransactionId();
    
    m_playerPurchases[playerId].push_back(record);
    m_totalGilRevenue += itemIt->gilPrice;
    
    // Update quantity
    if (itemIt->quantity > 0) {
        itemIt->quantity--;
    }
    
    return true;
}

bool ShopIntegrationManager::PurchaseWithToken(const std::string& playerId, const std::string& shopId,
                                               const std::string& itemId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto shopIt = m_shops.find(shopId);
    if (shopIt == m_shops.end()) {
        return false;
    }
    
    auto itemIt = std::find_if(shopIt->second.items.begin(), shopIt->second.items.end(),
                               [&itemId](const ShopItem& item) { return item.itemId == itemId; });
    
    if (itemIt == shopIt->second.items.end()) {
        return false;
    }
    
    if (!ValidatePurchase(playerId, *itemIt, CurrencyType::FFXToken)) {
        return false;
    }
    
    // Execute crypto transaction
    auto& tokenEngine = Crypto::FFXTokenEngine::Instance();
    
    // In production, would transfer to shop owner's wallet
    // For now, just deduct from player
    if (tokenEngine.GetBalance(playerId) < itemIt->tokenPrice) {
        return false;
    }
    
    tokenEngine.SendTransaction(playerId, "SHOP_" + shopId, itemIt->tokenPrice);
    
    AddItemToPlayerInventory(playerId, *itemIt);
    
    // Record purchase
    PlayerPurchase record;
    record.playerId = playerId;
    record.itemId = itemId;
    record.shopId = shopId;
    record.price = itemIt->tokenPrice;
    record.currencyUsed = CurrencyType::FFXToken;
    record.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    record.transactionHash = tokenEngine.GenerateHash(playerId + itemId + shopId);
    
    m_playerPurchases[playerId].push_back(record);
    m_totalTokenRevenue += itemIt->tokenPrice;
    
    // Update quantity
    if (itemIt->quantity > 0) {
        itemIt->quantity--;
    }
    
    return true;
}

bool ShopIntegrationManager::PurchaseHybrid(const std::string& playerId, const std::string& shopId,
                                            const std::string& itemId, float gilPercentage) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    if (gilPercentage < 0.0f || gilPercentage > 1.0f) {
        return false;
    }
    
    auto shopIt = m_shops.find(shopId);
    if (shopIt == m_shops.end()) {
        return false;
    }
    
    auto itemIt = std::find_if(shopIt->second.items.begin(), shopIt->second.items.end(),
                               [&itemId](const ShopItem& item) { return item.itemId == itemId; });
    
    if (itemIt == shopIt->second.items.end()) {
        return false;
    }
    
    uint64_t gilAmount = static_cast<uint64_t>(itemIt->gilPrice * gilPercentage);
    uint64_t tokenAmount = static_cast<uint64_t>(itemIt->tokenPrice * (1.0f - gilPercentage));
    
    // Validate both payments
    auto& tokenEngine = Crypto::FFXTokenEngine::Instance();
    
    if (tokenEngine.GetBalance(playerId) < tokenAmount) {
        return false;
    }
    
    // Process hybrid payment
    if (gilAmount > 0) {
        DeductPayment(playerId, gilAmount, CurrencyType::Gil);
    }
    if (tokenAmount > 0) {
        tokenEngine.SendTransaction(playerId, "SHOP_" + shopId, tokenAmount);
    }
    
    AddItemToPlayerInventory(playerId, *itemIt);
    
    // Record purchase
    PlayerPurchase record;
    record.playerId = playerId;
    record.itemId = itemId;
    record.shopId = shopId;
    record.price = gilAmount + TokensToGil(tokenAmount);
    record.currencyUsed = CurrencyType::Hybrid;
    record.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    record.transactionHash = tokenEngine.GenerateHash(playerId + itemId + shopId + "hybrid");
    
    m_playerPurchases[playerId].push_back(record);
    m_totalGilRevenue += gilAmount;
    m_totalTokenRevenue += tokenAmount;
    
    // Update quantity
    if (itemIt->quantity > 0) {
        itemIt->quantity--;
    }
    
    return true;
}

std::vector<ShopItem> ShopIntegrationManager::GetPlayerPurchases(const std::string& playerId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopItem> result;
    
    auto it = m_playerPurchases.find(playerId);
    if (it == m_playerPurchases.end()) {
        return result;
    }
    
    // Get unique items purchased
    std::unordered_map<std::string, ShopItem> itemMap;
    for (const auto& purchase : it->second) {
        if (itemMap.find(purchase.itemId) == itemMap.end()) {
            // Find the item in shops
            for (auto& shopPair : m_shops) {
                auto itemIt = std::find_if(shopPair.second.items.begin(), 
                                           shopPair.second.items.end(),
                                           [&id = purchase.itemId](const ShopItem& item) {
                                               return item.itemId == id;
                                           });
                if (itemIt != shopPair.second.items.end()) {
                    itemMap[purchase.itemId] = *itemIt;
                    break;
                }
            }
        }
    }
    
    for (const auto& pair : itemMap) {
        result.push_back(pair.second);
    }
    
    return result;
}

bool ShopIntegrationManager::HasPurchasedItem(const std::string& playerId, const std::string& itemId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto it = m_playerPurchases.find(playerId);
    if (it == m_playerPurchases.end()) {
        return false;
    }
    
    for (const auto& purchase : it->second) {
        if (purchase.itemId == itemId) {
            return true;
        }
    }
    
    return false;
}

int ShopIntegrationManager::GetPurchaseCount(const std::string& playerId, const std::string& itemId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto it = m_playerPurchases.find(playerId);
    if (it == m_playerPurchases.end()) {
        return 0;
    }
    
    int count = 0;
    for (const auto& purchase : it->second) {
        if (purchase.itemId == itemId) {
            count++;
        }
    }
    
    return count;
}

std::vector<ShopItem> ShopIntegrationManager::GetPremiumItems() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopItem> result;
    
    for (auto& shopPair : m_shops) {
        for (const auto& item : shopPair.second.items) {
            if (item.acceptedCurrency == CurrencyType::FFXToken || 
                shopPair.second.isPremiumVendor) {
                result.push_back(item);
            }
        }
    }
    
    return result;
}

std::vector<ShopItem> ShopIntegrationManager::GetLimitedEditionItems() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopItem> result;
    
    uint64_t now = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    
    for (auto& shopPair : m_shops) {
        for (const auto& item : shopPair.second.items) {
            if (item.isLimitedEdition && (item.availableUntil == 0 || item.availableUntil > now)) {
                result.push_back(item);
            }
        }
    }
    
    return result;
}

std::vector<ShopItem> ShopIntegrationManager::GetDailySpecials() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopItem> result;
    
    // In production, would filter by daily rotation
    // For now, return limited edition items as specials
    return GetLimitedEditionItems();
}

uint64_t ShopIntegrationManager::GetTotalGilRevenue() {
    std::lock_guard<std::mutex> lock(m_mutex);
    return m_totalGilRevenue;
}

uint64_t ShopIntegrationManager::GetTotalTokenRevenue() {
    std::lock_guard<std::mutex> lock(m_mutex);
    return m_totalTokenRevenue;
}

std::vector<PlayerPurchase> ShopIntegrationManager::GetSalesHistory(const std::string& shopId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<PlayerPurchase> result;
    
    for (const auto& playerPair : m_playerPurchases) {
        for (const auto& purchase : playerPair.second) {
            if (purchase.shopId == shopId) {
                result.push_back(purchase);
            }
        }
    }
    
    return result;
}

std::vector<ShopItem> ShopIntegrationManager::GetBestSellers(uint32_t limit) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    std::unordered_map<std::string, std::pair<ShopItem*, int>> salesCount;
    
    for (auto& shopPair : m_shops) {
        for (auto& item : shopPair.second.items) {
            salesCount[item.itemId] = {&item, 0};
        }
    }
    
    for (const auto& playerPair : m_playerPurchases) {
        for (const auto& purchase : playerPair.second) {
            auto it = salesCount.find(purchase.itemId);
            if (it != salesCount.end()) {
                it->second.second++;
            }
        }
    }
    
    std::vector<std::pair<ShopItem*, int>> sorted;
    for (const auto& pair : salesCount) {
        sorted.push_back(pair.second);
    }
    
    std::sort(sorted.begin(), sorted.end(),
              [](const auto& a, const auto& b) { return a.second > b.second; });
    
    std::vector<ShopItem> result;
    for (size_t i = 0; i < std::min(static_cast<size_t>(limit), sorted.size()); ++i) {
        result.push_back(*sorted[i].first);
    }
    
    return result;
}

uint64_t ShopIntegrationManager::GilToTokens(uint64_t gil) {
    std::lock_guard<std::mutex> lock(m_mutex);
    return gil / m_gilExchangeRate;
}

uint64_t ShopIntegrationManager::TokensToGil(uint64_t tokens) {
    std::lock_guard<std::mutex> lock(m_mutex);
    return tokens * m_gilExchangeRate;
}

void ShopIntegrationManager::SetExchangeRate(uint64_t gilPerToken) {
    std::lock_guard<std::mutex> lock(m_mutex);
    m_gilExchangeRate = gilPerToken;
}

bool ShopIntegrationManager::ValidatePurchase(const std::string& playerId, const ShopItem& item, 
                                              CurrencyType currency) {
    // Check quantity
    if (item.quantity == 0) {
        return false;
    }
    
    // Check time limit
    uint64_t now = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    
    if (item.isLimitedEdition && item.availableUntil > 0 && item.availableUntil <= now) {
        return false;
    }
    
    // Check purchase limit
    if (item.maxPurchasePerPlayer > 0) {
        int currentCount = 0;
        auto it = m_playerPurchases.find(playerId);
        if (it != m_playerPurchases.end()) {
            for (const auto& purchase : it->second) {
                if (purchase.itemId == item.itemId) {
                    currentCount++;
                }
            }
        }
        
        if (currentCount >= item.maxPurchasePerPlayer) {
            return false;
        }
    }
    
    // Check currency acceptance
    if ((currency == CurrencyType::Gil && item.acceptedCurrency == CurrencyType::FFXToken) ||
        (currency == CurrencyType::FFXToken && item.acceptedCurrency == CurrencyType::Gil)) {
        return false;
    }
    
    // Balance check handled in purchase functions
    
    return true;
}

void ShopIntegrationManager::DeductPayment(const std::string& playerId, uint64_t amount, 
                                           CurrencyType currency) {
    // In production, would interface with game economy system
    // Simplified for framework
}

void ShopIntegrationManager::AddItemToPlayerInventory(const std::string& playerId, 
                                                      const ShopItem& item) {
    // In production, would interface with game inventory system
    // Simplified for framework
}

std::string ShopIntegrationManager::GenerateTransactionId() {
    std::random_device rd;
    std::mt19937_64 gen(rd());
    std::uniform_int_distribution<uint64_t> dist;
    
    std::stringstream ss;
    ss << "TXN-";
    for (int i = 0; i < 4; ++i) {
        ss << std::hex << std::setw(4) << std::setfill('0') << (dist(gen) % 0xFFFF);
    }
    return ss.str();
}

} // namespace Shop
} // namespace CrystalEchoes
