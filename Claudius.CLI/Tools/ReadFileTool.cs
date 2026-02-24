using System.Text.Json;

namespace Claudius.CLI.Tools;

public class ReadFileTool : ITool
{
    public string Name => "read_file";

    public string Description =>
        "Gets the full content of a file provided by the user.\n" +
        ":param filename: The name of the file to read.\n" +
        ":return: The full content of the file.";

    public string Signature => "(filename: str)";

    public Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, JsonElement> args)
    {
        var filename = args.TryGetValue("filename", out var v) ? v.GetString()! : ".";
        var fullPath = PathHelper.ResolveAbsolutePath(filename);

        Console.WriteLine(fullPath);

        var content = File.ReadAllText(fullPath);

        return Task.FromResult(new Dictionary<string, object>
        {
            ["file_path"] = fullPath,
            ["content"] = content
        });
    }
}
