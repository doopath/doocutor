using Common;
using TextBuffer.TextBufferContents;
using Utils.Exceptions.NotExitExceptions;

namespace TextBuffer;

public static class TextBufferManager
{
    public static void OpenAsTextBuffer(string path, ITextBuffer textBuffer)
    {
        path = ModifyPath(path);
        string fileContent = File.ReadAllText(path).Replace("\r", "");
        List<string> fileContentLines = fileContent.Split("\n").ToList();
        
        textBuffer.ClearHistory();
        textBuffer.ReplaceCurrentContentBy(fileContentLines);
        textBuffer.SetCursorPositionFromTopAt(0);
        textBuffer.SetCursorPositionFromLeftAt(textBuffer.GetPrefixLength());
        textBuffer.FilePath = path;
    }

    public static void SaveTextBufferAsFile(ITextBuffer textBuffer, string? path = null)
    {
        if (path is null && textBuffer.FilePath is null)
            throw new TextBufferSavingException("Cannot save the textBuffer because of file path is not set!");

        string[] textBufferContent = textBuffer.Lines;

        File.WriteAllLinesAsync(path ?? textBuffer.FilePath!, textBufferContent);
    }

    public static bool IsDirPathCorrect(string path)
        => Directory.Exists(GetDirectory(path));

    public static bool IsFilePathCorrect(string path)
        => File.Exists(ModifyPath(path));

    public static string ModifyPath(string path)
    {
        char separator = Path.DirectorySeparatorChar;
        string homeDir = (Environment.GetEnvironmentVariable("userdir")
                          ?? Environment.GetEnvironmentVariable("HOME"))!
                         + separator;

        return path
            .Replace("~/", homeDir)
            .Replace("~\\", homeDir);
    }

    public static string GetDirectory(string path)
    {
        char separator = Path.DirectorySeparatorChar;
        path = ModifyPath(path);

        if (Directory.Exists(path))
            return path;

        string cutPath = string.Join(separator, path.Split(separator)[..^1]);

        if (Directory.Exists(cutPath))
            return cutPath;

        return string.Empty;
    }
}