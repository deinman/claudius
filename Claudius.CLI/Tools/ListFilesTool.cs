using System.Text.Json;

namespace Claudius.CLI.Tools;

public class ListFilesTool : ITool
{
    public string Name => "list_files";

    public string Description =>
        "Lists the files in a directory provided by the user.\n" +
        ":param path: The path to a directory to list files from.\n" +
        ":return: A list of files in the directory.";

    public string Signature => "(path: str)";

    public Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, JsonElement> args)
    {
        var path = args.TryGetValue("path", out var v) ? v.GetString()! : ".";
        var fullPath = PathHelper.ResolveAbsolutePath(path);

        var entries = new DirectoryInfo(fullPath).GetFileSystemInfos();
        var files = entries.Select(e => new Dictionary<string, object>
        {
            ["filename"] = e.Name,
            ["type"] = e is FileInfo ? "file" : "dir"
        }).ToList<object>();

        return Task.FromResult(new Dictionary<string, object>
        {
            ["path"] = fullPath,
            ["files"] = files
        });
    }
}
