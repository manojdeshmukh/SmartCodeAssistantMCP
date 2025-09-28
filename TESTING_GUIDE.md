# Smart Code Assistant MCP Server - Testing Guide

This guide will walk you through testing the MCP server step by step.

## Prerequisites

- .NET 9 SDK installed
- Terminal/Command Prompt access
- Basic understanding of MCP (Model Context Protocol)

## Step-by-Step Testing

### Step 1: Verify Project Structure

First, let's make sure all files are in place:

```bash
# Navigate to the project root
cd /Users/manojdeshmukh/SmartCodeAssistantMCP

# Check the project structure
ls -la
ls -la src/SmartCodeAssistantMCP/
ls -la src/SmartCodeAssistantMCP/Services/
ls -la src/SmartCodeAssistantMCP/Tools/
ls -la src/SmartCodeAssistantMCP/Resources/
```

### Step 2: Build the Project

```bash
# Navigate to the main project directory
cd src/SmartCodeAssistantMCP

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build
```

**Expected Output**: Build should succeed with "0 Warning(s), 0 Error(s)"

### Step 3: Test Server Startup

```bash
# Test if the server starts without errors
timeout 10s dotnet run 2>&1 | head -20
```

**Expected Output**: Server should start and show MCP protocol initialization messages

### Step 4: Run the Automated Test Script

```bash
# Go back to project root
cd ../..

# Make the test script executable (if not already)
chmod +x test-server.sh

# Run the test script
./test-server.sh
```

**Expected Output**: 
- ✅ Build successful!
- ✅ Server test completed!
- List of available tools and resources

### Step 5: Manual MCP Protocol Testing

Create a simple test client to verify MCP communication:

```bash
# Create a test directory
mkdir -p test-client
cd test-client

# Create a simple test script
cat > test-mcp.js << 'EOF'
const { spawn } = require('child_process');

// Start the MCP server
const server = spawn('dotnet', ['run'], {
  cwd: '../src/SmartCodeAssistantMCP',
  stdio: ['pipe', 'pipe', 'pipe']
});

// Test MCP initialization
const initMessage = {
  jsonrpc: "2.0",
  id: 1,
  method: "initialize",
  params: {
    protocolVersion: "2024-11-05",
    capabilities: {
      tools: {}
    },
    clientInfo: {
      name: "test-client",
      version: "1.0.0"
    }
  }
};

console.log('Sending initialization message...');
server.stdin.write(JSON.stringify(initMessage) + '\n');

// Listen for responses
server.stdout.on('data', (data) => {
  console.log('Server response:', data.toString());
});

server.stderr.on('data', (data) => {
  console.log('Server error:', data.toString());
});

// Clean up after 5 seconds
setTimeout(() => {
  server.kill();
  process.exit(0);
}, 5000);
EOF

# Run the test (requires Node.js)
node test-mcp.js
```

### Step 6: Test Individual Tools

Create a comprehensive tool testing script:

```bash
# Create tool test script
cat > test-tools.sh << 'EOF'
#!/bin/bash

echo "Testing Smart Code Assistant MCP Server Tools"
echo "=============================================="

# Test project path (use the current project)
PROJECT_PATH="../src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj"

echo "Using project: $PROJECT_PATH"
echo ""

# Test 1: Analyze Project
echo "🔍 Testing analyze_project tool..."
echo '{"jsonrpc":"2.0","id":1,"method":"tools/call","params":{"name":"analyze_project","arguments":{"projectPath":"'$PROJECT_PATH'"}}}' | \
timeout 30s dotnet run --project ../src/SmartCodeAssistantMCP 2>/dev/null | \
grep -o '"content":"[^"]*"' | head -1

echo ""

# Test 2: Find Dependencies
echo "📦 Testing find_dependencies tool..."
echo '{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"find_dependencies","arguments":{"projectPath":"'$PROJECT_PATH'"}}}' | \
timeout 30s dotnet run --project ../src/SmartCodeAssistantMCP 2>/dev/null | \
grep -o '"content":"[^"]*"' | head -1

echo ""

# Test 3: Check Code Quality
echo "✅ Testing check_code_quality tool..."
echo '{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"check_code_quality","arguments":{"projectPath":"'$PROJECT_PATH'"}}}' | \
timeout 30s dotnet run --project ../src/SmartCodeAssistantMCP 2>/dev/null | \
grep -o '"content":"[^"]*"' | head -1

echo ""

# Test 4: Generate README
echo "📝 Testing generate_readme tool..."
echo '{"jsonrpc":"2.0","id":4,"method":"tools/call","params":{"name":"generate_readme","arguments":{"projectPath":"'$PROJECT_PATH'"}}}' | \
timeout 30s dotnet run --project ../src/SmartCodeAssistantMCP 2>/dev/null | \
grep -o '"content":"[^"]*"' | head -1

echo ""
echo "✅ Tool testing completed!"
EOF

chmod +x test-tools.sh
./test-tools.sh
```

### Step 7: Test with Real MCP Client

If you have access to an MCP client (like Claude Desktop or another MCP-compatible client):

1. **Configure the client** to use your MCP server:
   ```json
   {
     "mcpServers": {
       "smart-code-assistant": {
         "command": "dotnet",
         "args": ["run", "--project", "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP"]
       }
     }
   }
   ```

2. **Test the tools** through the client interface:
   - Try analyzing a .NET project
   - Generate documentation
   - Check code quality

### Step 8: Performance Testing

```bash
# Test server performance with multiple requests
cat > performance-test.sh << 'EOF'
#!/bin/bash

echo "Performance Testing"
echo "=================="

PROJECT_PATH="../src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj"
START_TIME=$(date +%s)

# Run multiple analyze_project calls
for i in {1..5}; do
  echo "Test $i/5..."
  echo '{"jsonrpc":"2.0","id":'$i',"method":"tools/call","params":{"name":"analyze_project","arguments":{"projectPath":"'$PROJECT_PATH'"}}}' | \
  timeout 30s dotnet run --project ../src/SmartCodeAssistantMCP 2>/dev/null > /dev/null
done

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))

echo "Completed 5 tests in $DURATION seconds"
echo "Average time per test: $((DURATION / 5)) seconds"
EOF

chmod +x performance-test.sh
./performance-test.sh
```

## Expected Results

### Successful Test Indicators:
- ✅ Build completes without errors
- ✅ Server starts and responds to MCP protocol messages
- ✅ Tools return JSON responses with project analysis data
- ✅ Resources provide structured project information
- ✅ Performance is reasonable (< 10 seconds per analysis)

### Common Issues and Solutions:

1. **Build Errors**: Check .NET 9 SDK installation
2. **Package Restore Issues**: Run `dotnet restore` manually
3. **Permission Errors**: Ensure test scripts are executable
4. **Timeout Issues**: Increase timeout values for large projects

## Next Steps

After successful testing:
1. Connect to your preferred MCP client
2. Start using the tools for real .NET project analysis
3. Explore the generated documentation features
4. Consider implementing Phase 4 advanced features

## Troubleshooting

If you encounter issues:
1. Check the logs in the terminal output
2. Verify all dependencies are installed
3. Ensure the project path is correct
4. Check file permissions on scripts

For more help, refer to the MCP documentation or create an issue in the project repository.
