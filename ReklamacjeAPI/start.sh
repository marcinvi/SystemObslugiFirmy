#!/bin/bash

# ================================================
# Quick Start Script - Linux/Mac
# ================================================

echo "=================================="
echo "  Reklamacje API - Quick Start"
echo "=================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Check if .NET is installed
echo -e "${YELLOW}Checking .NET SDK...${NC}"
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK $DOTNET_VERSION found${NC}"
else
    echo -e "${RED}✗ .NET SDK not found!${NC}"
    echo -e "${YELLOW}  Download from: https://dotnet.microsoft.com/download${NC}"
    exit 1
fi

echo ""

# Restore packages
echo -e "${YELLOW}Restoring NuGet packages...${NC}"
dotnet restore
if [ $? -ne 0 ]; then
    echo -e "${RED}✗ Failed to restore packages${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Packages restored${NC}"

echo ""

# Build project
echo -e "${YELLOW}Building project...${NC}"
dotnet build
if [ $? -ne 0 ]; then
    echo -e "${RED}✗ Build failed${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Build successful${NC}"

echo ""
echo -e "${CYAN}==================================${NC}"
echo -e "${CYAN}  Configuration Checklist${NC}"
echo -e "${CYAN}==================================${NC}"
echo ""

# Check appsettings.json
if [ -f "appsettings.json" ]; then
    echo -e "${GREEN}✓ appsettings.json found${NC}"
    
    if grep -q "Password=your_password_here" appsettings.json 2>/dev/null; then
        echo -e "${RED}⚠ WARNING: Update connection string password!${NC}"
        echo -e "${YELLOW}  Edit: appsettings.Development.json${NC}"
    else
        echo -e "${GREEN}✓ Connection string configured${NC}"
    fi
    
    if grep -q "ChangeThis" appsettings.json 2>/dev/null; then
        echo -e "${RED}⚠ WARNING: Change JWT Secret!${NC}"
        echo -e "${YELLOW}  Edit: appsettings.json -> JwtSettings:Secret${NC}"
    else
        echo -e "${GREEN}✓ JWT Secret configured${NC}"
    fi
else
    echo -e "${RED}✗ appsettings.json not found${NC}"
fi

echo ""
echo -e "${CYAN}==================================${NC}"
echo -e "${CYAN}  Database Setup${NC}"
echo -e "${CYAN}==================================${NC}"
echo ""
echo -e "${YELLOW}Make sure you have:${NC}"
echo "1. MariaDB/MySQL running"
echo "2. ReklamacjeDB database created"
echo "3. First user created (see init_user.sql)"

echo ""
echo -e "${CYAN}==================================${NC}"
echo -e "${CYAN}  Starting API...${NC}"
echo -e "${CYAN}==================================${NC}"
echo ""

# Try to detect server IP for LAN access
SERVER_IP=""
if command -v hostname &> /dev/null; then
    SERVER_IP=$(hostname -I 2>/dev/null | awk '{print $1}')
fi
if [ -z "$SERVER_IP" ] && command -v ipconfig &> /dev/null; then
    SERVER_IP=$(ipconfig getifaddr en0 2>/dev/null)
fi
if [ -z "$SERVER_IP" ]; then
    SERVER_IP="<IP_SERWERA>"
fi

echo -e "${YELLOW}API will be available at:${NC}"
echo "  - HTTP:  http://localhost:5000 (or http://<LAN-IP>:5000 from mobile)"
echo "  - HTTPS: https://localhost:5001"
echo "  - Swagger: http://localhost:5000"
echo ""
echo -e "${YELLOW}For other devices on the network use:${NC}"
echo "  - HTTP:  http://${SERVER_IP}:5000"
echo "  - HTTPS: https://${SERVER_IP}:5001"
echo ""
echo -e "${YELLOW}Tip:${NC} If other devices cannot connect, set:"
echo "  export ASPNETCORE_URLS=\"http://0.0.0.0:5000;https://0.0.0.0:5001\""
echo ""
echo -e "${YELLOW}Press Ctrl+C to stop${NC}"
echo ""

# Run the API (bind to all interfaces for mobile access)
ASPNETCORE_URLS="http://0.0.0.0:5000;https://0.0.0.0:5001" dotnet run
