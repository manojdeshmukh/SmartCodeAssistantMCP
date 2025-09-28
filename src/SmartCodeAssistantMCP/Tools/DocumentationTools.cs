using System.ComponentModel;
using ModelContextProtocol.Server;
using SmartCodeAssistantMCP.Services;
using Microsoft.Extensions.Logging;

namespace SmartCodeAssistantMCP.Tools;

/// <summary>
/// MCP tools for generating documentation from .NET projects
/// </summary>
internal class DocumentationTools(DocumentationService documentationService, ILogger<DocumentationTools> logger)
{
    private readonly DocumentationService _documentationService = documentationService;
    private readonly ILogger<DocumentationTools> _logger = logger;

    [McpServerTool]
    [Description("Auto-generates a comprehensive README.md file for a .NET project or solution, including project overview, structure, dependencies, and API documentation.")]
    public async Task<string> GenerateReadme(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath,
        [Description("Include API documentation in the README")] bool includeApiDocs = true,
        [Description("Output path for the generated README file (optional, returns content if not specified)")] string? outputPath = null)
    {
        try
        {
            _logger.LogInformation("Generating README for: {ProjectPath}", projectPath);

            var readmeContent = await _documentationService.GenerateReadmeAsync(projectPath, includeApiDocs);

            if (!string.IsNullOrEmpty(outputPath))
            {
                await File.WriteAllTextAsync(outputPath, readmeContent);
                _logger.LogInformation("README written to: {OutputPath}", outputPath);
                return $"README successfully generated and saved to: {outputPath}";
            }

            return readmeContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating README");
            return $"Error generating README: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Extracts API documentation from XML comments in .NET code and returns it in a structured format.")]
    public async Task<string> ExtractApiDocs(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath,
        [Description("Output format: 'markdown' or 'json'")] string format = "markdown",
        [Description("Output path for the generated documentation file (optional, returns content if not specified)")] string? outputPath = null)
    {
        try
        {
            _logger.LogInformation("Extracting API documentation from: {ProjectPath}", projectPath);

            var apiDocs = await _documentationService.ExtractApiDocumentationAsync(projectPath);

            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                // Convert to JSON format
                var jsonDocs = new
                {
                    ProjectPath = projectPath,
                    ExtractionTimestamp = DateTime.UtcNow,
                    Documentation = apiDocs
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(jsonDocs, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                if (!string.IsNullOrEmpty(outputPath))
                {
                    await File.WriteAllTextAsync(outputPath, jsonContent);
                    _logger.LogInformation("API documentation written to: {OutputPath}", outputPath);
                    return $"API documentation successfully generated and saved to: {outputPath}";
                }

                return jsonContent;
            }
            else
            {
                // Return as markdown
                if (!string.IsNullOrEmpty(outputPath))
                {
                    await File.WriteAllTextAsync(outputPath, apiDocs);
                    _logger.LogInformation("API documentation written to: {OutputPath}", outputPath);
                    return $"API documentation successfully generated and saved to: {outputPath}";
                }

                return apiDocs;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting API documentation");
            return $"Error extracting API documentation: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Generates a project summary with key metrics and insights for quick project overview.")]
    public async Task<string> GenerateProjectSummary(
        [Description("Path to the solution (.sln) or project (.csproj) file")] string projectPath,
        [Description("Include detailed metrics in the summary")] bool includeDetailedMetrics = false)
    {
        try
        {
            _logger.LogInformation("Generating project summary for: {ProjectPath}", projectPath);

            var analysisResult = await _documentationService.AnalyzeProjectAsync(projectPath);
            var analysis = System.Text.Json.JsonSerializer.Deserialize<ProjectAnalysisResult>(analysisResult);

            if (analysis == null)
            {
                return "Failed to analyze project structure";
            }

            var summary = new List<string>();

            // Project header
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            summary.Add($"# {projectName} - Project Summary");
            summary.Add("");
            summary.Add($"**Analysis Date**: {analysis.AnalysisTimestamp:yyyy-MM-dd HH:mm:ss} UTC");
            summary.Add("");

            // Quick stats
            summary.Add("## Quick Stats");
            summary.Add("");
            summary.Add($"- 📁 **Project Type**: {analysis.ProjectType}");
            summary.Add($"- 🏗️ **Total Projects**: {analysis.TotalProjects}");
            summary.Add($"- 📄 **Total Files**: {analysis.TotalDocuments:N0}");
            summary.Add($"- 📝 **Lines of Code**: {analysis.TotalLinesOfCode:N0}");
            summary.Add($"- 📊 **Avg Lines/File**: {(analysis.TotalDocuments > 0 ? analysis.TotalLinesOfCode / analysis.TotalDocuments : 0):N0}");
            summary.Add("");

            // Project breakdown
            summary.Add("## Project Breakdown");
            summary.Add("");
            foreach (var project in analysis.Projects)
            {
                summary.Add($"### {project.Name}");
                summary.Add($"- **Language**: {project.Language}");
                summary.Add($"- **Assembly**: {project.AssemblyName}");
                summary.Add($"- **Files**: {project.DocumentCount:N0}");
                summary.Add($"- **Lines of Code**: {project.LinesOfCode:N0}");
                summary.Add($"- **Dependencies**: {project.MetadataReferences.Count}");
                summary.Add("");
            }

            // Detailed metrics if requested
            if (includeDetailedMetrics)
            {
                summary.Add("## Detailed Metrics");
                summary.Add("");

                // Dependency analysis
                var allDependencies = analysis.Projects
                    .SelectMany(p => p.MetadataReferences)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                summary.Add($"### Dependencies ({allDependencies.Count} total)");
                summary.Add("");
                foreach (var dependency in allDependencies.Take(10)) // Show top 10
                {
                    summary.Add($"- {dependency}");
                }
                if (allDependencies.Count > 10)
                {
                    summary.Add($"- ... and {allDependencies.Count - 10} more");
                }
                summary.Add("");

                // Code distribution
                summary.Add("### Code Distribution");
                summary.Add("");
                var totalLoc = analysis.TotalLinesOfCode;
                foreach (var project in analysis.Projects.OrderByDescending(p => p.LinesOfCode))
                {
                    var percentage = totalLoc > 0 ? (double)project.LinesOfCode / totalLoc * 100 : 0;
                    summary.Add($"- **{project.Name}**: {project.LinesOfCode:N0} lines ({percentage:F1}%)");
                }
                summary.Add("");
            }

            // Recommendations
            summary.Add("## Recommendations");
            summary.Add("");
            var recommendations = GenerateSummaryRecommendations(analysis);
            foreach (var recommendation in recommendations)
            {
                summary.Add($"- {recommendation}");
            }
            summary.Add("");

            return string.Join("\n", summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project summary");
            return $"Error generating project summary: {ex.Message}";
        }
    }

    private List<string> GenerateSummaryRecommendations(ProjectAnalysisResult analysis)
    {
        var recommendations = new List<string>();

        if (analysis.TotalLinesOfCode > 100000)
        {
            recommendations.Add("🔍 **Large codebase detected** - Consider modularization for better maintainability");
        }

        if (analysis.TotalDocuments > 1000)
        {
            recommendations.Add("📁 **High file count** - Review folder structure and organization");
        }

        if (analysis.TotalProjects == 1 && analysis.TotalLinesOfCode > 50000)
        {
            recommendations.Add("🏗️ **Single large project** - Consider splitting into multiple projects");
        }

        var avgLinesPerFile = analysis.TotalDocuments > 0 ? analysis.TotalLinesOfCode / analysis.TotalDocuments : 0;
        if (avgLinesPerFile > 500)
        {
            recommendations.Add("📄 **Large files detected** - Consider breaking down large files into smaller, focused classes");
        }

        var totalDependencies = analysis.Projects.Sum(p => p.MetadataReferences.Count);
        if (totalDependencies > 50)
        {
            recommendations.Add("📦 **Many dependencies** - Review if all dependencies are necessary");
        }

        if (recommendations.Count == 0)
        {
            recommendations.Add("✅ **Project structure looks good** - No major concerns detected");
        }

        return recommendations;
    }
}
