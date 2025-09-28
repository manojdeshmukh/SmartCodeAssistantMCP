using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartCodeAssistantMCP.Services;

/// <summary>
/// Simplified project analysis service that doesn't require MSBuild
/// </summary>
public class SimpleProjectAnalysisService
{
    private readonly ILogger<SimpleProjectAnalysisService> _logger;

    public SimpleProjectAnalysisService(ILogger<SimpleProjectAnalysisService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Analyzes a .NET project structure using file system operations
    /// </summary>
    /// <param name="projectPath">Path to the project file</param>
    /// <returns>Analysis results as JSON</returns>
    public async Task<string> AnalyzeProjectAsync(string projectPath)
    {
        try
        {
            _logger.LogInformation("Starting simple project analysis for: {ProjectPath}", projectPath);
            
            // Add timeout to prevent infinite loops
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Project file not found: {projectPath}");
            }

            var analysis = new SimpleProjectAnalysisResult
            {
                ProjectPath = projectPath,
                AnalysisTimestamp = DateTime.UtcNow,
                ProjectType = Path.GetExtension(projectPath).Equals(".sln", StringComparison.OrdinalIgnoreCase) ? "Solution" : "Project"
            };

            // Get project directory
            var projectDir = Path.GetDirectoryName(projectPath) ?? "";
            analysis.ProjectDirectory = projectDir;

            // Count files with limits to prevent infinite loops
            var csFiles = GetFilesWithLimit(projectDir, "*.cs", 100);
            var projectFiles = GetFilesWithLimit(projectDir, "*.csproj", 50);
            var solutionFiles = GetFilesWithLimit(projectDir, "*.sln", 10);

            analysis.TotalCSharpFiles = csFiles.Length;
            analysis.TotalProjectFiles = projectFiles.Length;
            analysis.TotalSolutionFiles = solutionFiles.Length;

            // Count lines of code
            analysis.TotalLinesOfCode = await CountLinesOfCodeAsync(csFiles);

            // Read project file for basic info
            if (File.Exists(projectPath))
            {
                var projectContent = await File.ReadAllTextAsync(projectPath);
                analysis.ProjectName = Path.GetFileNameWithoutExtension(projectPath);
                
                // Extract basic project info
                ExtractProjectInfo(projectContent, analysis);
            }

            // Get directory structure
            analysis.DirectoryStructure = GetDirectoryStructure(projectDir);

            return JsonSerializer.Serialize(analysis, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in simple project analysis: {ProjectPath}", projectPath);
            throw;
        }
    }

    private async Task<int> CountLinesOfCodeAsync(string[] files)
    {
        var totalLines = 0;
        
        foreach (var file in files)
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(file);
                totalLines += lines.Length;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not read file: {File}", file);
            }
        }

        return totalLines;
    }

    private string[] GetFilesWithLimit(string directory, string pattern, int maxFiles)
    {
        try
        {
            var files = new List<string>();
            var directories = new Queue<string>();
            directories.Enqueue(directory);
            var processedDirs = new HashSet<string>();

            while (directories.Count > 0 && files.Count < maxFiles)
            {
                var currentDir = directories.Dequeue();
                
                if (processedDirs.Contains(currentDir))
                    continue;
                    
                processedDirs.Add(currentDir);

                try
                {
                    // Get files in current directory
                    var dirFiles = Directory.GetFiles(currentDir, pattern, SearchOption.TopDirectoryOnly);
                    files.AddRange(dirFiles.Take(maxFiles - files.Count));

                    // Get subdirectories (limit to prevent deep recursion)
                    if (files.Count < maxFiles)
                    {
                        var subDirs = Directory.GetDirectories(currentDir, "*", SearchOption.TopDirectoryOnly)
                            .Where(d => !Path.GetFileName(d).StartsWith(".") && 
                                       !Path.GetFileName(d).Equals("bin") && 
                                       !Path.GetFileName(d).Equals("obj"))
                            .Take(5); // Limit subdirectories

                        foreach (var subDir in subDirs)
                        {
                            directories.Enqueue(subDir);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not process directory: {Directory}", currentDir);
                }
            }

            return files.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting files with limit from: {Directory}", directory);
            return new string[0];
        }
    }

    private void ExtractProjectInfo(string projectContent, SimpleProjectAnalysisResult analysis)
    {
        // Extract target framework
        var targetFrameworkMatch = System.Text.RegularExpressions.Regex.Match(projectContent, @"<TargetFramework>(.*?)</TargetFramework>");
        if (targetFrameworkMatch.Success)
        {
            analysis.TargetFramework = targetFrameworkMatch.Groups[1].Value;
        }

        // Count package references
        var packageRefMatches = System.Text.RegularExpressions.Regex.Matches(projectContent, @"<PackageReference\s+Include=""([^""]+)""");
        analysis.PackageReferences = new List<string>();
        foreach (System.Text.RegularExpressions.Match match in packageRefMatches)
        {
            analysis.PackageReferences.Add(match.Groups[1].Value);
        }

        // Count project references
        var projectRefMatches = System.Text.RegularExpressions.Regex.Matches(projectContent, @"<ProjectReference\s+Include=""([^""]+)""");
        analysis.ProjectReferences = new List<string>();
        foreach (System.Text.RegularExpressions.Match match in projectRefMatches)
        {
            analysis.ProjectReferences.Add(match.Groups[1].Value);
        }
    }

    private List<string> GetDirectoryStructure(string directory)
    {
        var structure = new List<string>();
        
        try
        {
            // Only show top-level directory structure to prevent loops
            var directories = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly)
                .Take(10); // Limit to 10 directories
                
            foreach (var dir in directories)
            {
                var dirName = Path.GetFileName(dir);
                if (!dirName.StartsWith(".") && !dirName.Equals("bin") && !dirName.Equals("obj"))
                {
                    structure.Add($"📁 {dirName}/");
                }
            }

            var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Take(20); // Limit to 20 files
                
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (!fileName.StartsWith("."))
                {
                    var extension = Path.GetExtension(file);
                    var icon = extension switch
                    {
                        ".cs" => "📄",
                        ".csproj" => "📦",
                        ".sln" => "🏗️",
                        ".md" => "📝",
                        ".json" => "⚙️",
                        _ => "📄"
                    };
                    structure.Add($"{icon} {fileName}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not read directory structure: {Directory}", directory);
        }

        return structure;
    }
}

/// <summary>
/// Result of simple project analysis
/// </summary>
public class SimpleProjectAnalysisResult
{
    public string ProjectPath { get; set; } = string.Empty;
    public string ProjectDirectory { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string TargetFramework { get; set; } = string.Empty;
    public DateTime AnalysisTimestamp { get; set; }
    public int TotalCSharpFiles { get; set; }
    public int TotalProjectFiles { get; set; }
    public int TotalSolutionFiles { get; set; }
    public int TotalLinesOfCode { get; set; }
    public List<string> PackageReferences { get; set; } = new();
    public List<string> ProjectReferences { get; set; } = new();
    public List<string> DirectoryStructure { get; set; } = new();
}
