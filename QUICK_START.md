# Quick Start Guide - Testing Your MCP Server

## 🚀 Fastest Way to Test

Run this single command to test everything:

```bash
./step-by-step-test.sh
```

This interactive script will guide you through testing each component step by step.

## 🔧 Manual Testing (If You Prefer)

### 1. Build and Test
```bash
cd src/SmartCodeAssistantMCP
dotnet build
```

### 2. Quick Server Test
```bash
timeout 5s dotnet run
```

### 3. Run Automated Test
```bash
cd ../..
./test-server.sh
```

## 📋 What Each Test Does

| Test | Purpose | Expected Result |
|------|---------|----------------|
| **Project Structure** | Verify all files exist | ✅ All directories and files found |
| **.NET Installation** | Check SDK version | ✅ .NET 9 SDK detected |
| **Package Restore** | Download dependencies | ✅ Packages restored |
| **Build** | Compile the project | ✅ Build successful (0 errors) |
| **Server Startup** | Test MCP server initialization | ✅ Server starts without errors |
| **MCP Protocol** | Verify communication setup | ✅ Protocol messages ready |
| **Tools Listing** | Show available tools | ✅ 6 tools + 2 resources available |
| **Sample Analysis** | Test with current project | ✅ Tool responds (may timeout without client) |
| **Performance** | Check build speed | ✅ Builds in < 10 seconds |
| **Final Check** | Verify executable created | ✅ DLL/executable exists |

## 🎯 Expected Results

After running the tests, you should see:
- ✅ All build steps successful
- ✅ Server starts without errors
- ✅ All 6 tools available
- ✅ 2 resources available
- ✅ Performance is good

## 🚨 Troubleshooting

### Common Issues:

1. **"dotnet command not found"**
   - Install .NET 9 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

2. **"Build failed"**
   - Run `dotnet restore` first
   - Check you're in the right directory

3. **"Permission denied"**
   - Run `chmod +x step-by-step-test.sh`

4. **"Server timeout"**
   - This is normal - MCP servers wait for client connections

## 🎉 Success!

If all tests pass, your MCP server is ready to:
- Analyze .NET projects
- Generate documentation
- Check code quality
- Discover dependencies
- Provide project insights

## 🔗 Next Steps

1. **Connect to MCP Client**: Configure your MCP client to use this server
2. **Test Real Projects**: Try analyzing actual .NET solutions
3. **Generate Docs**: Use the documentation tools
4. **Explore Features**: Try all 6 available tools

## 📚 More Information

- **Detailed Guide**: See `TESTING_GUIDE.md` for comprehensive testing
- **Project Docs**: Check `docs/` folder for learning materials
- **MCP Spec**: Visit [modelcontextprotocol.io](https://modelcontextprotocol.io)

---

**Ready to test?** Run `./step-by-step-test.sh` now! 🚀
