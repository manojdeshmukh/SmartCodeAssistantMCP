# Claude Desktop + Smart Code Assistant MCP Setup Guide

## 🎯 Current Status: READY TO USE!

Your Smart Code Assistant MCP server is now configured and running with Claude Desktop.

## 🚀 How to Use Your MCP Server

### Step 1: Start the MCP Server
The server is currently running in the background. If you need to restart it:

```bash
# Option 1: Use the convenience script
cd /Users/manojdeshmukh/SmartCodeAssistantMCP
./start-mcp-server.sh

# Option 2: Manual start
cd /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
dotnet run
```

**Important:** Keep the terminal with the server running open while using Claude Desktop!

### Step 2: Open Claude Desktop
1. Open Claude Desktop application
2. Start a new conversation
3. The MCP server should automatically connect

### Step 3: Verify Connection
Ask Claude:
```
Which MCP tools do you have?
```

You should see these tools listed:
- `analyze_project` - Analyze .NET solution structure
- `find_dependencies` - Discover NuGet dependencies  
- `check_code_quality` - Basic code quality checks
- `generate_readme` - Auto-generate project README
- `extract_api_docs` - Extract API documentation
- `generate_project_summary` - Generate project summary

## 🧪 Test Commands

Try these commands in Claude Desktop:

### Code Analysis
```
Analyze the SmartCodeAssistantMCP project structure
```

```
Check the code quality of my .NET project
```

```
Find all dependencies in my project
```

### Documentation Generation
```
Generate a comprehensive README for my .NET project
```

```
Extract API documentation from my project
```

```
Create a project summary with metrics
```

## 🔧 Configuration Details

**Configuration File:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

**Configuration Content:**
```json
{
  "mcpServers": {
    "smart-code-assistant": {
      "command": "dotnet",
      "args": ["run", "--project", "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"],
      "env": {
        "DOTNET_ENVIRONMENT": "Production"
      }
    }
  }
}
```

## 🚨 Troubleshooting

### If Claude doesn't see the tools:

1. **Check server is running:**
   ```bash
   ps aux | grep "dotnet run" | grep -v grep
   ```

2. **Restart the server:**
   ```bash
   cd /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
   dotnet run
   ```

3. **Restart Claude Desktop completely**

4. **Verify configuration file exists:**
   ```bash
   cat ~/Library/Application\ Support/Claude/claude_desktop_config.json
   ```

### If you get connection errors:

1. Make sure the server terminal is open and running
2. Check that the project path in the config is correct
3. Verify .NET is installed: `dotnet --version`

## 📊 What Your MCP Server Can Do

### Code Analysis Tools:
- **Project Structure Analysis** - Get comprehensive insights about your .NET projects
- **Dependency Discovery** - Find and analyze NuGet packages
- **Code Quality Checks** - Get recommendations and metrics

### Documentation Tools:
- **Auto-Generate README** - Create professional project documentation
- **Extract API Docs** - Pull documentation from XML comments
- **Project Summaries** - Generate quick overviews with metrics

### Resources:
- **Project Structure** - Access organized project information
- **Dependencies** - Get detailed dependency information

## 🎉 Success Indicators

You'll know it's working when:
- ✅ Claude lists the 6 tools when asked
- ✅ Claude can analyze your .NET projects
- ✅ Claude can generate documentation
- ✅ You see server logs in the terminal when Claude uses tools

## 🔄 Daily Usage

1. **Start the server** (keep terminal open)
2. **Open Claude Desktop**
3. **Ask Claude to analyze your .NET projects**
4. **Generate documentation automatically**
5. **Stop the server** when done (Ctrl+C in terminal)

Your Smart Code Assistant is now fully integrated with Claude Desktop! 🚀
