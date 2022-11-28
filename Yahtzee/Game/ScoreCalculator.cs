using static Yahtzee.Game.ScoreOption;

namespace Yahtzee.Game;

public static class ScoreCalculator
{
    public static int CalculateFinalScore(GameContext context)
    {
        var preBonus = context.Scores.Values.Sum();
        
        var upperBonus = context.CurrentUpperScore >= 63 ? 35 : 0;

        var yahtzeeBonus = context.NumBonuses * 100;

        return preBonus + upperBonus + yahtzeeBonus;
    }

    public static bool IsYahtzee(List<int> dice)
    {
        return dice.Distinct().Count() == 1;
    }
    
    public static int Calculate(bool hasScoredYahtzee, ScoreOption option, List<int> dice)
    {
        var isBonusYahtzee = hasScoredYahtzee && IsYahtzee(dice);
        
        return option switch
        {
            Ones => SumNs(dice, 1),
            Twos => SumNs(dice, 2),
            Threes => SumNs(dice, 3),
            Fours => SumNs(dice, 4),
            Fives => SumNs(dice, 5),
            Sixes => SumNs(dice, 6),
            Chance => dice.Sum(),
            ThreeKind => IsNKind(dice, 3) ? dice.Sum() : 0,
            FourKind => IsNKind(dice, 4) ? dice.Sum() : 0,
            YahtzeeOption => IsYahtzee(dice) ? 50 : 0,
            FullHouse => IsFullHouse(dice) || isBonusYahtzee ? 25 : 0,
            ShortStraight => IsShortStraight(dice) || isBonusYahtzee ? 30 : 0,
            LongStraight => IsLongStraight(dice) || isBonusYahtzee ? 40 : 0,
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }

    private static bool IsShortStraight(IEnumerable<int> dice)
    {
        var diceSet = dice.ToHashSet();
        
        return diceSet.IsSupersetOf(new HashSet<int> { 1, 2, 3, 4 })
               || diceSet.IsSupersetOf(new HashSet<int> { 2, 3, 4, 5 })
               || diceSet.IsSupersetOf(new HashSet<int> { 3, 4, 5, 6 });
    }

    private static bool IsLongStraight(IEnumerable<int> dice)
    {
        var diceSet = dice.ToHashSet();
        
        return diceSet.SetEquals(new HashSet<int> { 1, 2, 3, 4, 5 })
               || diceSet.SetEquals(new HashSet<int> { 2, 3, 4, 5, 6 });
    }

    private static bool IsNKind(IEnumerable<int> dice, int n)
    {
        return dice.GroupBy(x => x)
            .Any(x => x.Count() >= n);
    }

    private static bool IsFullHouse(IEnumerable<int> dice)
    {
        var groups = dice.GroupBy(x => x).ToList();
        
        return groups.Any(x => x.Count() == 2)
               && groups.Any(x => x.Count() == 3);
    }

    private static int SumNs(IEnumerable<int> dice, int n)
    {
        return dice.Count(x => x == n) * n;
    }
}