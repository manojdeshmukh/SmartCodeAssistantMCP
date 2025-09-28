# MCP Quick Reference

## Essential MCP Concepts

### MCP Server Components
```csharp
// Basic MCP server structure
public class MyMCPServer
{
    // Tools: Functions that can be called
    public List<Tool> Tools { get; set; }
    
    // Resources: Data sources that can be accessed
    public List<Resource> Resources { get; set; }
}
```

### Tool Definition Pattern
```csharp
[Tool("tool_name")]
public async Task<ToolResult> ToolMethod(ToolInput input)
{
    // 1. Validate input
    // 2. Process request
    // 3. Return result
}
```

### Common MCP Message Types
- `tools/list` - List available tools
- `tools/call` - Execute a tool
- `resources/list` - List available resources
- `resources/read` - Read resource content

### Error Handling
```csharp
try
{
    // Tool implementation
}
catch (Exception ex)
{
    return ToolResult.Error($"Tool failed: {ex.Message}");
}
```

## MCP Protocol Basics

### Request Format
```json
{
  "jsonrpc": "2.0",
  "id": "request-id",
  "method": "tools/call",
  "params": {
    "name": "tool_name",
    "arguments": { ... }
  }
}
```

### Response Format
```json
{
  "jsonrpc": "2.0",
  "id": "request-id",
  "result": {
    "content": "tool result",
    "isError": false
  }
}
```

## Development Workflow

1. **Design Tool Interface** - Define input/output schemas
2. **Implement Tool Logic** - Write the actual functionality
3. **Test Tool** - Verify it works correctly
4. **Document Tool** - Add usage examples
5. **Deploy Server** - Make it available to clients

## Best Practices

- Keep tools focused and single-purpose
- Validate all inputs thoroughly
- Provide clear error messages
- Use async/await for I/O operations
- Log important operations
- Handle edge cases gracefully
