// ce_marketplace.h
// Crypto Marketplace for Skins, Assets, and In-Game Items
// Integrates with FFX Token Engine

#pragma once

#include "ce_token.h"
#include <string>
#include <vector>
#include <unordered_map>
#include <functional>

namespace CrystalEchoes {
namespace Marketplace {

    enum class AssetType {
        Skin,
        Mount,
        Pet,
        Weapon,
        Armor,
        Emote,
        Housing,
        Consumable,
        Material
    };

    enum class AssetQuality {
        Common,      // 1x
        Uncommon,    // 2x
        Rare,        // 5x
        Epic,        // 10x
        Legendary,   // 25x
        Mythic,      // 50x
        Divine       // 100x (8K Ultra)
    };

    struct Asset {
        std::string id;
        std::string name;
        std::string description;
        AssetType type;
        AssetQuality quality;
        std::string creatorAddress;
        uint64_t price; // In FFX-T satoshis
        std::string modelPath;
        std::string texturePath;
        std::string previewPath;
        uint64_t createdDate;
        uint64_t lastSalePrice;
        uint32_t totalSales;
        float royaltyPercent; // Creator royalty on resales (e.g., 5.0 = 5%)
        bool isLimited;
        uint32_t maxSupply;
        uint32_t currentSupply;
        std::vector<std::string> compatibleJobs;
        std::unordered_map<std::string, int> statBonuses;
    };

    struct ShopListing {
        std::string listingId;
        Asset asset;
        std::string sellerAddress;
        uint64_t price;
        uint64_t listedDate;
        bool isAuction;
        uint64_t auctionEndTime;
        uint64_t highestBid;
        std::string highestBidder;
        bool isActive;
    };

    struct PurchaseRecord {
        std::string transactionHash;
        std::string assetId;
        std::string buyer;
        std::string seller;
        uint64_t price;
        uint64_t timestamp;
        uint64_t royaltyPaid;
    };

    class MarketplaceManager {
    public:
        static MarketplaceManager& Instance();

        // Initialization
        void Initialize();

        // Asset Management
        bool RegisterAsset(const Asset& asset);
        Asset* GetAsset(const std::string& assetId);
        std::vector<Asset*> GetAllAssets();
        std::vector<Asset*> GetAssetsByType(AssetType type);
        std::vector<Asset*> GetAssetsByCreator(const std::string& creatorAddress);
        std::vector<Asset*> SearchAssets(const std::string& query);

        // Shop Listings
        bool CreateListing(const std::string& assetId, const std::string& sellerAddress, 
                          uint64_t price, bool isAuction = false, uint64_t auctionDuration = 0);
        ShopListing* GetListing(const std::string& listingId);
        std::vector<ShopListing*> GetAllListings();
        std::vector<ShopListing*> GetActiveListings();
        std::vector<ShopListing*> GetListingsBySeller(const std::string& sellerAddress);
        bool CancelListing(const std::string& listingId, const std::string& ownerAddress);

        // Purchases
        bool BuyAsset(const std::string& listingId, const std::string& buyerAddress);
        bool PlaceBid(const std::string& listingId, const std::string& bidderAddress, uint64_t amount);
        bool FinalizeAuction(const std::string& listingId);

        // User Inventory
        std::vector<Asset*> GetUserInventory(const std::string& userAddress);
        bool TransferAsset(const std::string& assetId, const std::string& from, const std::string& to);
        bool EquipAsset(const std::string& userAddress, const std::string& assetId, int slot);
        bool UnequipAsset(const std::string& userAddress, int slot);
        Asset* GetEquippedAsset(const std::string& userAddress, int slot);

        // Analytics
        uint64_t GetTotalVolume();
        uint64_t GetCreatorEarnings(const std::string& creatorAddress);
        std::vector<PurchaseRecord> GetPurchaseHistory(const std::string& userAddress);
        std::vector<Asset*> GetTrendingAssets(uint32_t limit = 10);

        // Admin
        bool SetRoyaltyPercent(const std::string& assetId, float percent);
        bool VerifyAssetAuthenticity(const std::string& assetId);

    private:
        MarketplaceManager() = default;
        std::unordered_map<std::string, Asset> m_assets;
        std::unordered_map<std::string, ShopListing> m_listings;
        std::unordered_map<std::string, std::vector<std::string>> m_userInventories; // address -> assetIds
        std::unordered_map<std::string, std::unordered_map<int, std::string>> m_userEquipped; // address -> slot -> assetId
        std::vector<PurchaseRecord> m_purchaseHistory;
        std::mutex m_mutex;

        void DistributePayment(const std::string& buyer, const std::string& seller, 
                              const std::string& creator, uint64_t price, float royaltyPercent);
        std::string GenerateListingId();
        std::string GenerateAssetId();
    };

} // namespace Marketplace
} // namespace CrystalEchoes
