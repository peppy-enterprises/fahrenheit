#pragma once

/**
 * Crystal Echoes - Network Core Module
 * Provides low-level networking for MMORPG functionality
 * Supports TCP (reliable) and UDP (unreliable/fast) channels
 */

#ifndef CE_NET_H
#define CE_NET_H

#ifdef _WIN32
    #include <winsock2.h>
    #include <ws2tcpip.h>
    #pragma comment(lib, "ws2_32.lib")
#else
    #include <sys/socket.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
    #define SOCKET int
    #define INVALID_SOCKET -1
#endif

#include <cstdint>
#include <functional>
#include <string>
#include <queue>
#include <mutex>
#include <thread>
#include <atomic>
#include <vector>
#include <unordered_map>

namespace CrystalEchoes {
namespace Net {

    // Packet Types for MMO Protocol
    enum class PacketType : uint8_t {
        HANDSHAKE = 0x01,
        PLAYER_MOVE = 0x02,
        PLAYER_ACTION = 0x03,
        ENTITY_SPAWN = 0x04,
        ENTITY_UPDATE = 0x05,
        CHAT_MESSAGE = 0x06,
        ASSET_REQUEST = 0x07,
        ASSET_STREAM = 0x08,
        MARKETPLACE_PURCHASE = 0x09,
        WORLD_STATE_SYNC = 0x10,
        PLAYER_JOIN = 0x11,
        PLAYER_LEAVE = 0x12,
        ZONE_CHANGE = 0x13,
        INVENTORY_UPDATE = 0x14,
        TRADE_REQUEST = 0x15,
        GUILD_MESSAGE = 0x16,
        QUEST_UPDATE = 0x17,
        COMBAT_EVENT = 0x18,
        HEARTBEAT = 0xFF
    };

    struct PacketHeader {
        uint32_t magic; // 'CEMO'
        PacketType type;
        uint16_t size;
        uint32_t sequence;
        uint64_t timestamp;
    };

    struct PlayerInfo {
        uint64_t playerId;
        std::string username;
        float x, y, z;
        float rotation;
        uint32_t zoneId;
        uint32_t level;
        std::string currentClass;
    };

    class NetworkClient {
    public:
        NetworkClient();
        ~NetworkClient();

        bool Connect(const std::string& ip, uint16_t port);
        void Disconnect();
        bool IsConnected() const;

        // Sending
        bool SendPacket(PacketType type, const void* data, size_t size);
        
        // Receiving (Non-blocking)
        bool PollPacket(PacketType& outType, void* buffer, size_t bufferSize, size_t& outSize);

        // Callbacks
        using OnConnectCallback = std::function<void()>;
        using OnDisconnectCallback = std::function<void()>;
        using OnPacketCallback = std::function<void(PacketType, const void*, size_t)>;

        void SetOnConnect(OnConnectCallback cb);
        void SetOnDisconnect(OnDisconnectCallback cb);
        void SetOnPacket(OnPacketCallback cb);

        // Background thread management
        void StartNetworkThread();
        void StopNetworkThread();

        // Get assigned player ID after handshake
        uint64_t GetPlayerId() const { return m_playerId; }

    private:
        SOCKET m_socketTcp;
        SOCKET m_socketUdp;
        sockaddr_in m_serverAddr;
        
        std::atomic<bool> m_running;
        std::thread m_networkThread;
        
        std::mutex m_sendMutex;
        std::mutex m_recvMutex;

        uint64_t m_playerId;
        uint32_t m_sequence;

        OnConnectCallback m_onConnect;
        OnDisconnectCallback m_onDisconnect;
        OnPacketCallback m_onPacket;

        void NetworkLoop();
        bool ReceiveLoop();
        void ProcessHandshake(const void* data, size_t size);
    };

    class NetworkServer {
    public:
        NetworkServer();
        ~NetworkServer();

        bool Start(uint16_t port);
        void Stop();
        
        // Broadcast to all connected clients
        void Broadcast(PacketType type, const void* data, size_t size);
        
        // Send to specific client ID
        void SendTo(uint64_t playerId, PacketType type, const void* data, size_t size);

        // Get connected player count
        size_t GetPlayerCount() const;

        // Kick a player
        void KickPlayer(uint64_t playerId);

    private:
        SOCKET m_listenSocketTcp;
        SOCKET m_listenSocketUdp;
        std::atomic<bool> m_running;
        std::thread m_acceptThread;
        
        std::mutex m_clientsMutex;
        std::unordered_map<uint64_t, SOCKET> m_clients;
        std::unordered_map<SOCKET, uint64_t> m_socketToPlayer;
        
        uint64_t m_nextPlayerId;

        void AcceptLoop();
        void HandleClient(SOCKET clientSocket);
    };

    // Utility for serialization
    class Serializer {
    public:
        static void WriteFloat(uint8_t* buffer, size_t& offset, float val);
        static void WriteDouble(uint8_t* buffer, size_t& offset, double val);
        static void WriteInt(uint8_t* buffer, size_t& offset, int32_t val);
        static void WriteUInt(uint8_t* buffer, size_t& offset, uint32_t val);
        static void WriteLong(uint8_t* buffer, size_t& offset, int64_t val);
        static void WriteULong(uint8_t* buffer, size_t& offset, uint64_t val);
        static void WriteString(uint8_t* buffer, size_t& offset, const std::string& str);
        static void WriteBool(uint8_t* buffer, size_t& offset, bool val);
        static void WriteVector3(uint8_t* buffer, size_t& offset, float x, float y, float z);
        
        static float ReadFloat(const uint8_t* buffer, size_t& offset);
        static double ReadDouble(const uint8_t* buffer, size_t& offset);
        static int32_t ReadInt(const uint8_t* buffer, size_t& offset);
        static uint32_t ReadUInt(const uint8_t* buffer, size_t& offset);
        static int64_t ReadLong(const uint8_t* buffer, size_t& offset);
        static uint64_t ReadULong(const uint8_t* buffer, size_t& offset);
        static std::string ReadString(const uint8_t* buffer, size_t& offset);
        static bool ReadBool(const uint8_t* buffer, size_t& offset);
        static void ReadVector3(const uint8_t* buffer, size_t& offset, float& x, float& y, float& z);
    };

    // Connection state enumeration
    enum class ConnectionState {
        Disconnected,
        Connecting,
        Connected,
        Authenticating,
        Authenticated,
        InGame
    };

} // namespace Net
} // namespace CrystalEchoes

#endif // CE_NET_H
