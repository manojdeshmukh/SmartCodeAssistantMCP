# Fix MCP Connection Issue

## 🚨 Problem: Claude Desktop Not Connecting to MCP Server

Claude is searching the web instead of using your local MCP server. Here's how to fix it:

## ✅ Step 1: Updated Configuration

I've updated your configuration file with the full path to dotnet:

```json
{
  "mcpServers": {
    "smart-code-assistant": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": ["run", "--project", "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"],
      "env": {
        "DOTNET_ENVIRONMENT": "Production"
      }
    }
  }
}
```

## 🔧 Step 2: Complete Restart Process

1. **Stop the current MCP server** (Ctrl+C in the server terminal)

2. **Quit Claude Desktop completely** (Cmd+Q)

3. **Wait 10 seconds**

4. **Start the MCP server again**:
   ```bash
   cd /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
   dotnet run
   ```

5. **Open Claude Desktop**

6. **Start a NEW conversation** (don't use the old one)

## 🧪 Step 3: Test the Connection

Ask Claude this exact question:
```
Which MCP tools do you have?
```

**Expected Response:** List of 6 tools including `analyze_project`, `find_dependencies`, etc.

**If Claude still searches the web:** Continue to Step 4.

## 🔍 Step 4: Alternative Configuration

If the above doesn't work, try this alternative configuration:

```json
{
  "mcpServers": {
    "smart-code-assistant": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": ["run", "--project", "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"],
      "cwd": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"
    }
  }
}
```

## 🚨 Step 5: Check Claude Desktop Version

Make sure you're using a recent version of Claude Desktop that supports MCP. Check:
- Claude Desktop version should be recent (2024)
- MCP support was added in recent versions

## 🔧 Step 6: Manual Test

Test if the server responds to MCP messages:

```bash
cd /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{"tools":{}},"clientInfo":{"name":"test","version":"1.0"}}}' | dotnet run
```

## 📋 Step 7: Verify File Permissions

Make sure the configuration file has correct permissions:

```bash
ls -la ~/Library/Application\ Support/Claude/claude_desktop_config.json
```

## 🎯 Success Indicators

You'll know it's working when:
- ✅ Claude lists 6 tools when asked "Which MCP tools do you have?"
- ✅ Claude doesn't search the web for project analysis
- ✅ You see connection logs in the server terminal
- ✅ Claude can analyze projects directly

## 🚨 Common Issues

### Issue: "Command not found"
**Solution:** Use full path to dotnet (already fixed)

### Issue: "Project not found"
**Solution:** Verify the project path is correct and absolute

### Issue: Claude still searches the web
**Solution:** 
1. Restart both server and Claude Desktop
2. Start a NEW conversation in Claude Desktop
3. Check Claude Desktop version

### Issue: No connection logs in server terminal
**Solution:** Claude Desktop isn't connecting - check configuration file

## 🔄 Complete Restart Sequence

1. **Stop MCP server** (Ctrl+C)
2. **Quit Claude Desktop** (Cmd+Q)
3. **Wait 10 seconds**
4. **Start MCP server** (`dotnet run`)
5. **Open Claude Desktop**
6. **Start NEW conversation**
7. **Ask**: "Which MCP tools do you have?"

## 📞 If Still Not Working

Try this minimal test configuration:

```json
{
  "mcpServers": {
    "test": {
      "command": "/usr/local/share/dotnet/dotnet",
      "args": ["run", "--project", "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"]
    }
  }
}
```

The key is to restart everything and start a fresh conversation in Claude Desktop.
