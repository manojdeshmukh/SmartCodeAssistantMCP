#!/bin/bash

echo "🔧 Claude Desktop MCP Configuration Verification"
echo "================================================"
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}1. Checking Claude Desktop configuration file...${NC}"
CONFIG_FILE="$HOME/Library/Application Support/Claude/claude_desktop_config.json"

if [ -f "$CONFIG_FILE" ]; then
    echo -e "${GREEN}✅ Configuration file exists${NC}"
    echo "   Location: $CONFIG_FILE"
    echo ""
    echo "   Configuration content:"
    cat "$CONFIG_FILE" | sed 's/^/   /'
    echo ""
else
    echo -e "${RED}❌ Configuration file not found${NC}"
    echo "   Expected location: $CONFIG_FILE"
    exit 1
fi

echo -e "${BLUE}2. Verifying MCP server project...${NC}"
PROJECT_PATH="/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"

if [ -d "$PROJECT_PATH" ]; then
    echo -e "${GREEN}✅ Project directory exists${NC}"
    echo "   Location: $PROJECT_PATH"
else
    echo -e "${RED}❌ Project directory not found${NC}"
    echo "   Expected location: $PROJECT_PATH"
    exit 1
fi

if [ -f "$PROJECT_PATH/SmartCodeAssistantMCP.csproj" ]; then
    echo -e "${GREEN}✅ Project file exists${NC}"
else
    echo -e "${RED}❌ Project file not found${NC}"
    exit 1
fi

echo -e "${BLUE}3. Testing project build...${NC}"
cd "$PROJECT_PATH"
if dotnet build --verbosity quiet; then
    echo -e "${GREEN}✅ Project builds successfully${NC}"
else
    echo -e "${RED}❌ Project build failed${NC}"
    exit 1
fi

echo -e "${BLUE}4. Checking .NET installation...${NC}"
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✅ .NET found: version $DOTNET_VERSION${NC}"
else
    echo -e "${RED}❌ .NET not found${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}🎉 Configuration is ready!${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Restart Claude Desktop completely"
echo "2. Open Claude Desktop"
echo "3. Ask Claude: 'Which MCP tools do you have?'"
echo "4. You should see the Smart Code Assistant tools listed"
echo ""
echo -e "${YELLOW}Available tools you should see:${NC}"
echo "• analyze_project - Analyze .NET solution structure"
echo "• find_dependencies - Discover NuGet dependencies"
echo "• check_code_quality - Basic code quality checks"
echo "• generate_readme - Auto-generate project README"
echo "• extract_api_docs - Extract API documentation"
echo "• generate_project_summary - Generate project summary"
echo ""
echo -e "${YELLOW}Test commands to try in Claude:${NC}"
echo "• 'Analyze the SmartCodeAssistantMCP project'"
echo "• 'Generate a README for my .NET project'"
echo "• 'Check the code quality of my project'"
echo "• 'Find all dependencies in my project'"
echo ""
echo -e "${GREEN}Your Smart Code Assistant MCP server is ready to use! 🚀${NC}"
