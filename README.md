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

The Smart Code Assistant MCP Server is now **fully functional** with the following capabilities:

### ✅ Implemented Tools
- **`analyze_project`** - Comprehensive .NET solution/project analysis
- **`find_dependencies`** - NuGet dependency discovery and analysis
- **`check_code_quality`** - Basic code quality checks and recommendations
- **`generate_readme`** - Auto-generate comprehensive project README files
- **`extract_api_docs`** - Extract API documentation from XML comments
- **`generate_project_summary`** - Generate quick project overview and metrics

### ✅ Implemented Resources
- **`project://structure`** - Access to project file structure and organization
- **`project://dependencies`** - Access to project dependencies and package information

### ✅ Features
- Full .NET 9 support with modern C# features
- Roslyn-based code analysis
- Comprehensive project metrics
- Markdown documentation generation
- JSON-structured output for programmatic access
- Robust error handling and logging

## Getting Started

1. **Prerequisites**: .NET 9 SDK
2. **Clone and setup project**:
   ```bash
   git clone <repository-url>
   cd SmartCodeAssistantMCP
   ```
3. **Build the project**:
   ```bash
   cd src/SmartCodeAssistantMCP
   dotnet build
   ```
4. **Run the server**:
   ```bash
   dotnet run
   ```
5. **Test the server**:
   ```bash
   ./test-server.sh
   ```
6. **Connect with MCP client** and start using the tools!

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
