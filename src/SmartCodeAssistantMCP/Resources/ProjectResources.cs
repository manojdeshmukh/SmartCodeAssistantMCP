using ModelContextProtocol.Server;
using SmartCodeAssistantMCP.Services;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace SmartCodeAssistantMCP.Resources;

/// <summary>
/// MCP resources for accessing project information
/// </summary>
internal class ProjectResources
{
    private readonly ProjectAnalysisService _projectAnalysisService;
    private readonly ILogger<ProjectResources> _logger;

    public ProjectResources(ProjectAnalysisService projectAnalysisService, ILogger<ProjectResources> logger)
    {
        _projectAnalysisService = projectAnalysisService;
        _logger = logger;
    }

    [McpServerResource]
    [Description("Provides access to project file structure and organization information")]
    public async Task<string> GetProjectStructure(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Providing project structure resource for: {ProjectPath}", projectPath);

            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<ProjectAnalysisResult>(analysisResult);

            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            // Format as a readable structure overview
            var structure = new List<string>();
            structure.Add($"# Project Structure: {Path.GetFileNameWithoutExtension(projectPath)}");
            structure.Add("");
            structure.Add($"**Analysis Date**: {analysis.AnalysisTimestamp:yyyy-MM-dd HH:mm:ss} UTC");
            structure.Add($"**Project Type**: {analysis.ProjectType}");
            structure.Add("");

            structure.Add("## Overview");
            structure.Add($"- Total Projects: {analysis.TotalProjects}");
            structure.Add($"- Total Files: {analysis.TotalDocuments:N0}");
            structure.Add($"- Total Lines of Code: {analysis.TotalLinesOfCode:N0}");
            structure.Add("");

            structure.Add("## Projects");
            foreach (var project in analysis.Projects)
            {
                structure.Add($"### {project.Name}");
                structure.Add($"- **Path**: {project.FilePath}");
                structure.Add($"- **Language**: {project.Language}");
                structure.Add($"- **Assembly**: {project.AssemblyName}");
                structure.Add($"- **Files**: {project.DocumentCount:N0}");
                structure.Add($"- **Lines of Code**: {project.LinesOfCode:N0}");
                structure.Add($"- **Project References**: {project.ProjectReferences.Count}");
                structure.Add($"- **Package References**: {project.MetadataReferences.Count}");
                structure.Add("");
            }

            return string.Join("\n", structure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error providing project structure resource");
            return $"Error accessing project structure: {ex.Message}";
        }
    }

    [McpServerResource]
    [Description("Provides access to project dependencies and package information")]
    public async Task<string> GetProjectDependencies(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath)
    {
        try
        {
            _logger.LogInformation("Providing project dependencies resource for: {ProjectPath}", projectPath);

            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<ProjectAnalysisResult>(analysisResult);

            if (analysis == null)
            {
                return "Failed to analyze project dependencies";
            }

            var dependencies = new List<string>();
            dependencies.Add($"# Project Dependencies: {Path.GetFileNameWithoutExtension(projectPath)}");
            dependencies.Add("");
            dependencies.Add($"**Analysis Date**: {analysis.AnalysisTimestamp:yyyy-MM-dd HH:mm:ss} UTC");
            dependencies.Add("");

            // Collect all unique dependencies
            var allDependencies = analysis.Projects
                .SelectMany(p => p.MetadataReferences)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            dependencies.Add($"## Summary");
            dependencies.Add($"- Total Unique Dependencies: {allDependencies.Count}");
            dependencies.Add($"- Total Project References: {analysis.Projects.Sum(p => p.ProjectReferences.Count)}");
            dependencies.Add("");

            dependencies.Add("## All Dependencies");
            foreach (var dependency in allDependencies)
            {
                dependencies.Add($"- {dependency}");
            }
            dependencies.Add("");

            dependencies.Add("## Dependencies by Project");
            foreach (var project in analysis.Projects)
            {
                dependencies.Add($"### {project.Name}");
                dependencies.Add($"**Package References ({project.MetadataReferences.Count}):**");
                foreach (var dep in project.MetadataReferences.OrderBy(d => d))
                {
                    dependencies.Add($"- {dep}");
                }
                dependencies.Add("");
                dependencies.Add($"**Project References ({project.ProjectReferences.Count}):**");
                foreach (var projRef in project.ProjectReferences)
                {
                    dependencies.Add($"- {projRef}");
                }
                dependencies.Add("");
            }

            return string.Join("\n", dependencies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error providing project dependencies resource");
            return $"Error accessing project dependencies: {ex.Message}";
        }
    }
}
