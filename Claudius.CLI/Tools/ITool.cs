using System.Text.Json;

namespace Claudius.CLI.Tools;

public interface ITool
{
    string Name { get; }
    string Description { get; }
    string Signature { get; }
    Task<Dictionary<string, object>> ExecuteAsync(Dictionary<string, JsonElement> args);
}
