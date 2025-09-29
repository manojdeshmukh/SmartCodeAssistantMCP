# Smart Code Assistant MCP Server - Demo Script

## 🎯 **Demo Overview**
**Duration**: 15-20 minutes  
**Audience**: Development Team  
**Format**: Live demonstration with Claude Desktop  

---

## 📋 **Pre-Demo Checklist**

### **Technical Setup**
- [ ] MCP server is running (`dotnet run` in project directory)
- [ ] Claude Desktop is open and configured
- [ ] Sample .NET project is ready for analysis
- [ ] Terminal/console is visible for server logs
- [ ] Internet connection is stable

### **Demo Environment**
- [ ] Use a real project from the team (if available)
- [ ] Have backup sample project ready
- [ ] Prepare 2-3 different project types (console, web, library)
- [ ] Ensure projects have dependencies to show

### **Presentation Setup**
- [ ] Screen sharing is working
- [ ] Claude Desktop window is visible
- [ ] Terminal window is visible
- [ ] Have presentation slides ready as backup

---

## 🚀 **Demo Flow**

### **1. Introduction (2 minutes)**

**Script:**
> "Good morning everyone! Today I'm going to demonstrate our Smart Code Assistant MCP Server. This is an AI-powered tool that integrates with Claude Desktop to provide intelligent code analysis for .NET projects.
> 
> The MCP server is currently running in the background, and I have Claude Desktop open. Let me show you how this works."

**Actions:**
- Show the running MCP server in terminal
- Open Claude Desktop
- Explain what MCP is briefly

---

### **2. Tool Discovery (3 minutes)**

**Script:**
> "First, let's see what tools are available. I'll ask Claude what MCP tools it has access to."

**Demo Steps:**
1. In Claude Desktop, type: `Which MCP tools do you have available?`
2. Wait for Claude's response
3. Show the 4 available tools:
   - `analyze_project`
   - `find_dependencies` 
   - `check_code_quality`
   - `generate_project_summary`

**Expected Output:**
```
I have access to 4 MCP tools from the Smart Code Assistant server:

1. **analyze_project** - Analyzes a .NET project and returns basic information about its structure
2. **find_dependencies** - Finds basic dependencies in a .NET project file
3. **check_code_quality** - Performs basic code quality checks on a .NET project
4. **generate_project_summary** - Generates a basic project summary with key metrics
```

**Key Points to Highlight:**
- Claude automatically discovered the tools
- No manual configuration needed
- Tools are described with clear purposes

---

### **3. Project Analysis Demo (5 minutes)**

**Script:**
> "Now let's analyze a real .NET project. I'll use our own SmartCodeAssistantMCP project as an example."

**Demo Steps:**
1. In Claude Desktop, type: `Analyze the project at /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj`
2. Wait for Claude to use the `analyze_project` tool
3. Show the structured JSON response
4. Explain the key metrics

**Expected Output:**
```json
{
  "ProjectPath": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj",
  "ProjectName": "SmartCodeAssistantMCP",
  "AnalysisTimestamp": "2024-01-15T10:30:00Z",
  "Status": "Success",
  "Message": "Basic project analysis completed",
  "BasicInfo": {
    "FileExists": true,
    "ProjectDirectory": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP",
    "FileSize": 1234
  }
}
```

**Key Points to Highlight:**
- Structured JSON output
- Real-time analysis
- Project metadata extraction
- Error handling (if file doesn't exist)

---

### **4. Dependency Analysis Demo (4 minutes)**

**Script:**
> "Let's see what dependencies this project has. This is particularly useful for understanding project complexity and managing NuGet packages."

**Demo Steps:**
1. In Claude Desktop, type: `Find all dependencies in the project at /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj`
2. Wait for Claude to use the `find_dependencies` tool
3. Show the dependency list
4. Explain the value for dependency management

**Expected Output:**
```json
{
  "ProjectPath": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj",
  "AnalysisTimestamp": "2024-01-15T10:32:00Z",
  "Dependencies": {
    "Count": 3,
    "Packages": [
      "Microsoft.Extensions.DependencyInjection",
      "Microsoft.Extensions.Hosting",
      "Microsoft.Extensions.Logging"
    ]
  },
  "Message": "Found 3 package references"
}
```

**Key Points to Highlight:**
- Automatic dependency discovery
- Package reference extraction
- Useful for security audits
- Helps with dependency management

---

### **5. Code Quality Demo (4 minutes)**

**Script:**
> "Now let's check the code quality of our project. This tool provides insights into code structure and recommendations for improvement."

**Demo Steps:**
1. In Claude Desktop, type: `Check the code quality of the project at /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj`
2. Wait for Claude to use the `check_code_quality` tool
3. Show the quality metrics and recommendations
4. Explain how this helps with code reviews

**Expected Output:**
```json
{
  "ProjectPath": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj",
  "AnalysisTimestamp": "2024-01-15T10:34:00Z",
  "QualityMetrics": {
    "CSharpFilesFound": 8,
    "FilesAnalyzed": 8,
    "TotalLinesOfCode": 1250,
    "AverageLinesPerFile": 156
  },
  "Recommendations": [
    "File organization looks reasonable",
    "Code size is manageable"
  ],
  "Message": "Basic code quality analysis completed"
}
```

**Key Points to Highlight:**
- Lines of code analysis
- File organization insights
- Actionable recommendations
- Quality metrics for tracking

---

### **6. Project Summary Demo (3 minutes)**

**Script:**
> "Finally, let's generate a comprehensive project summary. This gives us a quick overview of the entire project."

**Demo Steps:**
1. In Claude Desktop, type: `Generate a project summary for the project at /Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj`
2. Wait for Claude to use the `generate_project_summary` tool
3. Show the summary output
4. Explain the value for project documentation

**Expected Output:**
```json
{
  "ProjectName": "SmartCodeAssistantMCP",
  "ProjectPath": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP/SmartCodeAssistantMCP.csproj",
  "AnalysisTimestamp": "2024-01-15T10:36:00Z",
  "QuickStats": {
    "ProjectExists": true,
    "ProjectDirectory": "/Users/manojdeshmukh/SmartCodeAssistantMCP/src/SmartCodeAssistantMCP",
    "ProjectFileSize": 1234
  },
  "Summary": "Project 'SmartCodeAssistantMCP' analysis completed successfully",
  "Status": "Ready for development"
}
```

**Key Points to Highlight:**
- Quick project overview
- Status indicators
- Useful for project documentation
- Easy to understand format

---

## 🎯 **Demo Variations**

### **Alternative Demo 1: Error Handling**
**Script:**
> "Let me show you how the tool handles errors gracefully."

**Demo Steps:**
1. Use a non-existent project path
2. Show the error response
3. Explain error handling benefits

**Expected Output:**
```
Error: Project file not found
```

### **Alternative Demo 2: Different Project Types**
**Script:**
> "Let's try analyzing a different type of project to show the versatility."

**Demo Steps:**
1. Use a web project or console application
2. Show different metrics and recommendations
3. Highlight adaptability

### **Alternative Demo 3: Large Project Analysis**
**Script:**
> "For larger projects, the analysis provides more detailed insights."

**Demo Steps:**
1. Use a larger, more complex project
2. Show more detailed recommendations
3. Highlight scalability

---

## 🛠️ **Troubleshooting Guide**

### **Common Issues & Solutions**

#### **Issue: Claude doesn't see the tools**
**Symptoms:**
- Claude responds with "I don't have access to any MCP tools"
- No tools listed in response

**Solutions:**
1. Check if MCP server is running
2. Restart Claude Desktop completely
3. Verify configuration file is correct
4. Check server logs for errors

#### **Issue: Tools timeout or hang**
**Symptoms:**
- Claude shows "Tool call in progress" for a long time
- No response from tools

**Solutions:**
1. Check server logs for errors
2. Restart the MCP server
3. Try with a smaller project first
4. Check file permissions

#### **Issue: "Command not found" errors**
**Symptoms:**
- Server fails to start
- Configuration errors

**Solutions:**
1. Verify dotnet is installed and in PATH
2. Check project path in configuration
3. Ensure project builds successfully
4. Update configuration with correct paths

#### **Issue: Permission errors**
**Symptoms:**
- "Access denied" errors
- File reading failures

**Solutions:**
1. Check file permissions
2. Run from appropriate directory
3. Ensure project files are accessible
4. Check antivirus software interference

---

## 📊 **Demo Success Metrics**

### **What to Look For**
- ✅ Claude successfully discovers all 4 tools
- ✅ Tools respond within 5-10 seconds
- ✅ JSON output is well-formatted
- ✅ Error handling works gracefully
- ✅ Team shows interest and asks questions

### **Engagement Indicators**
- Team members asking about specific use cases
- Questions about integration with existing tools
- Interest in trying the tools themselves
- Discussion about potential improvements

### **Red Flags**
- Tools not responding
- Poor performance (slow responses)
- Technical errors during demo
- Team appears disinterested

---

## 🎤 **Presentation Tips**

### **Before the Demo**
1. **Practice**: Run through the demo 2-3 times beforehand
2. **Prepare**: Have backup projects ready
3. **Test**: Ensure all tools work with your test projects
4. **Timing**: Keep demo moving, don't get stuck on technical details

### **During the Demo**
1. **Explain**: Always explain what you're doing and why
2. **Engage**: Ask the team questions about their projects
3. **Adapt**: Adjust based on team interest and questions
4. **Focus**: Stay on track, don't go down rabbit holes

### **After the Demo**
1. **Summarize**: Recap key benefits and next steps
2. **Q&A**: Encourage questions and discussion
3. **Follow-up**: Provide resources for team to try themselves
4. **Feedback**: Ask for feedback and suggestions

---

## 📝 **Demo Notes Template**

### **Team Information**
- **Team Name**: ________________
- **Date**: ________________
- **Attendees**: ________________
- **Project Types**: ________________

### **Demo Results**
- **Tools Discovered**: ✅ / ❌
- **Project Analysis**: ✅ / ❌
- **Dependency Analysis**: ✅ / ❌
- **Code Quality Check**: ✅ / ❌
- **Project Summary**: ✅ / ❌

### **Team Feedback**
- **Interest Level**: High / Medium / Low
- **Key Questions**: ________________
- **Use Cases Discussed**: ________________
- **Next Steps**: ________________

### **Technical Issues**
- **Issues Encountered**: ________________
- **Solutions Applied**: ________________
- **Follow-up Needed**: ________________

---

*Demo script prepared for Smart Code Assistant MCP Server team presentation*
