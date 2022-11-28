namespace Yahtzee.Game;

public class GameContext
{
    public int NumBonuses { get; private set; } = 0;
    
    private int _currentTurn;
    private readonly int[][] _diceRolls;
    
    private static readonly ScoreOption[] UpperScoreOptions =
    {
        ScoreOption.Ones, ScoreOption.Twos, ScoreOption.Threes, ScoreOption.Fours, ScoreOption.Fives, ScoreOption.Sixes
    };
    
    public GameContext(int[][] diceRolls)
    {
        _diceRolls = diceRolls;
    }

    private readonly int[] _dice = new int[5];
    public List<int> Dice => _dice.ToList();
    public int CurrentUpperScore => UpperScoreOptions
        .Intersect(Scores.Keys)
        .Sum(x => Scores[x]);

    public int NumRerolls { get; private set; } = 0;
    public bool CanPlay => _currentTurn < 13;

    public readonly Dictionary<ScoreOption, int> Scores = new();

    public void NewTurn()
    {
        _currentTurn++;
        
        NumRerolls = 0;
        
        for (var i = 0; i < _dice.Length; i++)
        {
            _dice[i] = _diceRolls[_currentTurn - 1][i];
        }
    }

    public void Reroll(bool[] diceRerolls)
    {
        NumRerolls++;

        for (var i = 0; i < _dice.Length; i++)
        {
            if (diceRerolls[i])
            {
                _dice[i] = _diceRolls[_currentTurn - 1][i + 5 * NumRerolls];
            }
        }
    }

    public void SetScore(ScoreOption option, int score)
    {
        Scores.Add(option, score);
    }

    public void AddBonus()
    {
        NumBonuses++;
    }
}