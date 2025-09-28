#!/bin/bash

echo "🚀 Starting Smart Code Assistant MCP Server"
echo "==========================================="
echo ""
echo "This server will run continuously and wait for Claude Desktop connections."
echo "Keep this terminal open while using Claude Desktop."
echo "Press Ctrl+C to stop the server when done."
echo ""
echo "Server Status: Starting..."
echo ""

# Navigate to the project directory
cd "$(dirname "$0")/src/SmartCodeAssistantMCP"

# Start the MCP server
dotnet run
