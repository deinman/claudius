using System.Text;
using Claudius.CLI.Tools;

namespace Claudius.CLI;

public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools = new();

    public ToolRegistry()
    {
        Register(new ReadFileTool());
        Register(new ListFilesTool());
        Register(new EditFileTool());
    }

    private void Register(ITool tool) => _tools[tool.Name] = tool;

    public ITool? GetTool(string name) => _tools.GetValueOrDefault(name);

    public string BuildSystemPrompt()
    {
        var sb = new StringBuilder();
        foreach (var tool in _tools.Values)
        {
            sb.AppendLine("TOOL");
            sb.AppendLine("===");
            sb.AppendLine($"    Name: {tool.Name}");
            sb.AppendLine($"    Description: {tool.Description}");
            sb.AppendLine($"    Signature: {tool.Signature}");
            sb.AppendLine(new string('=', 15));
        }

        return $$"""
            You are a coding assistant whose goal it is to help us solve coding tasks.
            You have access to a series of tools you can execute. Hear are the tools you can execute:

            {{sb}}

            When you want to use a tool, reply with exactly one line in the format: 'tool: TOOL_NAME({JSON_ARGS})' and nothing else.
            Use compact single-line JSON with double quotes. After receiving a tool_result(...) message, continue the task.
            If no tool is needed, respond normally.
            """;
    }
}
