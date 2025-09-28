# Claude Desktop Integration Guide

## 🎯 **Quick Start for Claude Desktop**

This guide will help you integrate the Smart Code Assistant MCP Server with Claude Desktop on macOS.

## ✅ **Prerequisites**

- .NET 9 SDK installed
- Claude Desktop for Mac
- Terminal access

## 🚀 **Step 1: Clone and Build**

```bash
git clone https://github.com/manojdeshmukh/SmartCodeAssistantMCP.git
cd SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
dotnet build
```

## 🔧 **Step 2: Configure Claude Desktop**

Create the configuration file:

```bash
mkdir -p ~/Library/Application\ Support/Claude
```

Create `~/Library/Application Support/Claude/claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "smart-code-assistant": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": ["run", "--project", "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"]
    }
  }
}
```

**Note:** Update the path to match your local installation.

## 🚀 **Step 3: Start the MCP Server**

Keep this terminal open while using Claude Desktop:

```bash
cd SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
dotnet run
```

## 🔄 **Step 4: Restart Claude Desktop**

1. **Quit Claude Desktop completely** (Cmd+Q)
2. **Wait 5 seconds**
3. **Reopen Claude Desktop**
4. **Start a new conversation**

## 🧪 **Step 5: Test the Integration**

Ask Claude:

```
Which MCP tools do you have?
```

You should see 4 tools listed:
- `analyze_project`
- `find_dependencies`
- `check_code_quality`
- `generate_project_summary`

## 🎯 **Available Tools**

### 1. **Analyze Project**
```
Analyze the project at /path/to/your/project.csproj
```

**Returns:** Project structure, file counts, basic metrics

### 2. **Find Dependencies**
```
Find all dependencies in the project at /path/to/your/project.csproj
```

**Returns:** List of NuGet packages and project references

### 3. **Check Code Quality**
```
Check the code quality of the project at /path/to/your/project.csproj
```

**Returns:** Code metrics, file analysis, recommendations

### 4. **Generate Project Summary**
```
Generate a project summary for the project at /path/to/your/project.csproj
```

**Returns:** Project overview with key statistics

## 🔧 **Troubleshooting**

### Issue: Claude doesn't see the tools
**Solution:**
1. Make sure the MCP server is running
2. Restart Claude Desktop completely
3. Start a new conversation

### Issue: "Command not found" errors
**Solution:**
1. Check the dotnet path: `which dotnet`
2. Update the configuration with the correct path

### Issue: Server keeps shutting down
**Solution:**
1. Make sure you're in the correct directory
2. Check that the project builds: `dotnet build`

### Issue: Tools timeout or hang
**Solution:**
The server uses minimal implementations that should respond quickly. If issues persist:
1. Restart the server
2. Try with smaller projects first

## 📊 **Example Usage**

Once connected, you can ask Claude things like:

```
Analyze my .NET project structure and provide insights
```

```
Find all the NuGet packages used in my project
```

```
Check the code quality of my C# project and give recommendations
```

```
Generate a summary of my project with key metrics
```

## 🎉 **Success Indicators**

You'll know it's working when:
- ✅ Claude lists 4 tools when asked
- ✅ Claude can analyze projects without asking for file uploads
- ✅ You see connection logs in the server terminal
- ✅ Claude responds with structured JSON data

## 🔄 **Daily Usage**

1. **Start the server** (keep terminal open)
2. **Open Claude Desktop**
3. **Analyze your .NET projects**
4. **Stop the server** when done (Ctrl+C)

Your Smart Code Assistant is now ready to help with .NET development! 🚀
