using Yahtzee.Agent;

namespace Yahtzee.Game;

public class MultiGame
{
    private readonly IAgent[] _agents;
    private readonly int _repeats;

    public MultiGame(IAgent[] agents, int repeats)
    {
        _agents = agents;
        _repeats = repeats;
    }

    public Dictionary<IAgent, List<int>> Play()
    {
        var results = _agents.ToDictionary(x => x, _ => new List<int>());

        for (var i = 0; i < _repeats; i++)
        {
            var rolls = RollGenerator.Generate();
            
            Parallel.ForEach(_agents, agent =>
            {
                var game = new Game(agent, rolls);
                results[agent].Add(game.Play());
            });
        }

        return results;
    }
}