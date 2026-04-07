# Crystal Echoes Crypto Marketplace Module
cmake_minimum_required(VERSION 3.15)

project(CrystalEchoesCrypto VERSION 1.0.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Source files
set(CRYPTO_SOURCES
    src/ce_token.cpp
    src/ce_marketplace.cpp
    src/ce_shop_integration.cpp
)

# Header files
set(CRYPTO_HEADERS
    inc/ce_token.h
    inc/ce_marketplace.h
    inc/ce_shop_integration.h
)

# Create static library
add_library(CrystalEchoesCrypto STATIC ${CRYPTO_SOURCES} ${CRYPTO_HEADERS})

# Include directories
target_include_directories(CrystalEchoesCrypto PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/inc
)

# Compile options
if(MSVC)
    target_compile_options(CrystalEchoesCrypto PRIVATE /W4 /WX-)
else()
    target_compile_options(CrystalEchoesCrypto PRIVATE -Wall -Wextra -Wpedantic)
endif()

# Thread support
find_package(Threads REQUIRED)
target_link_libraries(CrystalEchoesCrypto PUBLIC Threads::Threads)

# Export symbols for Windows
if(WIN32)
    target_compile_definitions(CrystalEchoesCrypto PRIVATE CRYSTAL_ECHOES_CRYPTO_EXPORTS)
endif()

# Installation
install(TARGETS CrystalEchoesCrypto
    ARCHIVE DESTINATION lib
    LIBRARY DESTINATION lib
    RUNTIME DESTINATION bin
)

install(FILES ${CRYPTO_HEADERS}
    DESTINATION include/CrystalEchoes
)

# Example usage
option(BUILD_EXAMPLES "Build example programs" OFF)
if(BUILD_EXAMPLES)
    add_executable(token_example examples/token_example.cpp)
    target_link_libraries(token_example CrystalEchoesCrypto)
    
    add_executable(marketplace_example examples/marketplace_example.cpp)
    target_link_libraries(marketplace_example CrystalEchoesCrypto)
endif()
