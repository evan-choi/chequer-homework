using System;
using System.IO;

namespace CSVQueryLanguage.Utilities;

internal static class PathUtility
{
    public static bool IsValidPath(ReadOnlySpan<char> path)
    {
        if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            return false;

        var invalidFileNameChars = Path.GetInvalidFileNameChars();

        while (!path.IsEmpty)
        {
            ReadOnlySpan<char> directoryName = Path.GetDirectoryName(path);
            ReadOnlySpan<char> fileName = Path.GetFileName(path);

            if (fileName.IndexOfAny(invalidFileNameChars) >= 0)
                return false;

            path = directoryName;
        }

        return true;
    }
}
