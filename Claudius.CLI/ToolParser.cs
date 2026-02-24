using System.Text.Json;

namespace Claudius.CLI;

public static class ToolParser
{
    public static List<(string Name, Dictionary<string, JsonElement> Args)> ExtractToolInvocations(string text)
    {
        var invocations = new List<(string, Dictionary<string, JsonElement>)>();

        foreach (var rawLine in text.Split('\n'))
        {
            var line = rawLine.Trim();
            if (!line.StartsWith("tool:"))
                continue;

            try
            {
                var after = line["tool:".Length..].Trim();
                var parenIndex = after.IndexOf('(');
                if (parenIndex < 0) continue;

                var name = after[..parenIndex].Trim();
                var rest = after[(parenIndex + 1)..];

                if (!rest.EndsWith(')')) continue;

                var jsonStr = rest[..^1].Trim();
                var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonStr)!;
                invocations.Add((name, args));
            }
            catch
            {
                continue;
            }
        }

        return invocations;
    }
}
