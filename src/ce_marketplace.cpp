// ce_marketplace.cpp
// Crypto Marketplace Implementation

#include "ce_marketplace.h"
#include <chrono>
#include <random>
#include <sstream>
#include <iomanip>
#include <algorithm>

namespace CrystalEchoes {
namespace Marketplace {

MarketplaceManager& MarketplaceManager::Instance() {
    static MarketplaceManager instance;
    return instance;
}

void MarketplaceManager::Initialize() {
    std::lock_guard<std::mutex> lock(m_mutex);
    // Initialize marketplace data
}

bool MarketplaceManager::RegisterAsset(const Asset& asset) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    if (m_assets.find(asset.id) != m_assets.end()) {
        return false; // Asset already exists
    }
    
    m_assets[asset.id] = asset;
    
    // Initialize user inventory if needed
    if (m_userInventories.find(asset.creatorAddress) == m_userInventories.end()) {
        m_userInventories[asset.creatorAddress] = std::vector<std::string>();
    }
    
    // Creator gets the first copy
    m_userInventories[asset.creatorAddress].push_back(asset.id);
    
    return true;
}

Asset* MarketplaceManager::GetAsset(const std::string& assetId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    auto it = m_assets.find(assetId);
    if (it != m_assets.end()) {
        return &it->second;
    }
    return nullptr;
}

std::vector<Asset*> MarketplaceManager::GetAllAssets() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<Asset*> result;
    for (auto& pair : m_assets) {
        result.push_back(&pair.second);
    }
    return result;
}

std::vector<Asset*> MarketplaceManager::GetAssetsByType(AssetType type) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<Asset*> result;
    for (auto& pair : m_assets) {
        if (pair.second.type == type) {
            result.push_back(&pair.second);
        }
    }
    return result;
}

std::vector<Asset*> MarketplaceManager::GetAssetsByCreator(const std::string& creatorAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<Asset*> result;
    for (auto& pair : m_assets) {
        if (pair.second.creatorAddress == creatorAddress) {
            result.push_back(&pair.second);
        }
    }
    return result;
}

std::vector<Asset*> MarketplaceManager::SearchAssets(const std::string& query) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<Asset*> result;
    std::string lowerQuery = query;
    std::transform(lowerQuery.begin(), lowerQuery.end(), lowerQuery.begin(), ::tolower);
    
    for (auto& pair : m_assets) {
        std::string lowerName = pair.second.name;
        std::transform(lowerName.begin(), lowerName.end(), lowerName.begin(), ::tolower);
        
        std::string lowerDesc = pair.second.description;
        std::transform(lowerDesc.begin(), lowerDesc.end(), lowerDesc.begin(), ::tolower);
        
        if (lowerName.find(lowerQuery) != std::string::npos ||
            lowerDesc.find(lowerQuery) != std::string::npos) {
            result.push_back(&pair.second);
        }
    }
    return result;
}

bool MarketplaceManager::CreateListing(const std::string& assetId, const std::string& sellerAddress,
                                       uint64_t price, bool isAuction, uint64_t auctionDuration) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto assetIt = m_assets.find(assetId);
    if (assetIt == m_assets.end()) {
        return false;
    }
    
    // Verify seller owns the asset
    auto invIt = m_userInventories.find(sellerAddress);
    if (invIt == m_userInventories.end()) {
        return false;
    }
    
    bool ownsAsset = false;
    for (const auto& id : invIt->second) {
        if (id == assetId) {
            ownsAsset = true;
            break;
        }
    }
    
    if (!ownsAsset) {
        return false;
    }
    
    ShopListing listing;
    listing.listingId = GenerateListingId();
    listing.asset = assetIt->second;
    listing.sellerAddress = sellerAddress;
    listing.price = price;
    listing.listedDate = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    listing.isAuction = isAuction;
    listing.auctionEndTime = isAuction ? listing.listedDate + auctionDuration : 0;
    listing.highestBid = 0;
    listing.isActive = true;
    
    m_listings[listing.listingId] = listing;
    
    return true;
}

ShopListing* MarketplaceManager::GetListing(const std::string& listingId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    auto it = m_listings.find(listingId);
    if (it != m_listings.end()) {
        return &it->second;
    }
    return nullptr;
}

std::vector<ShopListing*> MarketplaceManager::GetAllListings() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopListing*> result;
    for (auto& pair : m_listings) {
        result.push_back(&pair.second);
    }
    return result;
}

std::vector<ShopListing*> MarketplaceManager::GetActiveListings() {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopListing*> result;
    uint64_t now = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    
    for (auto& pair : m_listings) {
        if (pair.second.isActive) {
            if (pair.second.isAuction && pair.second.auctionEndTime <= now) {
                continue; // Auction expired
            }
            result.push_back(&pair.second);
        }
    }
    return result;
}

std::vector<ShopListing*> MarketplaceManager::GetListingsBySeller(const std::string& sellerAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<ShopListing*> result;
    for (auto& pair : m_listings) {
        if (pair.second.sellerAddress == sellerAddress && pair.second.isActive) {
            result.push_back(&pair.second);
        }
    }
    return result;
}

bool MarketplaceManager::CancelListing(const std::string& listingId, const std::string& ownerAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    auto it = m_listings.find(listingId);
    if (it == m_listings.end()) {
        return false;
    }
    if (it->second.sellerAddress != ownerAddress) {
        return false;
    }
    it->second.isActive = false;
    return true;
}

bool MarketplaceManager::BuyAsset(const std::string& listingId, const std::string& buyerAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto listingIt = m_listings.find(listingId);
    if (listingIt == m_listings.end() || !listingIt->second.isActive) {
        return false;
    }
    
    ShopListing& listing = listingIt->second;
    
    // Execute transaction via token engine
    auto& tokenEngine = Crypto::FFXTokenEngine::Instance();
    if (!tokenEngine.ExecuteAssetPurchase(buyerAddress, listing.sellerAddress, 
                                          listing.asset.id, listing.price)) {
        return false;
    }
    
    // Transfer asset ownership
    auto buyerInvIt = m_userInventories.find(buyerAddress);
    if (buyerInvIt == m_userInventories.end()) {
        m_userInventories[buyerAddress] = std::vector<std::string>();
    }
    m_userInventories[buyerAddress].push_back(listing.asset.id);
    
    // Remove from seller inventory
    auto sellerInvIt = m_userInventories.find(listing.sellerAddress);
    if (sellerInvIt != m_userInventories.end()) {
        auto& inv = sellerInvIt->second;
        inv.erase(std::remove(inv.begin(), inv.end(), listing.asset.id), inv.end());
    }
    
    // Update asset stats
    auto assetIt = m_assets.find(listing.asset.id);
    if (assetIt != m_assets.end()) {
        assetIt->second.lastSalePrice = listing.price;
        assetIt->second.totalSales++;
    }
    
    // Record purchase
    PurchaseRecord record;
    record.transactionHash = tokenEngine.GenerateHash(listingId + buyerAddress);
    record.assetId = listing.asset.id;
    record.buyer = buyerAddress;
    record.seller = listing.sellerAddress;
    record.price = listing.price;
    record.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    record.royaltyPaid = static_cast<uint64_t>(listing.price * listing.asset.royaltyPercent / 100.0);
    m_purchaseHistory.push_back(record);
    
    // Distribute payment with royalties
    // (Handled by token engine in production)
    
    listing.isActive = false;
    
    return true;
}

bool MarketplaceManager::PlaceBid(const std::string& listingId, const std::string& bidderAddress, 
                                  uint64_t amount) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto listingIt = m_listings.find(listingId);
    if (listingIt == m_listings.end() || !listingIt->second.isActive || !listingIt->second.isAuction) {
        return false;
    }
    
    uint64_t now = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    
    if (listingIt->second.auctionEndTime <= now) {
        return false;
    }
    
    if (amount <= listingIt->second.highestBid) {
        return false;
    }
    
    // Verify bidder has enough balance
    auto& tokenEngine = Crypto::FFXTokenEngine::Instance();
    if (tokenEngine.GetBalance(bidderAddress) < amount) {
        return false;
    }
    
    listingIt->second.highestBid = amount;
    listingIt->second.highestBidder = bidderAddress;
    
    return true;
}

bool MarketplaceManager::FinalizeAuction(const std::string& listingId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto listingIt = m_listings.find(listingId);
    if (listingIt == m_listings.end() || !listingIt->second.isAuction) {
        return false;
    }
    
    uint64_t now = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    
    if (listingIt->second.auctionEndTime > now) {
        return false; // Auction not ended yet
    }
    
    if (listingIt->second.highestBid == 0 || listingIt->second.highestBidder.empty()) {
        listingIt->second.isActive = false; // No bids, cancel
        return true;
    }
    
    // Complete the sale to highest bidder
    std::string buyer = listingIt->second.highestBidder;
    uint64_t price = listingIt->second.highestBid;
    
    // Execute transaction
    auto& tokenEngine = Crypto::FFXTokenEngine::Instance();
    if (!tokenEngine.ExecuteAssetPurchase(buyer, listingIt->second.sellerAddress,
                                          listingIt->second.asset.id, price)) {
        return false;
    }
    
    // Transfer asset
    m_userInventories[buyer].push_back(listingIt->second.asset.id);
    
    auto sellerInvIt = m_userInventories.find(listingIt->second.sellerAddress);
    if (sellerInvIt != m_userInventories.end()) {
        auto& inv = sellerInvIt->second;
        inv.erase(std::remove(inv.begin(), inv.end(), listingIt->second.asset.id), inv.end());
    }
    
    listingIt->second.isActive = false;
    
    return true;
}

std::vector<Asset*> MarketplaceManager::GetUserInventory(const std::string& userAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<Asset*> result;
    
    auto invIt = m_userInventories.find(userAddress);
    if (invIt == m_userInventories.end()) {
        return result;
    }
    
    for (const auto& assetId : invIt->second) {
        auto assetIt = m_assets.find(assetId);
        if (assetIt != m_assets.end()) {
            result.push_back(&assetIt->second);
        }
    }
    
    return result;
}

bool MarketplaceManager::TransferAsset(const std::string& assetId, const std::string& from, 
                                       const std::string& to) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto fromInvIt = m_userInventories.find(from);
    if (fromInvIt == m_userInventories.end()) {
        return false;
    }
    
    bool found = false;
    for (const auto& id : fromInvIt->second) {
        if (id == assetId) {
            found = true;
            break;
        }
    }
    
    if (!found) {
        return false;
    }
    
    // Remove from sender
    fromInvIt->second.erase(std::remove(fromInvIt->second.begin(), 
                                        fromInvIt->second.end(), assetId), 
                            fromInvIt->second.end());
    
    // Add to receiver
    if (m_userInventories.find(to) == m_userInventories.end()) {
        m_userInventories[to] = std::vector<std::string>();
    }
    m_userInventories[to].push_back(assetId);
    
    return true;
}

bool MarketplaceManager::EquipAsset(const std::string& userAddress, const std::string& assetId, int slot) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    // Verify user owns the asset
    auto invIt = m_userInventories.find(userAddress);
    if (invIt == m_userInventories.end()) {
        return false;
    }
    
    bool ownsAsset = false;
    for (const auto& id : invIt->second) {
        if (id == assetId) {
            ownsAsset = true;
            break;
        }
    }
    
    if (!ownsAsset) {
        return false;
    }
    
    m_userEquipped[userAddress][slot] = assetId;
    return true;
}

bool MarketplaceManager::UnequipAsset(const std::string& userAddress, int slot) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto userEqIt = m_userEquipped.find(userAddress);
    if (userEqIt == m_userEquipped.end()) {
        return false;
    }
    
    auto slotIt = userEqIt->second.find(slot);
    if (slotIt == userEqIt->second.end()) {
        return false;
    }
    
    userEqIt->second.erase(slotIt);
    return true;
}

Asset* MarketplaceManager::GetEquippedAsset(const std::string& userAddress, int slot) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto userEqIt = m_userEquipped.find(userAddress);
    if (userEqIt == m_userEquipped.end()) {
        return nullptr;
    }
    
    auto slotIt = userEqIt->second.find(slot);
    if (slotIt == userEqIt->second.end()) {
        return nullptr;
    }
    
    auto assetIt = m_assets.find(slotIt->second);
    if (assetIt == m_assets.end()) {
        return nullptr;
    }
    
    return &assetIt->second;
}

uint64_t MarketplaceManager::GetTotalVolume() {
    std::lock_guard<std::mutex> lock(m_mutex);
    uint64_t total = 0;
    for (const auto& record : m_purchaseHistory) {
        total += record.price;
    }
    return total;
}

uint64_t MarketplaceManager::GetCreatorEarnings(const std::string& creatorAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    uint64_t total = 0;
    
    for (const auto& record : m_purchaseHistory) {
        auto assetIt = m_assets.find(record.assetId);
        if (assetIt != m_assets.end() && assetIt->second.creatorAddress == creatorAddress) {
            total += record.royaltyPaid;
        }
    }
    
    return total;
}

std::vector<PurchaseRecord> MarketplaceManager::GetPurchaseHistory(const std::string& userAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    std::vector<PurchaseRecord> result;
    
    for (const auto& record : m_purchaseHistory) {
        if (record.buyer == userAddress || record.seller == userAddress) {
            result.push_back(record);
        }
    }
    
    return result;
}

std::vector<Asset*> MarketplaceManager::GetTrendingAssets(uint32_t limit) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    std::vector<std::pair<Asset*, uint32_t>> assetSales;
    for (auto& pair : m_assets) {
        assetSales.push_back({&pair.second, pair.second.totalSales});
    }
    
    std::sort(assetSales.begin(), assetSales.end(),
              [](const auto& a, const auto& b) { return a.second > b.second; });
    
    std::vector<Asset*> result;
    for (size_t i = 0; i < std::min(static_cast<size_t>(limit), assetSales.size()); ++i) {
        result.push_back(assetSales[i].first);
    }
    
    return result;
}

bool MarketplaceManager::SetRoyaltyPercent(const std::string& assetId, float percent) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto assetIt = m_assets.find(assetId);
    if (assetIt == m_assets.end()) {
        return false;
    }
    
    if (percent < 0.0f || percent > 50.0f) { // Max 50% royalty
        return false;
    }
    
    assetIt->second.royaltyPercent = percent;
    return true;
}

bool MarketplaceManager::VerifyAssetAuthenticity(const std::string& assetId) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto assetIt = m_assets.find(assetId);
    if (assetIt == m_assets.end()) {
        return false;
    }
    
    // Verify creator signature and blockchain record
    // (Simplified - in production would verify against blockchain)
    return !assetIt->second.creatorAddress.empty();
}

std::string MarketplaceManager::GenerateListingId() {
    std::random_device rd;
    std::mt19937_64 gen(rd());
    std::uniform_int_distribution<uint64_t> dist;
    
    std::stringstream ss;
    ss << "LST-";
    for (int i = 0; i < 4; ++i) {
        ss << std::hex << std::setw(4) << std::setfill('0') << (dist(gen) % 0xFFFF);
    }
    return ss.str();
}

std::string MarketplaceManager::GenerateAssetId() {
    std::random_device rd;
    std::mt19937_64 gen(rd());
    std::uniform_int_distribution<uint64_t> dist;
    
    std::stringstream ss;
    ss << "AST-";
    for (int i = 0; i < 4; ++i) {
        ss << std::hex << std::setw(4) << std::setfill('0') << (dist(gen) % 0xFFFF);
    }
    return ss.str();
}

} // namespace Marketplace
} // namespace CrystalEchoes
