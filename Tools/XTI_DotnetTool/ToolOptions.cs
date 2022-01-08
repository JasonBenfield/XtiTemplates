namespace XTI_DotnetTool;

internal sealed class ToolOptions
{
    public string Directory { get; set; } = "";
    public string AppName { get; set; } = "";
    public string AppType { get; set; } = "";
    public bool DeleteExisting { get; set; }
}