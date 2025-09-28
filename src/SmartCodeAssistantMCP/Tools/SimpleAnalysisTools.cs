using System.ComponentModel;
using ModelContextProtocol.Server;
using SmartCodeAssistantMCP.Services;
using Microsoft.Extensions.Logging;

namespace SmartCodeAssistantMCP.Tools;

/// <summary>
/// Simplified MCP tools for .NET project analysis without MSBuild dependencies
/// </summary>
internal class SimpleAnalysisTools
{
    private readonly SimpleProjectAnalysisService _projectAnalysisService;
    private readonly ILogger<SimpleAnalysisTools> _logger;

    public SimpleAnalysisTools(SimpleProjectAnalysisService projectAnalysisService, ILogger<SimpleAnalysisTools> logger)
    {
        _projectAnalysisService = projectAnalysisService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Analyzes a .NET project structure using file system operations (no MSBuild required).")]
    public async Task<string> AnalyzeProject(
        [Description("Path to the solution (.sln) or project (.csproj) file to analyze")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Analyzing project: {ProjectPath}", projectPath);
            
            var result = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            
            _logger.LogInformation("Project analysis completed successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during project analysis");
            return $"Error analyzing project: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Discovers dependencies in a .NET project by parsing the project file.")]
    public async Task<string> FindDependencies(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Finding dependencies for: {ProjectPath}", projectPath);
            
            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<SimpleProjectAnalysisResult>(analysisResult);
            
            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            var dependencyReport = new
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                analysis.ProjectName,
                analysis.TargetFramework,
                PackageReferences = new
                {
                    analysis.PackageReferences.Count,
                    Packages = analysis.PackageReferences
                },
                ProjectReferences = new
                {
                    analysis.ProjectReferences.Count,
                    References = analysis.ProjectReferences
                },
                Summary = $"Found {analysis.PackageReferences.Count} NuGet packages and {analysis.ProjectReferences.Count} project references"
            };

            return System.Text.Json.JsonSerializer.Serialize(dependencyReport, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding dependencies");
            return $"Error finding dependencies: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Performs basic code quality checks using file system analysis.")]
    public async Task<string> CheckCodeQuality(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Checking code quality for: {ProjectPath}", projectPath);
            
            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<SimpleProjectAnalysisResult>(analysisResult);
            
            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            var qualityReport = new
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                analysis.ProjectName,
                Metrics = new
                {
                    TotalFiles = analysis.TotalCSharpFiles,
                    analysis.TotalLinesOfCode,
                    AverageLinesPerFile = analysis.TotalCSharpFiles > 0 ? analysis.TotalLinesOfCode / analysis.TotalCSharpFiles : 0,
                    analysis.TargetFramework
                },
                Recommendations = GenerateQualityRecommendations(analysis),
                analysis.DirectoryStructure
            };

            return System.Text.Json.JsonSerializer.Serialize(qualityReport, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking code quality");
            return $"Error checking code quality: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Generates a basic project summary with key metrics.")]
    public async Task<string> GenerateProjectSummary(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Generating project summary for: {ProjectPath}", projectPath);
            
            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<SimpleProjectAnalysisResult>(analysisResult);
            
            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            var summary = new
            {
                analysis.ProjectName,
                analysis.ProjectType,
                analysis.TargetFramework,
                analysis.AnalysisTimestamp,
                QuickStats = new
                {
                    CSharpFiles = analysis.TotalCSharpFiles,
                    LinesOfCode = analysis.TotalLinesOfCode,
                    NuGetPackages = analysis.PackageReferences.Count,
                    ProjectReferences = analysis.ProjectReferences.Count
                },
                TopLevelStructure = analysis.DirectoryStructure.Take(10).ToList(),
                Recommendations = GenerateQualityRecommendations(analysis)
            };

            return System.Text.Json.JsonSerializer.Serialize(summary, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project summary");
            return $"Error generating project summary: {ex.Message}";
        }
    }

    private List<string> GenerateQualityRecommendations(SimpleProjectAnalysisResult analysis)
    {
        var recommendations = new List<string>();

        if (analysis.TotalLinesOfCode > 100000)
        {
            recommendations.Add("🔍 Large codebase detected - consider modularization for better maintainability");
        }

        if (analysis.TotalCSharpFiles > 100)
        {
            recommendations.Add("📁 High file count - review folder structure and organization");
        }

        var avgLinesPerFile = analysis.TotalCSharpFiles > 0 ? analysis.TotalLinesOfCode / analysis.TotalCSharpFiles : 0;
        if (avgLinesPerFile > 500)
        {
            recommendations.Add("📄 Large files detected - consider breaking down large files into smaller, focused classes");
        }

        if (analysis.PackageReferences.Count > 50)
        {
            recommendations.Add("📦 Many dependencies - review if all packages are necessary");
        }

        if (string.IsNullOrEmpty(analysis.TargetFramework))
        {
            recommendations.Add("⚠️ Target framework not detected - ensure project file is valid");
        }

        if (recommendations.Count == 0)
        {
            recommendations.Add("✅ Project structure looks good - no major concerns detected");
        }

        return recommendations;
    }
}
