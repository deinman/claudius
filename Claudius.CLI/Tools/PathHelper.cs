namespace Claudius.CLI.Tools;

public static class PathHelper
{
    public static string ResolveAbsolutePath(string path)
    {
        if (path.StartsWith('~'))
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path[1..].TrimStart('/'));

        if (!Path.IsPathRooted(path))
            path = Path.Combine(Directory.GetCurrentDirectory(), path);

        return Path.GetFullPath(path);
    }
}
