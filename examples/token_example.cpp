// Example: Using the FFX Token Engine and Marketplace

#include <iostream>
#include "ce_token.h"
#include "ce_marketplace.h"
#include "ce_shop_integration.h"

using namespace CrystalEchoes;

int main() {
    std::cout << "=== Crystal Echoes Crypto Marketplace Demo ===" << std::endl;
    
    // Initialize systems
    Crypto::FFXTokenEngine::Instance().Initialize();
    Marketplace::MarketplaceManager::Instance().Initialize();
    Shop::ShopIntegrationManager::Instance().Initialize();
    
    // Create wallets
    std::cout << "\n--- Creating Wallets ---" << std::endl;
    std::string player1Wallet = Crypto::FFXTokenEngine::Instance().CreateWallet();
    std::string player2Wallet = Crypto::FFXTokenEngine::Instance().CreateWallet();
    std::string creatorWallet = Crypto::FFXTokenEngine::Instance().CreateWallet();
    
    std::cout << "Player 1 Wallet: " << player1Wallet << std::endl;
    std::cout << "Player 2 Wallet: " << player2Wallet << std::endl;
    std::cout << "Creator Wallet: " << creatorWallet << std::endl;
    
    // Check balances
    std::cout << "\n--- Initial Balances ---" << std::endl;
    std::cout << "Player 1 Balance: " << Crypto::FFXTokenEngine::Instance().GetBalance(player1Wallet) << " satoshis" << std::endl;
    std::cout << "Creator Balance: " << Crypto::FFXTokenEngine::Instance().GetBalance(creatorWallet) << " satoshis" << std::endl;
    
    // Create an asset
    std::cout << "\n--- Creating Asset ---" << std::endl;
    Marketplace::Asset legendarySkin;
    legendarySkin.id = "SKIN-LEGENDARY-001";
    legendarySkin.name = "Legendary Warrior Skin";
    legendarySkin.description = "An ultra-rare 8K quality warrior skin with glowing effects";
    legendarySkin.type = Marketplace::AssetType::Skin;
    legendarySkin.quality = Marketplace::AssetQuality::Legendary;
    legendarySkin.creatorAddress = creatorWallet;
    legendarySkin.price = Crypto::FFXTokenEngine::Instance().TokensToSatoshis(100.0f); // 100 FFX-T
    legendarySkin.royaltyPercent = 5.0f; // 5% royalty on resales
    legendarySkin.isLimited = true;
    legendarySkin.maxSupply = 100;
    legendarySkin.currentSupply = 1;
    
    Marketplace::MarketplaceManager::Instance().RegisterAsset(legendarySkin);
    std::cout << "Registered: " << legendarySkin.name << std::endl;
    
    // Create a shop listing
    std::cout << "\n--- Creating Shop Listing ---" << std::endl;
    Marketplace::MarketplaceManager::Instance().CreateListing(
        legendarySkin.id, 
        creatorWallet, 
        Crypto::FFXTokenEngine::Instance().TokensToSatoshis(100.0f)
    );
    
    // Send some tokens to player 1 for testing
    std::cout << "\n--- Sending Tokens to Player 1 ---" << std::endl;
    Crypto::FFXTokenEngine::Instance().SendTransaction(creatorWallet, player1Wallet, 
        Crypto::FFXTokenEngine::Instance().TokensToSatoshis(500.0f));
    std::cout << "Sent 500 FFX-T to Player 1" << std::endl;
    std::cout << "Player 1 New Balance: " << Crypto::FFXTokenEngine::Instance().GetBalance(player1Wallet) << " satoshis" << std::endl;
    
    // Get active listings
    std::cout << "\n--- Active Listings ---" << std::endl;
    auto listings = Marketplace::MarketplaceManager::Instance().GetActiveListings();
    for (auto* listing : listings) {
        std::cout << "Listing: " << listing->asset.name 
                  << " | Price: " << Crypto::FFXTokenEngine::Instance().SatoshisToTokens(listing->price) 
                  << " FFX-T" << std::endl;
    }
    
    // Purchase the asset
    std::cout << "\n--- Purchasing Asset ---" << std::endl;
    if (!listings.empty()) {
        bool success = Marketplace::MarketplaceManager::Instance().BuyAsset(
            listings[0]->listingId, player1Wallet);
        
        if (success) {
            std::cout << "Purchase successful!" << std::endl;
            
            // Check inventory
            auto inventory = Marketplace::MarketplaceManager::Instance().GetUserInventory(player1Wallet);
            std::cout << "Player 1 Inventory:" << std::endl;
            for (auto* asset : inventory) {
                std::cout << "  - " << asset->name << " (" << asset->id << ")" << std::endl;
            }
            
            // Check creator earnings
            uint64_t earnings = Marketplace::MarketplaceManager::Instance().GetCreatorEarnings(creatorWallet);
            std::cout << "Creator Royalties Earned: " << Crypto::FFXTokenEngine::Instance().SatoshisToTokens(earnings) << " FFX-T" << std::endl;
        } else {
            std::cout << "Purchase failed!" << std::endl;
        }
    }
    
    // Blockchain info
    std::cout << "\n--- Blockchain Info ---" << std::endl;
    std::cout << "Chain Height: " << Crypto::FFXTokenEngine::Instance().GetChainHeight() << std::endl;
    std::cout << "Total Supply: " << Crypto::TOTAL_SUPPLY << " satoshis (" 
              << Crypto::FFXTokenEngine::Instance().SatoshisToTokens(Crypto::TOTAL_SUPPLY) << " FFX-T)" << std::endl;
    std::cout << "Chain Valid: " << (Crypto::FFXTokenEngine::Instance().ValidateChain() ? "Yes" : "No") << std::endl;
    
    std::cout << "\n=== Demo Complete ===" << std::endl;
    return 0;
}
