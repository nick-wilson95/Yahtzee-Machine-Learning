using Yahtzee.Game;

namespace Yahtzee.Agent;

public class HumanPlayer : IAgent
{
    public PlayOutput Play(GameContext context)
    {
        return Play(context, null);
    }
    
    private static PlayOutput Play(GameContext context, string message)
    {
        Console.Clear();
        
        if (message is not null)
        {
            Console.WriteLine(message);
            Console.WriteLine("");
        }
        
        DisplayScores(context);
        Console.WriteLine("");
        
        Console.WriteLine($"Dice are {string.Join(", ", context.Dice)}");

        Console.WriteLine(context.NumRerolls < 2
            ? "Enter reroll booleans (eg. 00101) or a score option:"
            : "Enter a score option:");

        var input = Console.ReadLine();

        if (context.NumRerolls < 2 && TryParseRerollInput(input, out var indices))
        {
            return new PlayOutput(null, indices);
        }

        if (!int.TryParse(input,out _) &&  Enum.TryParse<ScoreOption>(input, out var option))
        {
            if (context.Scores.ContainsKey(option))
            {
                return Play(context, "Option already scored");
            }
            
            return new PlayOutput(option, null);
        }

        return Play(context, "Invalid input");
    }

    private static void DisplayScores(GameContext context)
    {
        foreach (var option in Enum.GetValues<ScoreOption>())
        {
            var hasScore = context.Scores.TryGetValue(option, out int score);
            var scoreText = hasScore ? score.ToString() : "-";
            Console.WriteLine($"{option}: {scoreText}");
        }
    }

    private static bool TryParseRerollInput(string input, out bool[] rerollIndices)
    {
        rerollIndices = null;
        
        if (input.Length != 5 || input.Any(x => x != '0' && x != '1'))
        {
            return false;
        }

        rerollIndices = input.Select(x => (int)char.GetNumericValue(x) == 1).ToArray();
        return true;
    }
}