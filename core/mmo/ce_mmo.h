#pragma once

/**
 * Crystal Echoes - MMORPG Module Main Header
 * Combines networking, assets, and world management for MMO functionality
 */

#ifndef CE_MMO_H
#define CE_MMO_H

#include "ce_net.h"
#include "ce_assets.h"
#include "ce_world.h"

namespace CrystalEchoes {
namespace MMO {

    /**
     * Main MMORPG Client Controller
     * Coordinates all MMO subsystems
     */
    class MMOClient {
    public:
        static MMOClient& Instance();

        // Lifecycle
        bool Initialize(const std::string& serverAddress, uint16_t serverPort);
        void Shutdown();
        bool IsRunning() const;

        // Connection State
        Net::ConnectionState GetConnectionState() const;
        void Connect(const std::string& serverAddress, uint16_t serverPort);
        void Disconnect();

        // Authentication
        bool Login(const std::string& username, const std::string& password);
        void Logout();
        bool IsLoggedIn() const;
        uint64_t GetPlayerId() const;

        // World Interaction
        void RequestZoneChange(uint32_t zoneId, float x, float y, float z);
        void SendMovement(float x, float y, float z, float rotation);
        void SendAction(uint32_t actionId, uint64_t targetId = 0);
        void SendChat(World::ChatChannel channel, const std::string& message);

        // Asset Management Integration
        Assets::AssetManager* GetAssetManager();
        Assets::MarketplaceClient* GetMarketplaceClient();
        Assets::SkinManager* GetSkinManager();

        // World State Access
        World::WorldManager* GetWorldManager();
        World::QuestManager* GetQuestManager();
        World::ChatManager* GetChatManager();

        // Event Handlers
        using OnLoginSuccessCallback = std::function<void()>;
        using OnLoginFailureCallback = std::function<void(const std::string& reason)>;
        using OnLogoutCallback = std::function<void()>;
        using OnConnectionLostCallback = std::function<void()>;
        using OnZoneChangedCallback = std::function<void(uint32_t zoneId)>;

        void SetOnLoginSuccess(OnLoginSuccessCallback cb);
        void SetOnLoginFailure(OnLoginFailureCallback cb);
        void SetOnLogout(OnLogoutCallback cb);
        void SetOnConnectionLost(OnConnectionLostCallback cb);
        void SetOnZoneChanged(OnZoneChangedCallback cb);

        // Game Loop (call every frame)
        void Update(float deltaTime);

    private:
        MMOClient() = default;
        ~MMOClient() = default;
        MMOClient(const MMOClient&) = delete;
        MMOClient& operator=(const MMOClient&) = delete;

        std::atomic<bool> m_running;
        std::atomic<bool> m_loggedIn;
        
        Net::NetworkClient m_networkClient;
        uint64_t m_playerId;
        uint32_t m_currentZoneId;

        // Subsystem references
        Assets::AssetManager* m_assetManager;
        Assets::MarketplaceClient* m_marketplaceClient;
        Assets::SkinManager* m_skinManager;
        World::WorldManager* m_worldManager;
        World::QuestManager* m_questManager;
        World::ChatManager* m_chatManager;

        // Callbacks
        OnLoginSuccessCallback m_onLoginSuccess;
        OnLoginFailureCallback m_onLoginFailure;
        OnLogoutCallback m_onLogout;
        OnConnectionLostCallback m_onConnectionLost;
        OnZoneChangedCallback m_onZoneChanged;

        // Internal handlers
        void HandleNetworkPacket(Net::PacketType type, const void* data, size_t size);
        void ProcessServerMessage(const void* data, size_t size);
    };

    /**
     * MMORPG Server Controller (for hosting custom servers)
     */
    class MMOServer {
    public:
        static MMOServer& Instance();

        // Lifecycle
        bool Initialize(uint16_t port, const std::string& worldConfigPath);
        void Shutdown();
        bool IsRunning() const;

        // Server Control
        void Start();
        void Stop();
        
        // Player Management
        size_t GetPlayerCount() const;
        size_t GetMaxPlayers() const;
        void SetMaxPlayers(size_t max);
        void KickPlayer(uint64_t playerId, const std::string& reason);
        void BanPlayer(uint64_t playerId, int64_t durationSeconds);

        // World Control
        void BroadcastMessage(const std::string& message);
        void TriggerWorldEvent(uint32_t eventId);
        void SetWeather(uint32_t zoneId, uint32_t weatherType);
        void SetTimeOfDay(float hour);

        // Database Integration (abstracted)
        bool LoadPlayerData(uint64_t playerId);
        bool SavePlayerData(uint64_t playerId);
        bool CreateNewCharacter(uint64_t accountId, const std::string& charName, 
                               uint32_t classId, uint32_t raceId);

        // Configuration
        struct ServerConfig {
            uint16_t port;
            size_t maxPlayers;
            std::string worldName;
            std::string databaseConnection;
            bool enablePvP;
            bool enableTrading;
            bool enableGuilds;
            float experienceRate;
            float dropRate;
            float currencyRate;
        };

        const ServerConfig& GetConfig() const;
        void UpdateConfig(const ServerConfig& config);

    private:
        MMOServer() = default;
        ~MMOServer() = default;
        MMOServer(const MMOServer&) = delete;
        MMOServer& operator=(const MMOServer&) = delete;

        std::atomic<bool> m_running;
        Net::NetworkServer m_networkServer;
        ServerConfig m_config;
        
        World::WorldManager* m_worldManager;
        World::QuestManager* m_questManager;
        World::ChatManager* m_chatManager;

        // Player session management
        std::mutex m_sessionsMutex;
        // Session data would be stored here
    };

    // Global initialization/shutdown
    bool InitializeMMOSystem();
    void ShutdownMMOSystem();
    bool IsMMOSystemInitialized();

} // namespace MMO
} // namespace CrystalEchoes

#endif // CE_MMO_H
