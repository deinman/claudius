using System.Text.Json;

namespace Claudius.CLI.Tools;

public class EditFileTool : ITool
{
    public string Name => "edit_file";

    public string Description =>
        "Replaces first occurrence of old_str with new_str in file. If old_str is empty,\n" +
        "create/overwrite file with new_str.\n" +
        ":param path: The path to the file to edit.\n" +
        ":param old_str: The string to replace.\n" +
        ":param new_str: The string to replace with.\n" +
        ":return: A dictionary with the path to the file and the action taken.";

    public string Signature => "(path: str, old_str: str, new_str: str)";

    public Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, JsonElement> args)
    {
        var path = args.TryGetValue("path", out var v) ? v.GetString()! : ".";
        var oldStr = args.TryGetValue("old_str", out var o) ? o.GetString()! : "";
        var newStr = args.TryGetValue("new_str", out var n) ? n.GetString()! : "";

        var fullPath = PathHelper.ResolveAbsolutePath(path);

        if (string.IsNullOrEmpty(oldStr))
        {
            var dir = Path.GetDirectoryName(fullPath);
            if (dir != null) Directory.CreateDirectory(dir);
            File.WriteAllText(fullPath, newStr);
            return Task.FromResult(new Dictionary<string, object>
            {
                ["path"] = fullPath,
                ["action"] = "created_file"
            });
        }

        var original = File.ReadAllText(fullPath);
        var idx = original.IndexOf(oldStr, StringComparison.Ordinal);
        if (idx == -1)
        {
            return Task.FromResult(new Dictionary<string, object>
            {
                ["path"] = fullPath,
                ["action"] = "old_str not found"
            });
        }

        var edited = original[..idx] + newStr + original[(idx + oldStr.Length)..];
        File.WriteAllText(fullPath, edited);

        return Task.FromResult(new Dictionary<string, object>
        {
            ["path"] = fullPath,
            ["action"] = "edited"
        });
    }
}
