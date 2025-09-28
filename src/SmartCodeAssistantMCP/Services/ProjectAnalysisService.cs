using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartCodeAssistantMCP.Services;

/// <summary>
/// Service for analyzing .NET project structure and code quality
/// </summary>
public class ProjectAnalysisService
{
    private readonly ILogger<ProjectAnalysisService> _logger;

    public ProjectAnalysisService(ILogger<ProjectAnalysisService> logger)
    {
        _logger = logger;
        
        // Initialize MSBuild locator if not already done
        if (!MSBuildLocator.IsRegistered)
        {
            try
            {
                var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
                if (instances.Length > 0)
                {
                    MSBuildLocator.RegisterInstance(instances[0]);
                    _logger.LogInformation("MSBuild locator registered with instance: {Instance}", instances[0].Name);
                }
                else
                {
                    _logger.LogWarning("No MSBuild instances found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register MSBuild locator");
            }
        }
    }

    /// <summary>
    /// Analyzes a .NET solution or project structure
    /// </summary>
    /// <param name="projectPath">Path to the solution (.sln) or project (.csproj) file</param>
    /// <returns>Analysis results as JSON</returns>
    public async Task<string> AnalyzeProjectAsync(string projectPath)
    {
        try
        {
            _logger.LogInformation("Starting project analysis for: {ProjectPath}", projectPath);

            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Project file not found: {projectPath}");
            }

            var analysis = new ProjectAnalysisResult
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow
            };

            // Determine if it's a solution or project file
            if (Path.GetExtension(projectPath).Equals(".sln", StringComparison.OrdinalIgnoreCase))
            {
                await AnalyzeSolutionAsync(projectPath, analysis);
            }
            else if (Path.GetExtension(projectPath).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                await AnalyzeProjectFileAsync(projectPath, analysis);
            }
            else
            {
                throw new ArgumentException("File must be a .sln or .csproj file");
            }

            return JsonSerializer.Serialize(analysis, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing project: {ProjectPath}", projectPath);
            throw;
        }
    }

    private async Task AnalyzeSolutionAsync(string solutionPath, ProjectAnalysisResult analysis)
    {
        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(solutionPath);

        analysis.ProjectType = "Solution";
        analysis.Projects = new List<ProjectInfo>();

        foreach (var project in solution.Projects)
        {
            var projectInfo = new ProjectInfo
            {
                Name = project.Name,
                FilePath = project.FilePath,
                Language = project.Language,
                AssemblyName = project.AssemblyName,
                OutputFilePath = project.OutputFilePath
            };

            // Analyze project references
            projectInfo.ProjectReferences = project.ProjectReferences
                .Select(pr => pr.ProjectId.ToString())
                .ToList();

            // Analyze metadata references (NuGet packages)
            projectInfo.MetadataReferences = project.MetadataReferences
                .Select(mr => mr.Display ?? "Unknown")
                .ToList();

            // Count documents and lines of code
            projectInfo.DocumentCount = project.Documents.Count();
            projectInfo.LinesOfCode = await CountLinesOfCodeAsync(project.Documents);

            analysis.Projects.Add(projectInfo);
        }

        analysis.TotalProjects = analysis.Projects.Count;
        analysis.TotalDocuments = analysis.Projects.Sum(p => p.DocumentCount);
        analysis.TotalLinesOfCode = analysis.Projects.Sum(p => p.LinesOfCode);
    }

    private async Task AnalyzeProjectFileAsync(string projectPath, ProjectAnalysisResult analysis)
    {
        using var workspace = MSBuildWorkspace.Create();
        var project = await workspace.OpenProjectAsync(projectPath);

        analysis.ProjectType = "Project";
        analysis.Projects = new List<ProjectInfo>
        {
            new ProjectInfo
            {
                Name = project.Name,
                FilePath = project.FilePath,
                Language = project.Language,
                AssemblyName = project.AssemblyName,
                OutputFilePath = project.OutputFilePath,
                ProjectReferences = project.ProjectReferences.Select(pr => pr.ProjectId.ToString()).ToList(),
                MetadataReferences = project.MetadataReferences.Select(mr => mr.Display ?? "Unknown").ToList(),
                DocumentCount = project.Documents.Count(),
                LinesOfCode = await CountLinesOfCodeAsync(project.Documents)
            }
        };

        analysis.TotalProjects = 1;
        analysis.TotalDocuments = analysis.Projects[0].DocumentCount;
        analysis.TotalLinesOfCode = analysis.Projects[0].LinesOfCode;
    }

    private async Task<int> CountLinesOfCodeAsync(IEnumerable<Document> documents)
    {
        var totalLines = 0;
        
        foreach (var document in documents)
        {
            if (document.FilePath?.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) == true)
            {
                var text = await document.GetTextAsync();
                totalLines += text.Lines.Count;
            }
        }

        return totalLines;
    }
}

/// <summary>
/// Result of project analysis
/// </summary>
public class ProjectAnalysisResult
{
    public string ProjectPath { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public DateTime AnalysisTimestamp { get; set; }
    public int TotalProjects { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalLinesOfCode { get; set; }
    public int AverageLinesPerFile => TotalDocuments > 0 ? TotalLinesOfCode / TotalDocuments : 0;
    public List<ProjectInfo> Projects { get; set; } = new();
}

/// <summary>
/// Information about a specific project
/// </summary>
public class ProjectInfo
{
    public string Name { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string Language { get; set; } = string.Empty;
    public string? AssemblyName { get; set; }
    public string? OutputFilePath { get; set; }
    public List<string> ProjectReferences { get; set; } = new();
    public List<string> MetadataReferences { get; set; } = new();
    public int DocumentCount { get; set; }
    public int LinesOfCode { get; set; }
}
