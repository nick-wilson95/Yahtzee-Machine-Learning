namespace Yahtzee.Game;

public record PlayOutput(ScoreOption? ScoreOption, bool[] Rerolls)
{
}

public enum ScoreOption
{
    Ones,
    Twos,
    Threes,
    Fours,
    Fives,
    Sixes,
    ThreeKind,
    FourKind,
    FullHouse,
    ShortStraight,
    LongStraight,
    YahtzeeOption,
    Chance
}