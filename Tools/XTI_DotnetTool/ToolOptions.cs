namespace XTI_DotnetTool;

internal sealed class ToolOptions
{
    public string SrcDir { get; set; } = "";
    public string RepoOwner { get; set; } = "";
    public string RepoName { get; set; } = "";
    public string AppName { get; set; } = "";
    public string AppType { get; set; } = "";
    public string TestType { get; set; } = "";
    public string GroupName { get; set; } = "";
    public bool DeleteExisting { get; set; }
    public bool InstallTemplatesLocally { get; set; }
    public string Command { get; set; } = "";
    public string Domain { get; set; } = "development.guinevere.com";
}