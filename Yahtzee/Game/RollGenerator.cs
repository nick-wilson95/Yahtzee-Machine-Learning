namespace Yahtzee.Game;
using Array = Yahtzee.Utils.Array;

public static class RollGenerator
{
    private static readonly Random Random = new();
    
    public static int[][] Generate()
    {
        return Array.Of(
            () => Array.Of(
                () => (int)MathF.Ceiling(Random.NextSingle() * 6),
                15),
            13);
    }
}