#pragma once

/**
 * Crystal Echoes - World & Zone Management for MMORPG
 * Handles world state, zone transitions, and entity management
 */

#ifndef CE_WORLD_H
#define CE_WORLD_H

#include <string>
#include <vector>
#include <unordered_map>
#include <functional>
#include <mutex>
#include <atomic>
#include <cstdint>
#include <memory>

namespace CrystalEchoes {
namespace World {

    // Zone Types
    enum class ZoneType : uint8_t {
        OPEN_WORLD = 0,
        DUNGEON = 1,
        RAID = 2,
        ARENA = 3,
        CITY = 4,
        INSTANCE = 5,
        HOUSING = 6,
        HUB = 7
    };

    // Zone Information
    struct ZoneInfo {
        uint32_t zoneId;
        std::string name;
        std::string description;
        ZoneType type;
        uint32_t minLevel;
        uint32_t maxLevel;
        uint32_t maxPlayers;
        float centerX, centerY, centerZ;
        float radius;
        std::string skyboxAssetId;
        std::string ambientMusicAssetId;
        std::vector<uint32_t> connectedZones;
        bool isPvPEnabled;
        bool isFlyingAllowed;
        bool hasWeatherSystem;
        int64_t createdAt;
    };

    // Entity Types in the world
    enum class EntityType : uint8_t {
        PLAYER = 0,
        NPC = 1,
        MONSTER = 2,
        BOSS = 3,
        PET = 4,
        MOUNT = 5,
        VEHICLE = 6,
        GATHERING_NODE = 7,
        CHEST = 8,
        PORTAL = 9,
        WORLD_OBJECT = 10
    };

    // Base Entity Structure
    struct Entity {
        uint64_t entityId;
        EntityType type;
        uint32_t zoneId;
        
        // Position & Rotation
        float x, y, z;
        float rotationX, rotationY, rotationZ;
        
        // Movement
        float velocityX, velocityY, velocityZ;
        bool isMoving;
        uint64_t moveTargetEntityId;
        float moveSpeed;
        
        // Appearance
        std::string modelAssetId;
        std::unordered_map<std::string, std::string> equippedSkins;
        uint32_t level;
        
        // State
        uint64_t currentHealth;
        uint64_t maxHealth;
        uint64_t currentMana;
        uint64_t maxMana;
        bool isAlive;
        bool isInCombat;
        uint64_t targetEntityId;
        
        // Metadata
        std::string name;
        std::string title;
        std::string guildName;
        uint32_t guildRank;
        
        // AI (for NPCs/Monsters)
        uint32_t aiState;
        uint64_t aiTargetId;
        float aggroRadius;
        float leashRadius;
    };

    // Player-specific entity data
    struct PlayerEntity : public Entity {
        uint64_t playerId;
        std::string className;
        uint32_t classLevel;
        uint32_t experience;
        uint32_t experienceToNext;
        uint64_t currencyCredits;
        uint64_t currencyPremium;
        uint32_t guildId;
        bool isOnline;
        int64_t lastSeenTimestamp;
        uint32_t zoneInstanceId; // For phased content
    };

    // Quest Information
    struct QuestInfo {
        uint32_t questId;
        std::string name;
        std::string description;
        uint32_t level;
        uint32_t prerequisiteQuestId;
        uint64_t giverEntityId;
        uint64_t turnInEntityId;
        uint32_t rewardExperience;
        uint64_t rewardCredits;
        std::vector<std::pair<std::string, uint32_t>> rewardItems;
        std::vector<std::pair<uint64_t, uint32_t>> objectives; // entityId -> count
        bool isRepeatable;
        int64_t cooldownSeconds;
    };

    // Player's Quest Progress
    struct QuestProgress {
        uint32_t questId;
        uint32_t currentStep;
        std::unordered_map<uint32_t, uint32_t> objectiveProgress; // objectiveId -> count
        bool isCompleted;
        bool isFailed;
        int64_t startedAt;
        int64_t completedAt;
    };

    // Inventory Item
    struct InventoryItem {
        uint64_t itemId;
        uint32_t inventorySlot;
        uint32_t quantity;
        uint32_t durability;
        uint32_t maxDurability;
        bool isEquipped;
        bool isBound;
        int64_t acquiredAt;
        std::string metadata; // JSON for custom properties
    };

    // Chat Channel Types
    enum class ChatChannel : uint8_t {
        SAY = 0,
        YELL = 1,
        PARTY = 2,
        RAID = 3,
        GUILD = 4,
        TRADE = 5,
        GENERAL = 6,
        LFG = 7,
        WHISPER = 8,
        EMOTE = 9
    };

    // Chat Message
    struct ChatMessage {
        ChatChannel channel;
        uint64_t senderId;
        std::string senderName;
        std::string message;
        uint64_t targetPlayerId; // For whispers
        uint32_t zoneId; // For local channels
        int64_t timestamp;
        bool isGM;
    };

    /**
     * World Manager - Manages zones, entities, and world state
     */
    class WorldManager {
    public:
        static WorldManager& Instance();

        // Initialization
        bool Initialize();
        void Shutdown();

        // Zone Management
        bool LoadZone(uint32_t zoneId);
        bool UnloadZone(uint32_t zoneId);
        ZoneInfo* GetZoneInfo(uint32_t zoneId);
        std::vector<ZoneInfo> GetAllZones();
        
        // Zone Transitions
        bool RequestZoneChange(uint64_t playerId, uint32_t targetZoneId, 
                              float spawnX, float spawnY, float spawnZ);
        void HandleZoneChangeComplete(uint64_t playerId, uint32_t newZoneId);

        // Entity Management
        Entity* GetEntity(uint64_t entityId);
        PlayerEntity* GetPlayer(uint64_t playerId);
        std::vector<Entity*> GetEntitiesInZone(uint32_t zoneId);
        std::vector<Entity*> GetEntitiesInRange(float x, float y, float z, float radius, uint32_t zoneId);
        
        // Entity Spawning/Despawning
        uint64_t SpawnEntity(EntityType type, uint32_t zoneId, 
                            float x, float y, float z,
                            const std::string& modelAssetId);
        void DespawnEntity(uint64_t entityId);
        
        // Entity Updates
        void UpdateEntityPosition(uint64_t entityId, float x, float y, float z);
        void UpdateEntityRotation(uint64_t entityId, float rotX, float rotY, float rotZ);
        void UpdateEntityState(uint64_t entityId, const Entity& newState);

        // Player Management
        uint64_t AddPlayer(const std::string& playerName, uint32_t zoneId,
                          float x, float y, float z);
        void RemovePlayer(uint64_t playerId);
        void UpdatePlayer(uint64_t playerId, const PlayerEntity& playerData);

        // Combat System
        void StartCombat(uint64_t attackerId, uint64_t targetId);
        void EndCombat(uint64_t entityId);
        void DealDamage(uint64_t attackerId, uint64_t targetId, uint64_t damage);
        void Heal(uint64_t healerId, uint64_t targetId, uint64_t amount);

        // Query Functions
        std::vector<PlayerEntity*> GetPlayersInGuild(uint32_t guildId);
        std::vector<PlayerEntity*> GetPlayersInParty(uint64_t partyId);
        std::vector<Entity*> GetMonstersInZone(uint32_t zoneId);
        std::vector<Entity*> GetNPCsInZone(uint32_t zoneId);

    private:
        WorldManager() = default;
        ~WorldManager() = default;
        WorldManager(const WorldManager&) = delete;
        WorldManager& operator=(const WorldManager&) = delete;

        std::mutex m_zonesMutex;
        std::unordered_map<uint32_t, ZoneInfo> m_zones;
        
        std::mutex m_entitiesMutex;
        std::unordered_map<uint64_t, std::unique_ptr<Entity>> m_entities;
        std::unordered_map<uint32_t, std::vector<uint64_t>> m_zoneEntities; // zoneId -> entityIds
        
        std::mutex m_playersMutex;
        std::unordered_map<uint64_t, std::unique_ptr<PlayerEntity>> m_players;
        std::unordered_map<std::string, uint64_t> m_playerNameToId;
        
        std::atomic<uint64_t> m_nextEntityId;
    };

    /**
     * Quest Manager - Handles quest logic and tracking
     */
    class QuestManager {
    public:
        static QuestManager& Instance();

        // Initialization
        bool Initialize();
        void Shutdown();

        // Quest Definitions
        QuestInfo* GetQuestInfo(uint32_t questId);
        std::vector<QuestInfo> GetQuestsInZone(uint32_t zoneId);
        std::vector<QuestInfo> GetQuestsFromGiver(uint64_t giverEntityId);

        // Player Quest Progress
        bool AcceptQuest(uint64_t playerId, uint32_t questId);
        bool AbandonQuest(uint64_t playerId, uint32_t questId);
        bool CompleteQuest(uint64_t playerId, uint32_t questId);
        
        // Objective Tracking
        void UpdateObjective(uint64_t playerId, uint32_t questId, 
                            uint32_t objectiveId, uint32_t count);
        void TriggerEvent(uint64_t playerId, uint32_t eventId);

        // Progress Queries
        QuestProgress* GetQuestProgress(uint64_t playerId, uint32_t questId);
        std::vector<QuestProgress> GetActiveQuests(uint64_t playerId);
        std::vector<QuestProgress> GetCompletedQuests(uint64_t playerId);
        
        // Reward Distribution
        void DistributeRewards(uint64_t playerId, uint32_t questId);

    private:
        QuestManager() = default;
        ~QuestManager() = default;
        QuestManager(const QuestManager&) = delete;
        QuestManager& operator=(const QuestManager&) = delete;

        std::mutex m_questsMutex;
        std::unordered_map<uint32_t, QuestInfo> m_questDefinitions;
        std::unordered_map<uint64_t, std::unordered_map<uint32_t, QuestProgress>> m_playerQuests;
    };

    /**
     * Chat Manager - Handles all chat communication
     */
    class ChatManager {
    public:
        static ChatManager& Instance();

        // Initialization
        bool Initialize();
        void Shutdown();

        // Sending Messages
        bool SendMessage(ChatChannel channel, uint64_t senderId,
                        const std::string& senderName, const std::string& message,
                        uint64_t targetId = 0, uint32_t zoneId = 0);
        
        // Receiving Messages (callback registration)
        using OnChatMessageCallback = std::function<void(const ChatMessage&)>;
        void RegisterChatHandler(OnChatMessageCallback callback);

        // Channel Management
        bool JoinChannel(uint64_t playerId, ChatChannel channel);
        bool LeaveChannel(uint64_t playerId, ChatChannel channel);
        std::vector<ChatChannel> GetJoinedChannels(uint64_t playerId);

        // Chat Filters
        void AddProfanityFilter(const std::string& word);
        void RemoveProfanityFilter(const std::string& word);
        std::string FilterMessage(const std::string& message);

        // GM Functions
        void BroadcastGMMessage(const std::string& message);
        void MutePlayer(uint64_t playerId, int64_t durationSeconds);
        void UnmutePlayer(uint64_t playerId);

    private:
        ChatManager() = default;
        ~ChatManager() = default;
        ChatManager(const ChatManager&) = delete;
        ChatManager& operator=(const ChatManager&) = delete;

        std::mutex m_handlersMutex;
        std::vector<OnChatMessageCallback> m_chatHandlers;
        
        std::mutex m_filtersMutex;
        std::vector<std::string> m_profanityFilters;
        
        std::mutex m_mutedMutex;
        std::unordered_map<uint64_t, int64_t> m_mutedPlayers; // playerId -> unmuteTimestamp
    };

    // Utility Functions
    float CalculateDistance(float x1, float y1, float z1, float x2, float y2, float z2);
    float CalculateDistance2D(float x1, float y1, float x2, float y2);
    bool IsPointInZone(float x, float y, float z, const ZoneInfo& zone);

} // namespace World
} // namespace CrystalEchoes

#endif // CE_WORLD_H
