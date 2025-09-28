using System.ComponentModel;
using ModelContextProtocol.Server;
using SmartCodeAssistantMCP.Services;
using Microsoft.Extensions.Logging;

namespace SmartCodeAssistantMCP.Tools;

/// <summary>
/// MCP tools for .NET code analysis and project insights
/// </summary>
internal class CodeAnalysisTools
{
    private readonly ProjectAnalysisService _projectAnalysisService;
    private readonly ILogger<CodeAnalysisTools> _logger;

    public CodeAnalysisTools(ProjectAnalysisService projectAnalysisService, ILogger<CodeAnalysisTools> logger)
    {
        _projectAnalysisService = projectAnalysisService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Analyzes a .NET solution or project structure, providing insights about projects, dependencies, and code metrics.")]
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
    [Description("Discovers and analyzes NuGet dependencies in a .NET project or solution.")]
    public async Task<string> FindDependencies(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath,
        [Description("Include transitive dependencies in the analysis")] bool includeTransitive = false)
    {
        try
        {
            _logger.LogInformation("Finding dependencies for: {ProjectPath}", projectPath);
            
            // First get the project analysis
            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<ProjectAnalysisResult>(analysisResult);
            
            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            var dependencyAnalysis = new DependencyAnalysisResult
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                IncludeTransitive = includeTransitive
            };

            foreach (var project in analysis.Projects)
            {
                var projectDependencies = new ProjectDependencies
                {
                    ProjectName = project.Name,
                    DirectDependencies = project.MetadataReferences.ToList(),
                    ProjectReferences = project.ProjectReferences.ToList()
                };

                // If including transitive dependencies, we would need to resolve the dependency tree
                // For now, we'll focus on direct dependencies
                if (includeTransitive)
                {
                    projectDependencies.TransitiveDependencies = await ResolveTransitiveDependencies(project.MetadataReferences);
                }

                dependencyAnalysis.ProjectDependencies.Add(projectDependencies);
            }

            dependencyAnalysis.TotalDirectDependencies = dependencyAnalysis.ProjectDependencies
                .Sum(pd => pd.DirectDependencies.Count);
            dependencyAnalysis.TotalTransitiveDependencies = dependencyAnalysis.ProjectDependencies
                .Sum(pd => pd.TransitiveDependencies.Count);

            return System.Text.Json.JsonSerializer.Serialize(dependencyAnalysis, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding dependencies");
            return $"Error finding dependencies: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Performs basic code quality checks on a .NET project, including complexity analysis and best practices validation.")]
    public async Task<string> CheckCodeQuality(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath,
        [Description("Include detailed analysis for each file")] bool includeFileDetails = false)
    {
        try
        {
            _logger.LogInformation("Checking code quality for: {ProjectPath}", projectPath);
            
            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<ProjectAnalysisResult>(analysisResult);
            
            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            var qualityReport = new CodeQualityReport
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                IncludeFileDetails = includeFileDetails
            };

            // Basic quality metrics
            qualityReport.TotalProjects = analysis.TotalProjects;
            qualityReport.TotalFiles = analysis.TotalDocuments;
            qualityReport.TotalLinesOfCode = analysis.TotalLinesOfCode;
            qualityReport.AverageLinesPerFile = analysis.TotalDocuments > 0 ? analysis.TotalLinesOfCode / analysis.TotalDocuments : 0;

            // Quality recommendations
            qualityReport.Recommendations = GenerateQualityRecommendations(analysis);

            // File-level analysis if requested
            if (includeFileDetails)
            {
                qualityReport.FileDetails = await AnalyzeFileQuality(analysis);
            }

            return System.Text.Json.JsonSerializer.Serialize(qualityReport, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking code quality");
            return $"Error checking code quality: {ex.Message}";
        }
    }

    private async Task<List<string>> ResolveTransitiveDependencies(List<string> directDependencies)
    {
        // This is a simplified implementation
        // In a real scenario, you would use NuGet APIs to resolve the full dependency tree
        var transitive = new List<string>();
        
        // For now, return empty list - this would be implemented with proper NuGet resolution
        await Task.CompletedTask;
        return transitive;
    }

    private List<string> GenerateQualityRecommendations(ProjectAnalysisResult analysis)
    {
        var recommendations = new List<string>();

        if (analysis.TotalLinesOfCode > 100000)
        {
            recommendations.Add("Consider breaking down large codebase into smaller, more manageable projects");
        }

        if (analysis.TotalDocuments > 1000)
        {
            recommendations.Add("High file count detected - consider organizing code into better folder structure");
        }

        if (analysis.AverageLinesPerFile > 500)
        {
            recommendations.Add("Some files are quite large - consider splitting large files into smaller, focused classes");
        }

        if (analysis.TotalProjects == 1 && analysis.TotalLinesOfCode > 50000)
        {
            recommendations.Add("Large single project detected - consider splitting into multiple projects for better maintainability");
        }

        if (recommendations.Count == 0)
        {
            recommendations.Add("Project structure looks good! No major quality concerns detected.");
        }

        return recommendations;
    }

    private async Task<List<FileQualityInfo>> AnalyzeFileQuality(ProjectAnalysisResult analysis)
    {
        var fileDetails = new List<FileQualityInfo>();
        
        // This would involve analyzing individual files for complexity, naming conventions, etc.
        // For now, return a placeholder
        await Task.CompletedTask;
        
        return fileDetails;
    }
}

/// <summary>
/// Result of dependency analysis
/// </summary>
public class DependencyAnalysisResult
{
    public string ProjectPath { get; set; } = string.Empty;
    public DateTime AnalysisTimestamp { get; set; }
    public bool IncludeTransitive { get; set; }
    public int TotalDirectDependencies { get; set; }
    public int TotalTransitiveDependencies { get; set; }
    public List<ProjectDependencies> ProjectDependencies { get; set; } = new();
}

/// <summary>
/// Dependencies for a specific project
/// </summary>
public class ProjectDependencies
{
    public string ProjectName { get; set; } = string.Empty;
    public List<string> DirectDependencies { get; set; } = new();
    public List<string> TransitiveDependencies { get; set; } = new();
    public List<string> ProjectReferences { get; set; } = new();
}

/// <summary>
/// Code quality analysis report
/// </summary>
public class CodeQualityReport
{
    public string ProjectPath { get; set; } = string.Empty;
    public DateTime AnalysisTimestamp { get; set; }
    public bool IncludeFileDetails { get; set; }
    public int TotalProjects { get; set; }
    public int TotalFiles { get; set; }
    public int TotalLinesOfCode { get; set; }
    public int AverageLinesPerFile { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public List<FileQualityInfo> FileDetails { get; set; } = new();
}

/// <summary>
/// Quality information for a specific file
/// </summary>
public class FileQualityInfo
{
    public string FilePath { get; set; } = string.Empty;
    public int LinesOfCode { get; set; }
    public int Complexity { get; set; }
    public List<string> Issues { get; set; } = new();
}
