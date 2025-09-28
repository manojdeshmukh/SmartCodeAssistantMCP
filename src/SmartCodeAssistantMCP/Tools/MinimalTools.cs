using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;

namespace SmartCodeAssistantMCP.Tools;

/// <summary>
/// Minimal MCP tools for basic .NET project analysis
/// </summary>
internal class MinimalTools
{
    private readonly ILogger<MinimalTools> _logger;

    public MinimalTools(ILogger<MinimalTools> logger)
    {
        _logger = logger;
    }

    [McpServerTool]
    [Description("Analyzes a .NET project and returns basic information about its structure.")]
    public async Task<string> AnalyzeProject(
        [Description("Path to the project file (.csproj) to analyze")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Analyzing project: {ProjectPath}", projectPath);

            var result = new
            {
                ProjectPath = projectPath,
                ProjectName = Path.GetFileNameWithoutExtension(projectPath),
                AnalysisTimestamp = DateTime.UtcNow,
                Status = "Success",
                Message = "Basic project analysis completed",
                BasicInfo = new
                {
                    FileExists = File.Exists(projectPath),
                    ProjectDirectory = Path.GetDirectoryName(projectPath),
                    FileSize = File.Exists(projectPath) ? new FileInfo(projectPath).Length : 0
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing project");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Finds basic dependencies in a .NET project file.")]
    public async Task<string> FindDependencies(
        [Description("Path to the project file (.csproj)")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Finding dependencies for: {ProjectPath}", projectPath);

            if (!File.Exists(projectPath))
            {
                return "Project file not found";
            }

            var content = await File.ReadAllTextAsync(projectPath);
            var packageReferences = new List<string>();

            // Simple regex to find package references
            var matches = System.Text.RegularExpressions.Regex.Matches(content, @"<PackageReference\s+Include=""([^""]+)""");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                packageReferences.Add(match.Groups[1].Value);
            }

            var result = new
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                Dependencies = new
                {
                    Count = packageReferences.Count,
                    Packages = packageReferences
                },
                Message = $"Found {packageReferences.Count} package references"
            };

            return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding dependencies");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Performs basic code quality checks on a .NET project.")]
    public async Task<string> CheckCodeQuality(
        [Description("Path to the project file (.csproj)")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Checking code quality for: {ProjectPath}", projectPath);

            var projectDir = Path.GetDirectoryName(projectPath);
            if (string.IsNullOrEmpty(projectDir))
            {
                return "Could not determine project directory";
            }

            // Count C# files in the project directory
            var csFiles = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories);
            var totalLines = 0;
            var fileCount = 0;

            foreach (var file in csFiles.Take(10)) // Limit to first 10 files
            {
                try
                {
                    var lines = await File.ReadAllLinesAsync(file);
                    totalLines += lines.Length;
                    fileCount++;
                }
                catch
                {
                    // Skip files that can't be read
                }
            }

            var result = new
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                QualityMetrics = new
                {
                    CSharpFilesFound = csFiles.Length,
                    FilesAnalyzed = fileCount,
                    TotalLinesOfCode = totalLines,
                    AverageLinesPerFile = fileCount > 0 ? totalLines / fileCount : 0
                },
                Recommendations = new List<string>
                {
                    csFiles.Length > 50 ? "Consider organizing code into smaller modules" : "File organization looks reasonable",
                    totalLines > 10000 ? "Large codebase - consider refactoring" : "Code size is manageable"
                },
                Message = "Basic code quality analysis completed"
            };

            return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking code quality");
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Generates a basic project summary with key metrics.")]
    public async Task<string> GenerateProjectSummary(
        [Description("Path to the project file (.csproj)")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Generating project summary for: {ProjectPath}", projectPath);

            var projectDir = Path.GetDirectoryName(projectPath);
            var projectName = Path.GetFileNameWithoutExtension(projectPath);

            var summary = new
            {
                ProjectName = projectName,
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                QuickStats = new
                {
                    ProjectExists = File.Exists(projectPath),
                    ProjectDirectory = projectDir,
                    ProjectFileSize = File.Exists(projectPath) ? new FileInfo(projectPath).Length : 0
                },
                Summary = $"Project '{projectName}' analysis completed successfully",
                Status = "Ready for development"
            };

            return System.Text.Json.JsonSerializer.Serialize(summary, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project summary");
            return $"Error: {ex.Message}";
        }
    }
}
