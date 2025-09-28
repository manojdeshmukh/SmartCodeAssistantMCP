# Smart Code Assistant MCP Server

A Model Context Protocol (MCP) server built with .NET that provides intelligent code analysis and development assistance tools.

## Learning Objectives

This project is designed to teach MCP concepts through practical implementation:

- **MCP Server Architecture**: Understanding how MCP servers work and communicate
- **Tool Definition**: Creating and implementing MCP tools
- **Resource Management**: Managing and serving resources through MCP
- **Protocol Implementation**: Understanding the MCP protocol specification
- **Real-world Integration**: Applying MCP in development workflows

## Project Structure

```
SmartCodeAssistantMCP/
├── src/
│   ├── SmartCodeAssistantMCP/          # Main MCP server project
│   │   ├── Tools/                      # MCP tool implementations
│   │   ├── Resources/                  # MCP resource definitions
│   │   ├── Services/                   # Business logic services
│   │   └── Program.cs                  # MCP server entry point
│   └── SmartCodeAssistantMCP.Tests/   # Unit tests
├── docs/                               # Documentation
├── examples/                           # Usage examples
└── README.md
```

## MCP Tools Implementation Plan

### Phase 1: Basic Setup ✅
- [x] Project structure setup
- [x] Basic MCP server skeleton
- [x] First simple tool implementation

### Phase 2: Code Analysis Tools ✅
- [x] `analyze_project` - Analyze .NET solution structure
- [x] `find_dependencies` - Discover NuGet dependencies
- [x] `check_code_quality` - Basic code quality checks

### Phase 3: Documentation Tools ✅
- [x] `generate_readme` - Auto-generate project README
- [x] `extract_api_docs` - Extract API documentation

### Phase 4: Advanced Features
- [ ] `suggest_improvements` - Code improvement suggestions
- [ ] `validate_config` - Configuration validation
- [ ] `check_security` - Security vulnerability scanning

## Current Implementation Status

The Smart Code Assistant MCP Server is **fully functional** and ready to use with Claude Desktop!

### ✅ Available Tools
- **`analyze_project`** - Analyze .NET project structure and metrics
- **`find_dependencies`** - Discover and analyze NuGet dependencies
- **`check_code_quality`** - Basic code quality checks and recommendations
- **`generate_project_summary`** - Generate project overview with key metrics

### ✅ Features
- Fast, reliable analysis without complex dependencies
- Claude Desktop integration ready
- JSON-structured output for easy consumption
- Comprehensive error handling and logging

## 🚀 Quick Start

1. **Clone the repository**:
   ```bash
   git clone https://github.com/manojdeshmukh/SmartCodeAssistantMCP.git
   cd SmartCodeAssistantMCP
   ```

2. **Build the project**:
   ```bash
   cd src/SmartCodeAssistantMCP
   dotnet build
   ```

3. **Start the server**:
   ```bash
   dotnet run
   ```

4. **Configure Claude Desktop** - See [Claude Desktop Integration Guide](./docs/claude-desktop-integration.md)

## 📚 Documentation

- **[Claude Desktop Integration](./docs/claude-desktop-integration.md)** - Complete setup guide
- **[Learning Notes](./docs/learning-notes.md)** - MCP concepts and learning
- **[Progress Tracking](./docs/progress.md)** - Development progress

## Learning Resources

- [MCP Specification](https://modelcontextprotocol.io/docs)
- [MCP .NET SDK Documentation](https://github.com/modelcontextprotocol/dotnet-sdk)
- [Project Learning Notes](./docs/learning-notes.md)
- [Implementation Progress](./docs/progress.md)

## Tech Stack

- **Language**: C# (.NET 8)
- **MCP Framework**: Official MCP .NET SDK
- **Code Analysis**: Roslyn Analyzers
- **Documentation**: Markdig
- **Testing**: xUnit
