using Markdig;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartCodeAssistantMCP.Services;

/// <summary>
/// Service for generating documentation from .NET projects
/// </summary>
public class DocumentationService
{
    private readonly ILogger<DocumentationService> _logger;
    private readonly ProjectAnalysisService _projectAnalysisService;

    public DocumentationService(ILogger<DocumentationService> logger, ProjectAnalysisService projectAnalysisService)
    {
        _logger = logger;
        _projectAnalysisService = projectAnalysisService;
        
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
    /// Generates a comprehensive README for a .NET project
    /// </summary>
    /// <param name="projectPath">Path to the project or solution</param>
    /// <param name="includeApiDocs">Whether to include API documentation</param>
    /// <returns>Generated README content</returns>
    public async Task<string> GenerateReadmeAsync(string projectPath, bool includeApiDocs = true)
    {
        try
        {
            _logger.LogInformation("Generating README for: {ProjectPath}", projectPath);

            var analysisResult = await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
            var analysis = JsonSerializer.Deserialize<ProjectAnalysisResult>(analysisResult);

            if (analysis == null)
            {
                throw new InvalidOperationException("Failed to analyze project");
            }

            var readme = new List<string>();

            // Header
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            readme.Add($"# {projectName}");
            readme.Add("");
            readme.Add($"A .NET {analysis.ProjectType.ToLower()} built with modern development practices.");
            readme.Add("");

            // Project Overview
            readme.Add("## Project Overview");
            readme.Add("");
            readme.Add($"- **Project Type**: {analysis.ProjectType}");
            readme.Add($"- **Total Projects**: {analysis.TotalProjects}");
            readme.Add($"- **Total Files**: {analysis.TotalDocuments}");
            readme.Add($"- **Lines of Code**: {analysis.TotalLinesOfCode:N0}");
            readme.Add($"- **Analysis Date**: {analysis.AnalysisTimestamp:yyyy-MM-dd HH:mm:ss} UTC");
            readme.Add("");

            // Project Structure
            readme.Add("## Project Structure");
            readme.Add("");
            foreach (var project in analysis.Projects)
            {
                readme.Add($"### {project.Name}");
                readme.Add($"- **Language**: {project.Language}");
                readme.Add($"- **Assembly**: {project.AssemblyName}");
                readme.Add($"- **Files**: {project.DocumentCount}");
                readme.Add($"- **Lines of Code**: {project.LinesOfCode:N0}");
                readme.Add("");
            }

            // Dependencies
            readme.Add("## Dependencies");
            readme.Add("");
            var allDependencies = analysis.Projects
                .SelectMany(p => p.MetadataReferences)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (allDependencies.Any())
            {
                readme.Add("### NuGet Packages");
                readme.Add("");
                foreach (var dependency in allDependencies)
                {
                    readme.Add($"- {dependency}");
                }
                readme.Add("");
            }

            // Getting Started
            readme.Add("## Getting Started");
            readme.Add("");
            readme.Add("### Prerequisites");
            readme.Add("");
            readme.Add("- .NET 8.0 SDK or later");
            readme.Add("- Visual Studio 2022 or VS Code with C# extension");
            readme.Add("");
            readme.Add("### Installation");
            readme.Add("");
            readme.Add("1. Clone the repository");
            readme.Add("2. Restore NuGet packages:");
            readme.Add("   ```bash");
            readme.Add("   dotnet restore");
            readme.Add("   ```");
            readme.Add("3. Build the solution:");
            readme.Add("   ```bash");
            readme.Add("   dotnet build");
            readme.Add("   ```");
            readme.Add("4. Run the application:");
            readme.Add("   ```bash");
            readme.Add("   dotnet run");
            readme.Add("   ```");
            readme.Add("");

            // API Documentation
            if (includeApiDocs)
            {
                var apiDocs = await ExtractApiDocumentationAsync(projectPath);
                if (!string.IsNullOrEmpty(apiDocs))
                {
                    readme.Add("## API Documentation");
                    readme.Add("");
                    readme.Add(apiDocs);
                    readme.Add("");
                }
            }

            // Contributing
            readme.Add("## Contributing");
            readme.Add("");
            readme.Add("1. Fork the repository");
            readme.Add("2. Create a feature branch");
            readme.Add("3. Make your changes");
            readme.Add("4. Add tests if applicable");
            readme.Add("5. Submit a pull request");
            readme.Add("");

            // License
            readme.Add("## License");
            readme.Add("");
            readme.Add("This project is licensed under the MIT License - see the LICENSE file for details.");
            readme.Add("");

            return string.Join("\n", readme);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating README");
            throw;
        }
    }

    /// <summary>
    /// Extracts API documentation from code comments
    /// </summary>
    /// <param name="projectPath">Path to the project or solution</param>
    /// <returns>Formatted API documentation</returns>
    public async Task<string> ExtractApiDocumentationAsync(string projectPath)
    {
        try
        {
            _logger.LogInformation("Extracting API documentation from: {ProjectPath}", projectPath);

            using var workspace = MSBuildWorkspace.Create();
            var projects = new List<Project>();

            if (Path.GetExtension(projectPath).Equals(".sln", StringComparison.OrdinalIgnoreCase))
            {
                var solution = await workspace.OpenSolutionAsync(projectPath);
                projects.AddRange(solution.Projects);
            }
            else if (Path.GetExtension(projectPath).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                var project = await workspace.OpenProjectAsync(projectPath);
                projects.Add(project);
            }

            var apiDocs = new List<string>();
            var publicTypes = new List<TypeDocumentation>();

            foreach (var project in projects)
            {
                var compilation = await project.GetCompilationAsync();
                if (compilation == null) continue;

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    var semanticModel = compilation.GetSemanticModel(syntaxTree);
                    var root = await syntaxTree.GetRootAsync();

                    var publicMembers = root.DescendantNodes()
                        .OfType<MemberDeclarationSyntax>()
                        .Where(IsPublicMember)
                        .ToList();

                    foreach (var member in publicMembers)
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(member);
                        if (symbol == null) continue;

                        var doc = ExtractMemberDocumentation(member, symbol);
                        if (doc != null)
                        {
                            publicTypes.Add(doc);
                        }
                    }
                }
            }

            if (publicTypes.Any())
            {
                apiDocs.Add("### Public API");
                apiDocs.Add("");

                var groupedTypes = publicTypes.GroupBy(t => t.Namespace).OrderBy(g => g.Key);
                foreach (var group in groupedTypes)
                {
                    apiDocs.Add($"#### {group.Key}");
                    apiDocs.Add("");

                    foreach (var type in group.OrderBy(t => t.Name))
                    {
                        apiDocs.Add($"**{type.Name}**");
                        if (!string.IsNullOrEmpty(type.Summary))
                        {
                            apiDocs.Add($"- {type.Summary}");
                        }
                        apiDocs.Add("");
                    }
                }
            }

            return string.Join("\n", apiDocs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting API documentation");
            return $"Error extracting API documentation: {ex.Message}";
        }
    }

    private bool IsPublicMember(MemberDeclarationSyntax member)
    {
        return member.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
    }

    private TypeDocumentation? ExtractMemberDocumentation(MemberDeclarationSyntax member, ISymbol symbol)
    {
        var xmlComments = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(xmlComments)) return null;

        var summary = ExtractXmlElement(xmlComments, "summary");
        var name = symbol.Name;
        var namespaceName = symbol.ContainingNamespace?.ToDisplayString() ?? "Global";

        return new TypeDocumentation
        {
            Name = name,
            Namespace = namespaceName,
            Summary = summary,
            Type = symbol.Kind.ToString()
        };
    }

    private string ExtractXmlElement(string xml, string elementName)
    {
        var startTag = $"<{elementName}>";
        var endTag = $"</{elementName}>";
        
        var startIndex = xml.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1) return string.Empty;
        
        startIndex += startTag.Length;
        var endIndex = xml.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase);
        if (endIndex == -1) return string.Empty;
        
        return xml.Substring(startIndex, endIndex - startIndex).Trim();
    }

    /// <summary>
    /// Analyzes a project and returns the analysis result
    /// </summary>
    /// <param name="projectPath">Path to the project or solution</param>
    /// <returns>Project analysis result as JSON</returns>
    public async Task<string> AnalyzeProjectAsync(string projectPath)
    {
        return await _projectAnalysisService.AnalyzeProjectAsync(projectPath);
    }
}

/// <summary>
/// Documentation for a type or member
/// </summary>
public class TypeDocumentation
{
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
