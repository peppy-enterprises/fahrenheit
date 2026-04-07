# Crystal Echoes MMORPG Module

## Overview

The Crystal Echoes MMORPG Module transforms the game into a fully-featured massively multiplayer online role-playing game with custom assets, marketplace integration, and persistent world support.

## Features

### 🌐 Networking System (`ce_net.h`)
- **Dual Protocol Support**: TCP for reliable communication, UDP for fast updates
- **MMO Protocol**: Custom packet types for player movement, actions, chat, and world state
- **Client-Server Architecture**: Full support for both client and server implementations
- **Serialization Utilities**: Efficient binary serialization for network packets
- **Connection Management**: Automatic reconnection, heartbeat, and latency monitoring

### 🎨 Asset Management (`ce_assets.h`)
- **Custom Asset Support**: Load user-created textures, models, animations, and scripts
- **Marketplace Integration**: 
  - Browse, search, and purchase assets from creators
  - Creator tools for uploading and selling assets
  - Rating and review system
  - Wishlist functionality
- **Asset Streaming**: Priority-based streaming for large open worlds
- **Skin System**: Character customization with equipable skins for multiple slots
- **Quality Levels**: Support for LOW to EXTREME (8K) quality assets
- **Cache Management**: LRU caching with configurable limits

### 🌍 World Management (`ce_world.h`)
- **Zone System**: Multiple zone types (open world, dungeons, raids, cities, instances)
- **Entity Management**: Players, NPCs, monsters, bosses, pets, mounts, vehicles
- **Combat System**: Real-time combat with damage, healing, and aggro mechanics
- **Quest System**: Full quest lifecycle with objectives and rewards
- **Chat System**: Multiple channels (say, yell, party, guild, trade, whisper)
- **Player Features**: Guilds, parties, trading, friends lists
- **Phasing Support**: Instance IDs for personalized content

### 🎮 MMO Client/Server (`ce_mmo.h`)
- **Client Controller**: Unified interface for all MMO functionality
- **Server Controller**: Host your own custom MMO servers
- **Event System**: Callbacks for login, logout, zone changes, and connection loss
- **Configuration**: Adjustable rates for XP, drops, and currency
- **Administration**: Kick, ban, and broadcast capabilities

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Crystal Echoes MMO                        │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   Network   │  │   Assets    │  │       World         │  │
│  │   Module    │  │   Module    │  │      Module         │  │
│  │             │  │             │  │                     │  │
│  │ - TCP/UDP   │  │ - Marketplace│  │ - Zones            │  │
│  │ - Packets   │  │ - Skins     │  │ - Entities         │  │
│  │ - Serialize │  │ - Streaming │  │ - Quests           │  │
│  │             │  │ - Caching   │  │ - Chat             │  │
│  └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘  │
│         │                │                     │             │
│         └────────────────┼─────────────────────┘             │
│                          │                                   │
│                 ┌────────▼────────┐                          │
│                 │   MMO Client    │                          │
│                 │   MMO Server    │                          │
│                 └─────────────────┘                          │
└─────────────────────────────────────────────────────────────┘
```

## Usage Examples

### Initializing the MMO Client

```cpp
#include "ce_mmo.h"

using namespace CrystalEchoes::MMO;

// Initialize the entire MMO system
InitializeMMOSystem();

// Get the client instance
auto& client = MMOClient::Instance();

// Connect to server
client.Initialize("mmo.crystalechoes.com", 7777);

// Set up event handlers
client.SetOnLoginSuccess([]() {
    printf("Successfully logged in!\n");
});

client.SetOnLoginFailure([](const std::string& reason) {
    printf("Login failed: %s\n", reason.c_str());
});

client.SetOnZoneChanged([](uint32_t zoneId) {
    printf("Entered zone: %u\n", zoneId);
});

// Login
if (client.Login("playername", "password")) {
    // Start game loop
    while (client.IsRunning()) {
        client.Update(deltaTime);
        // Render frame...
    }
}
```

### Browsing the Marketplace

```cpp
#include "ce_assets.h"

using namespace CrystalEchoes::Assets;

auto& marketplace = MarketplaceClient::Instance();

// Login to marketplace
marketplace.Initialize("https://api.crystalechoes.com", authToken);
marketplace.Login("username", "password");

// Search for dragon skins
marketplace.SearchAssets("dragon", AssetType::SKIN, 1, 20,
    [](bool success, const std::vector<MarketplaceItem>& items) {
        if (success) {
            for (const auto& item : items) {
                printf("Found: %s - %llu credits\n", 
                       item.name.c_str(), item.priceCredits);
            }
        }
    });

// Purchase an asset
marketplace.PurchaseAsset("item_12345",
    [](bool success, const std::string& txId) {
        if (success) {
            printf("Purchased! Transaction: %s\n", txId.c_str());
            
            // Download the asset
            marketplace.DownloadAsset("item_12345",
                [](const std::string& assetId, bool success) {
                    if (success) {
                        printf("Asset downloaded: %s\n", assetId.c_str());
                        
                        // Equip the skin
                        SkinManager::Instance().EquipSkin(assetId, "BODY");
                    }
                });
        }
    });
```

### Hosting a Custom Server

```cpp
#include "ce_mmo.h"

using namespace CrystalEchoes::MMO;

auto& server = MMOServer::Instance();

// Configure server
MMOServer::ServerConfig config{};
config.port = 7777;
config.maxPlayers = 1000;
config.worldName = "Crystal Echoes - Shiva";
config.databaseConnection = "postgresql://localhost/crystalechoes";
config.enablePvP = true;
config.enableTrading = true;
config.enableGuilds = true;
config.experienceRate = 2.0f; // 2x XP
config.dropRate = 1.5f;       // 1.5x drops
config.currencyRate = 1.0f;   // Normal currency

server.Initialize(config.port, "world_config.json");
server.UpdateConfig(config);

// Start the server
server.Start();

// Server runs until stopped
while (server.IsRunning()) {
    // Server update loop
    std::this_thread::sleep_for(std::chrono::milliseconds(16));
}

server.Shutdown();
```

### Creating Custom Assets

```cpp
#include "ce_assets.h"

using namespace CrystalEchoes::Assets;

auto& assetMgr = AssetManager::Instance();
assetMgr.Initialize("./cache");

// Register a custom texture
assetMgr.RegisterCustomAsset("my_custom_armor", 
                            "./mods/my_mod/armor.dds",
                            AssetType::TEXTURE_2D);

// Upload to marketplace as a creator
auto& marketplace = MarketplaceClient::Instance();
marketplace.UploadAsset(
    "./mods/my_mod/armor_pack.zip",
    "Dragon Scale Armor",
    "High-quality dragon scale armor skin with 8K textures",
    AssetType::SKIN,
    AssetQuality::EXTREME,
    5000, // 5000 credits
    {"dragon", "armor", "scale", "legendary"},
    [](bool success, const std::string& itemId) {
        if (success) {
            printf("Asset uploaded! Item ID: %s\n", itemId.c_str());
        }
    });
```

## Packet Types

| Type | ID | Description |
|------|-----|-------------|
| HANDSHAKE | 0x01 | Initial connection handshake |
| PLAYER_MOVE | 0x02 | Player position/rotation update |
| PLAYER_ACTION | 0x03 | Player abilities/actions |
| ENTITY_SPAWN | 0x04 | New entity entered range |
| ENTITY_UPDATE | 0x05 | Entity state changed |
| CHAT_MESSAGE | 0x06 | Chat communication |
| ASSET_REQUEST | 0x07 | Request asset download |
| ASSET_STREAM | 0x08 | Streamed asset data |
| MARKETPLACE_PURCHASE | 0x09 | Purchase transaction |
| WORLD_STATE_SYNC | 0x10 | Full world state sync |
| PLAYER_JOIN | 0x11 | Player joined zone |
| PLAYER_LEAVE | 0x12 | Player left zone |
| ZONE_CHANGE | 0x13 | Zone transition request |
| INVENTORY_UPDATE | 0x14 | Inventory changed |
| TRADE_REQUEST | 0x15 | Trade initiated |
| GUILD_MESSAGE | 0x16 | Guild chat |
| QUEST_UPDATE | 0x17 | Quest progress |
| COMBAT_EVENT | 0x18 | Combat action |
| HEARTBEAT | 0xFF | Connection keepalive |

## Performance Considerations

- **Interest Management**: Only receive updates for entities in your zone/range
- **Priority Streaming**: Critical assets load first
- **Delta Compression**: Only send changed data
- **LOD System**: Lower detail for distant entities
- **Prediction**: Client-side prediction for smooth movement
- **Interpolation**: Smooth entity movement between updates

## Requirements

- C++17 or later
- OpenSSL for secure connections
- SQLite/PostgreSQL for server database
- libcurl for HTTP marketplace API
- DirectX 11/12 or Vulkan for rendering

## Building

```bash
# Clone the repository
git clone https://github.com/CrystalEchoes/MMO.git

# Build with vcpkg dependencies
cd MMO
mkdir build && cd build
cmake .. -DCMAKE_TOOLCHAIN_FILE=[vcpkg]/scripts/buildsystems/vcpkg.cmake
cmake --build . --config Release
```

## License

MIT License - See LICENSE file for details

## Contributing

Contributions welcome! Please read CONTRIBUTING.md before submitting PRs.
