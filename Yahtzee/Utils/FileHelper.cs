namespace Yahtzee.Utils;

public static class FileHelper
{
    public static string ProjectPath => Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
}