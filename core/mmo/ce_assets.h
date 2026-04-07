#pragma once

/**
 * Crystal Echoes - Asset Management System
 * Handles custom assets, marketplace integration, and asset streaming for MMORPG
 */

#ifndef CE_ASSETS_H
#define CE_ASSETS_H

#include <string>
#include <vector>
#include <unordered_map>
#include <memory>
#include <functional>
#include <mutex>
#include <atomic>
#include <cstdint>

namespace CrystalEchoes {
namespace Assets {

    // Asset Types supported
    enum class AssetType : uint8_t {
        UNKNOWN = 0,
        TEXTURE_2D = 1,
        TEXTURE_CUBE = 2,
        MODEL_3D = 3,
        ANIMATION = 4,
        AUDIO = 5,
        SHADER = 6,
        SCRIPT = 7,
        UI_ELEMENT = 8,
        SKIN = 9,
        MOUNT = 10,
        PET = 11,
        EMOTE = 12,
        HOUSING_ITEM = 13,
        WORLD_OBJECT = 14
    };

    // Asset Quality Levels
    enum class AssetQuality : uint8_t {
        LOW = 0,      // 512x512 or equivalent
        MEDIUM = 1,   // 1024x1024
        HIGH = 2,     // 2048x2048
        ULTRA = 3,    // 4096x4096
        EXTREME = 4   // 8192x8192
    };

    // Marketplace Item Information
    struct MarketplaceItem {
        std::string itemId;
        std::string name;
        std::string description;
        std::string creatorId;
        std::string creatorName;
        AssetType type;
        AssetQuality quality;
        uint64_t priceCredits;
        uint64_t priceRealMoney; // In cents
        std::string thumbnailUrl;
        std::string previewUrl;
        uint32_t downloadCount;
        float rating;
        uint32_t ratingCount;
        bool isFree;
        bool isOfficial;
        std::vector<std::string> tags;
        std::string compatibleVersion;
        int64_t createdAt;
        int64_t updatedAt;
    };

    // User's Owned Asset
    struct OwnedAsset {
        std::string assetId;
        std::string marketplaceItemId;
        std::string ownerId;
        AssetType type;
        std::string localPath;
        bool isDownloaded;
        bool isActive;
        int64_t acquiredAt;
        std::string metadata; // JSON string with additional data
    };

    // Asset Download Progress
    struct DownloadProgress {
        std::string assetId;
        uint64_t totalBytes;
        uint64_t downloadedBytes;
        float percentComplete;
        bool isComplete;
        bool hasError;
        std::string errorMessage;
    };

    // Callbacks
    using OnAssetDownloadedCallback = std::function<void(const std::string& assetId, bool success)>;
    using OnDownloadProgressCallback = std::function<void(const DownloadProgress& progress)>;
    using OnMarketplaceRefreshCallback = std::function<void(bool success)>;

    /**
     * Asset Manager - Core system for managing game assets
     */
    class AssetManager {
    public:
        static AssetManager& Instance();

        // Initialization
        bool Initialize(const std::string& cacheDirectory);
        void Shutdown();

        // Asset Loading
        void* LoadAsset(const std::string& assetId, AssetType type);
        void UnloadAsset(const std::string& assetId);
        void UnloadAllAssets();

        // Asset Streaming (for large worlds)
        void RequestAssetStream(const std::string& assetId, float priority);
        void CancelAssetStream(const std::string& assetId);

        // Cache Management
        void ClearCache();
        uint64_t GetCacheSize() const;
        void SetCacheLimit(uint64_t maxBytes);
        void OptimizeCache();

        // Custom Asset Registration (for modders)
        bool RegisterCustomAsset(const std::string& assetId, const std::string& filePath, AssetType type);
        bool UnregisterCustomAsset(const std::string& assetId);

    private:
        AssetManager() = default;
        ~AssetManager() = default;
        AssetManager(const AssetManager&) = delete;
        AssetManager& operator=(const AssetManager&) = delete;

        std::mutex m_assetsMutex;
        std::unordered_map<std::string, void*> m_loadedAssets;
        std::unordered_map<std::string, std::string> m_assetPaths;
        
        std::string m_cacheDirectory;
        uint64_t m_cacheLimit;
        std::atomic<uint64_t> m_currentCacheSize;
    };

    /**
     * Marketplace Client - Interface to the asset marketplace
     */
    class MarketplaceClient {
    public:
        static MarketplaceClient& Instance();

        // Initialization
        bool Initialize(const std::string& apiEndpoint, const std::string& authToken);
        void Shutdown();

        // Authentication
        bool Login(const std::string& username, const std::string& password);
        void Logout();
        bool IsLoggedIn() const;
        std::string GetCurrentUserId() const;

        // Browsing & Search
        void BrowseCategories(std::function<void(bool, const std::vector<std::string>&)> callback);
        void SearchAssets(const std::string& query, 
                         AssetType filter = AssetType::UNKNOWN,
                         int page = 1, int pageSize = 20,
                         std::function<void(bool, const std::vector<MarketplaceItem>&)> callback = nullptr);
        
        void GetFeaturedAssets(std::function<void(bool, const std::vector<MarketplaceItem>&)> callback);
        void GetNewReleases(std::function<void(bool, const std::vector<MarketplaceItem>&)> callback);
        void GetTopRated(std::function<void(bool, const std::vector<MarketplaceItem>&)> callback);

        // Asset Details
        void GetAssetDetails(const std::string& itemId,
                            std::function<void(bool, const MarketplaceItem&)> callback);

        // Purchasing & Downloads
        void PurchaseAsset(const std::string& itemId,
                          std::function<void(bool, const std::string& transactionId)> callback);
        
        void DownloadAsset(const std::string& itemId,
                          OnAssetDownloadedCallback onComplete,
                          OnDownloadProgressCallback onProgress = nullptr);

        void CancelDownload(const std::string& itemId);

        // User's Library
        void GetOwnedAssets(std::function<void(bool, const std::vector<OwnedAsset>&)> callback);
        void GetWishlist(std::function<void(bool, const std::vector<MarketplaceItem>&)> callback);
        void AddToWishlist(const std::string& itemId,
                          std::function<void(bool)> callback);
        void RemoveFromWishlist(const std::string& itemId,
                               std::function<void(bool)> callback);

        // Creator Functions
        void UploadAsset(const std::string& filePath,
                        const std::string& name,
                        const std::string& description,
                        AssetType type,
                        AssetQuality quality,
                        uint64_t priceCredits,
                        const std::vector<std::string>& tags,
                        std::function<void(bool, const std::string& itemId)> callback);

        void UpdateAssetListing(const std::string& itemId,
                               const std::string& name,
                               const std::string& description,
                               uint64_t priceCredits,
                               const std::vector<std::string>& tags,
                               std::function<void(bool)> callback);

        void DeleteAsset(const std::string& itemId,
                        std::function<void(bool)> callback);

        // Reviews & Ratings
        void SubmitRating(const std::string& itemId, float rating,
                         std::function<void(bool)> callback);
        
        void SubmitReview(const std::string& itemId, const std::string& review,
                         std::function<void(bool)> callback);

        // Currency & Wallet
        void GetWalletBalance(std::function<void(bool, uint64_t credits, uint64_t realMoneyCents)> callback);
        void AddFunds(uint64_t amountCents,
                     std::function<void(bool, const std::string& transactionId)> callback);

    private:
        MarketplaceClient() = default;
        ~MarketplaceClient() = default;
        MarketplaceClient(const MarketplaceClient&) = delete;
        MarketplaceClient& operator=(const MarketplaceClient&) = delete;

        std::string m_apiEndpoint;
        std::string m_authToken;
        std::string m_currentUserId;
        std::atomic<bool> m_loggedIn;

        std::mutex m_downloadsMutex;
        std::unordered_map<std::string, DownloadProgress> m_activeDownloads;
    };

    /**
     * Skin System - Character customization through marketplace assets
     */
    class SkinManager {
    public:
        static SkinManager& Instance();

        // Initialize with AssetManager reference
        void Initialize(AssetManager* assetMgr);

        // Apply skins to character/equipment
        bool EquipSkin(const std::string& assetId, const std::string& slot);
        bool UnequipSkin(const std::string& slot);
        void UnequipAllSkins();

        // Get current equipped skins
        std::unordered_map<std::string, std::string> GetEquippedSkins() const;

        // Preview skin before purchasing
        void* PreviewSkin(const std::string& assetId);

        // Skin slots
        enum class SkinSlot {
            HEAD,
            BODY,
            HANDS,
            LEGS,
            FEET,
            WEAPON_MAIN,
            WEAPON_OFF,
            ACCESSORY_1,
            ACCESSORY_2,
            ACCESSORY_3,
            FACE,
            HAIR,
            EYES,
            SKIN_COLOR,
            TATTOO,
            FULL_BODY // Overrides all body parts
        };

        static std::string SlotToString(SkinSlot slot);
        static SkinSlot StringToSlot(const std::string& str);

    private:
        SkinManager() = default;
        ~SkinManager() = default;
        SkinManager(const SkinManager&) = delete;
        SkinManager& operator=(const SkinManager&) = delete;

        AssetManager* m_assetManager;
        std::mutex m_equippedMutex;
        std::unordered_map<std::string, std::string> m_equippedSkins; // slot -> assetId
    };

    // Utility functions
    AssetType StringToAssetType(const std::string& str);
    std::string AssetTypeToString(AssetType type);
    
    AssetQuality StringToAssetQuality(const std::string& str);
    std::string AssetQualityToString(AssetQuality quality);

} // namespace Assets
} // namespace CrystalEchoes

#endif // CE_ASSETS_H
