using NeuralNetwork;
using Yahtzee.Game;
using Array = Yahtzee.Utils.Array;

namespace Yahtzee.Agent;

public class AiPlayer : IAgent, ISerialiseable<AiPlayer>
{
    private NeuralNet _neuralNet;

    public int NumHiddenLayers => _neuralNet.NumHiddenLayers;
    public int HiddenLayerSize => _neuralNet.HiddenLayerSize;

    public static AiPlayer Deserialise(string toDeserialise)
    {
        return new AiPlayer(NeuralNet.Deserialise(toDeserialise));
    }
    
    public AiPlayer(int numHiddenLayers, int hiddenLayerSize)
    {
        _neuralNet = new NeuralNet(47, 18, numHiddenLayers, hiddenLayerSize);
    }
    
    private AiPlayer(NeuralNet neuralNet)
    {
        _neuralNet = neuralNet;
    }

    public string Serialise() => _neuralNet.Serialise();

    public AiPlayer[] Reproduce(int numOffspring)
    {
        return Array.Of(
            () => new AiPlayer(_neuralNet.Spawn(Randomiser.Randomise)),
            numOffspring
        );
    }
    
    public PlayOutput Play(GameContext context)
    {
        var inputLayer = MapToInput(context);
        var outputLayer = _neuralNet.Activate(inputLayer);
        return MapFromOutput(context, outputLayer);
    }

    private static float[] MapToInput(GameContext context)
    {
        var diceValues = new[]{ 1, 2, 3, 4, 5, 6 };
        var diceIndices = new[] { 0, 1, 2, 3, 4 };

        var diceInputs = diceValues.SelectMany(
            _ => diceIndices,
            (v, i) => context.Dice[i] == v ? 1f : 0f
        );

        var scoredOptionInputs = Enum.GetValues<ScoreOption>().Select(x => MapScored(context.Scores, x));

        var rerollInputs = new[] { 0f, 0f, 0f };
        rerollInputs[context.NumRerolls] = 1;

        return diceInputs
            .Concat(scoredOptionInputs)
            .Concat(rerollInputs)
            .Concat(new[]{(float)context.CurrentUpperScore})
            .ToArray();
    }

    private static float MapScored(Dictionary<ScoreOption, int> contextScores, ScoreOption option)
    {
        return contextScores.ContainsKey(option) ? 1 : 0;
    }

    private static PlayOutput MapFromOutput(GameContext gameContext, float[] outputLayer)
    {
        var rerolls = outputLayer.Take(5).Select(x => x > 0).ToArray();

        if (rerolls.Any() && gameContext.NumRerolls < 2) return new PlayOutput(null, rerolls);
        
        var alreadyScoredOptions = gameContext.Scores.Keys.Select(x => (int)x);

        var scoreOptionIndex = outputLayer.Skip(5)
            .Select((x, i) => (value: x, index: i))
            .Where(x => !alreadyScoredOptions.Contains(x.index))
            .MaxBy(x => x.value)
            .index;

        var scoreOption = (ScoreOption)scoreOptionIndex;

        return new PlayOutput(scoreOption, rerolls);
    }
}