using Yahtzee.Agent;
using Yahtzee.Exceptions;

namespace Yahtzee.Game;

public class Game
{
    private readonly IAgent _agent;
    private readonly GameContext _gameContext;

    public Game(IAgent agent, int[][] diceRolls = null)
    {
        _agent = agent;
        diceRolls ??= RollGenerator.Generate();
        _gameContext = new GameContext(diceRolls);
    }

    public int Play()
    {
        while (_gameContext.CanPlay)
        {
            _gameContext.NewTurn();
            
            var output = _agent.Play(_gameContext);

            while (output.ScoreOption is null)
            {
                Reroll(output.Rerolls);
                output = _agent.Play(_gameContext);
            }

            var hasScoredYahtzee = _gameContext.Scores.TryGetValue(ScoreOption.YahtzeeOption, out var yahtzeeScore) &&
                                   yahtzeeScore > 0;

            if (hasScoredYahtzee && ScoreCalculator.IsYahtzee(_gameContext.Dice))
            {
                _gameContext.AddBonus();
            }
            
            var score = ScoreCalculator.Calculate(hasScoredYahtzee, output.ScoreOption.Value, _gameContext.Dice);
            
            _gameContext.SetScore(output.ScoreOption.Value, score);
        }

        return ScoreCalculator.CalculateFinalScore(_gameContext);
    }

    private void Reroll(bool[] rerolls)
    {
        if (_gameContext.NumRerolls > 1)
        {
            throw new RuleException("Tried to reroll more than twice");
        }

        _gameContext.Reroll(rerolls);
    }
}