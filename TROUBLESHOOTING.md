# Claude Desktop MCP Connection Troubleshooting

## 🚨 Issue: Claude Desktop Not Recognizing MCP Tools

If Claude responds with "I don't see any files" instead of listing MCP tools, follow these steps:

## Step 1: Verify Server is Running

Check if the MCP server is running:
```bash
ps aux | grep "dotnet run" | grep -v grep
```

If not running, start it:
```bash
cd /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
dotnet run
```

**Keep this terminal open!**

## Step 2: Restart Claude Desktop Completely

1. **Quit Claude Desktop** (Cmd+Q or right-click dock icon → Quit)
2. **Wait 5 seconds**
3. **Reopen Claude Desktop**
4. **Start a new conversation**

## Step 3: Test MCP Connection

Ask Claude this exact question:
```
Which MCP tools do you have?
```

**Expected Response:** List of 6 tools including `analyze_project`, `find_dependencies`, etc.

**If you still get "I don't see any files":** Continue to Step 4.

## Step 4: Check Configuration File

Verify the configuration file exists and is correct:
```bash
cat ~/Library/Application\ Support/Claude/claude_desktop_config.json
```

Should show:
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

## Step 5: Alternative Configuration Method

If the above doesn't work, try this alternative configuration:

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

## Step 6: Check .NET Path

Find your .NET installation:
```bash
which dotnet
```

Update the configuration with the full path if needed.

## Step 7: Test Server Manually

Test if the server responds to MCP messages:
```bash
cd /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{"tools":{}},"clientInfo":{"name":"test","version":"1.0"}}}' | dotnet run
```

## Step 8: Check Claude Desktop Logs

Look for error messages in:
- Claude Desktop console (if available)
- System Console app → search for "Claude"
- Terminal where server is running

## Step 9: Alternative: Use Different MCP Client

If Claude Desktop continues to have issues, you can test with other MCP clients or use the server directly.

## Common Issues & Solutions

### Issue: "Command not found"
**Solution:** Use full path to dotnet in configuration

### Issue: "Project not found"
**Solution:** Verify the project path is correct and absolute

### Issue: "Permission denied"
**Solution:** Check file permissions on the project directory

### Issue: Server starts but Claude doesn't connect
**Solution:** Restart Claude Desktop completely

## Success Indicators

You'll know it's working when:
- ✅ Claude lists 6 tools when asked "Which MCP tools do you have?"
- ✅ Claude can analyze projects without asking for file uploads
- ✅ You see connection logs in the server terminal
- ✅ Claude responds with project analysis data

## Quick Test Commands

Once connected, try:
```
Which MCP tools do you have?
```

```
Analyze the SmartCodeAssistantMCP project
```

```
Generate a README for my .NET project
```

## Still Having Issues?

1. Check the server terminal for error messages
2. Verify all file paths are absolute
3. Ensure .NET 9 is properly installed
4. Try restarting both the server and Claude Desktop
5. Check Claude Desktop version compatibility
