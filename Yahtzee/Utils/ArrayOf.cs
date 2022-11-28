namespace Yahtzee.Utils;

public static class Array
{
    public static T[] Of<T>(Func<T> generator, int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => generator())
            .ToArray();
    }
}