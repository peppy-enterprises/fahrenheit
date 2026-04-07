// ce_token.cpp
// Final Fantasy X Token (FFX-T) Engine Implementation

#include "ce_token.h"
#include <chrono>
#include <random>
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <cstring>

namespace CrystalEchoes {
namespace Crypto {

FFXTokenEngine& FFXTokenEngine::Instance() {
    static FFXTokenEngine instance;
    return instance;
}

void FFXTokenEngine::Initialize() {
    std::lock_guard<std::mutex> lock(m_mutex);
    if (m_blockchain.empty()) {
        AddGenesisBlock();
    }
}

std::string FFXTokenEngine::CreateWallet() {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    std::random_device rd;
    std::mt19937_64 gen(rd());
    std::uniform_int_distribution<uint64_t> dist;
    
    // Generate address (simulated)
    std::stringstream ss;
    ss << "FFX";
    for (int i = 0; i < 8; ++i) {
        ss << std::hex << std::setw(4) << std::setfill('0') << (dist(gen) % 0xFFFF);
    }
    std::string address = ss.str();
    
    // Generate keys (simulated)
    std::stringstream pk, pubk;
    for (int i = 0; i < 16; ++i) {
        pk << std::hex << std::setw(2) << std::setfill('0') << (dist(gen) % 0xFF);
        if (i < 8) {
            pubk << std::hex << std::setw(2) << std::setfill('0') << (dist(gen) % 0xFF);
        }
    }
    
    Wallet wallet;
    wallet.address = address;
    wallet.balance = 0;
    wallet.privateKey = pk.str();
    wallet.publicKey = pubk.str();
    wallet.nonce = 0;
    
    m_wallets[address] = wallet;
    
    // Give genesis balance to first wallet (for testing)
    if (m_wallets.size() == 1) {
        wallet.balance = TOTAL_SUPPLY / 2; // 50% in circulation initially
        m_wallets[address] = wallet;
    }
    
    return address;
}

Wallet* FFXTokenEngine::GetWallet(const std::string& address) {
    std::lock_guard<std::mutex> lock(m_mutex);
    auto it = m_wallets.find(address);
    if (it != m_wallets.end()) {
        return &it->second;
    }
    return nullptr;
}

uint64_t FFXTokenEngine::GetBalance(const std::string& address) {
    Wallet* wallet = GetWallet(address);
    if (wallet) {
        return wallet->balance;
    }
    return 0;
}

bool FFXTokenEngine::SendTransaction(const std::string& from, const std::string& to, uint64_t amount) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    auto fromIt = m_wallets.find(from);
    auto toIt = m_wallets.find(to);
    
    if (fromIt == m_wallets.end() || toIt == m_wallets.end()) {
        return false;
    }
    
    if (fromIt->second.balance < amount) {
        return false;
    }
    
    Transaction tx;
    tx.from = from;
    tx.to = to;
    tx.amount = amount;
    tx.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    tx.isMiningReward = false;
    
    // Create transaction data for hashing
    std::stringstream ss;
    ss << from << to << amount << tx.timestamp << fromIt->second.nonce;
    tx.hash = GenerateHash(ss.str());
    tx.signature = SignMessage(fromIt->second.privateKey, tx.hash);
    
    fromIt->second.nonce++;
    fromIt->second.balance -= amount;
    toIt->second.balance += amount;
    
    m_pendingTransactions.push_back(tx);
    
    return true;
}

bool FFXTokenEngine::VerifyTransaction(const Transaction& tx) {
    // Simplified verification (in production, would verify signature)
    if (tx.hash.empty() || tx.signature.empty()) {
        return false;
    }
    if (tx.from == tx.to) {
        return false;
    }
    if (tx.amount == 0) {
        return false;
    }
    return true;
}

void FFXTokenEngine::MinePendingTransactions(const std::string& minerAddress) {
    std::lock_guard<std::mutex> lock(m_mutex);
    
    if (m_pendingTransactions.empty()) {
        return;
    }
    
    auto walletIt = m_wallets.find(minerAddress);
    if (walletIt == m_wallets.end()) {
        return;
    }
    
    std::string previousHash = m_blockchain.empty() ? 
        std::string(64, '0') : m_blockchain.back().hash;
    
    // Add mining reward
    Transaction rewardTx;
    rewardTx.from = "SYSTEM";
    rewardTx.to = minerAddress;
    rewardTx.amount = 5000000000; // 50 FFX-T reward (in satoshis)
    rewardTx.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    rewardTx.isMiningReward = true;
    rewardTx.hash = GenerateHash(rewardTx.to + std::to_string(rewardTx.timestamp));
    
    std::vector<Transaction> blockTxs = m_pendingTransactions;
    blockTxs.push_back(rewardTx);
    
    Block newBlock = CreateNewBlock(blockTxs, previousHash);
    m_blockchain.push_back(newBlock);
    
    // Clear pending transactions
    m_pendingTransactions.clear();
}

uint64_t FFXTokenEngine::GetChainHeight() const {
    return m_blockchain.size();
}

bool FFXTokenEngine::ValidateChain() const {
    for (size_t i = 1; i < m_blockchain.size(); ++i) {
        if (!IsValidBlock(m_blockchain[i], m_blockchain[i-1])) {
            return false;
        }
    }
    return true;
}

bool FFXTokenEngine::ExecuteAssetPurchase(const std::string& buyer, const std::string& seller,
                                         const std::string& assetId, uint64_t price) {
    if (!SendTransaction(buyer, seller, price)) {
        return false;
    }
    
    // Record the purchase on-chain (simplified)
    Transaction purchaseTx;
    purchaseTx.from = buyer;
    purchaseTx.to = seller;
    purchaseTx.amount = price;
    purchaseTx.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    purchaseTx.hash = GenerateHash(assetId + buyer + seller + std::to_string(price));
    
    m_pendingTransactions.push_back(purchaseTx);
    
    return true;
}

std::string FFXTokenEngine::GenerateHash(const std::string& data) {
    // Simplified SHA-256 simulation (use proper crypto lib in production)
    std::hash<std::string> hasher;
    size_t hash = hasher(data);
    
    std::stringstream ss;
    ss << std::hex << std::setfill('0');
    for (int i = 0; i < 8; ++i) {
        ss << std::setw(8) << ((hash >> (i * 8)) & 0xFF);
    }
    return ss.str();
}

std::string FFXTokenEngine::SignMessage(const std::string& privateKey, const std::string& message) {
    // Simplified signature (use proper ECDSA in production)
    return GenerateHash(privateKey + message);
}

uint64_t FFXTokenEngine::TokensToSatoshis(float tokens) {
    return static_cast<uint64_t>(tokens * 100000000.0f);
}

float FFXTokenEngine::SatoshisToTokens(uint64_t satoshis) {
    return static_cast<float>(satoshis) / 100000000.0f;
}

void FFXTokenEngine::AddGenesisBlock() {
    Block genesis;
    genesis.index = 0;
    genesis.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    genesis.previousHash = std::string(64, '0');
    genesis.nonce = 0;
    genesis.difficulty = 4;
    
    Transaction genesisTx;
    genesisTx.from = "GENESIS";
    genesisTx.to = "SYSTEM";
    genesisTx.amount = TOTAL_SUPPLY;
    genesisTx.timestamp = genesis.timestamp;
    genesisTx.isMiningReward = true;
    genesisTx.hash = GenerateHash("genesis");
    
    genesis.transactions.push_back(genesisTx);
    genesis.hash = GenerateHash(
        std::to_string(genesis.index) + 
        std::to_string(genesis.timestamp) + 
        genesis.previousHash);
    
    m_blockchain.push_back(genesis);
}

Block FFXTokenEngine::CreateNewBlock(const std::vector<Transaction>& transactions, 
                                     const std::string& previousHash) {
    Block block;
    block.index = m_blockchain.size();
    block.timestamp = std::chrono::duration_cast<std::chrono::seconds>(
        std::chrono::system_clock::now().time_since_epoch()).count();
    block.previousHash = previousHash;
    block.transactions = transactions;
    block.difficulty = 4;
    block.nonce = 0;
    
    // Simple proof of work (simplified for game use)
    std::string target(block.difficulty, '0');
    while (true) {
        std::stringstream ss;
        ss << block.index << block.timestamp << previousHash << block.nonce;
        for (const auto& tx : transactions) {
            ss << tx.hash;
        }
        block.hash = GenerateHash(ss.str());
        
        if (block.hash.substr(0, block.difficulty) >= target) {
            break;
        }
        block.nonce++;
    }
    
    return block;
}

bool FFXTokenEngine::IsValidBlock(const Block& current, const Block& previous) const {
    if (current.index != previous.index + 1) {
        return false;
    }
    if (current.previousHash != previous.hash) {
        return false;
    }
    // Verify proof of work (simplified)
    return true;
}

} // namespace Crypto
} // namespace CrystalEchoes
