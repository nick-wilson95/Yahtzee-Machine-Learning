using System.Text.Json;
using NeuralNetwork;
using Yahtzee.Agent;
using Yahtzee.Game;
using Array = Yahtzee.Utils.Array;

namespace Yahtzee;

public class Experiment : ISerialiseable<Experiment>
{
    private const int InitialLayerSizeMultiplier = 4;
    
    private readonly int _numberOfReproducers;
    private readonly int _numberOfOffspring;
    private readonly int _gamesPerGeneration;

    /// Very roughly number of seconds per iteration
    public double IterationComplexity =>
        _numberOfReproducers
        * _numberOfOffspring
        * _gamesPerGeneration
        * Agents[0].NumHiddenLayers
        * Math.Pow(Agents[0].HiddenLayerSize, 2)
        / 1_000_000;

    public AiPlayer[] Agents { get; private set; }

    public static Experiment Deserialise(string toDeserialise)
    {
        var deserialised = JsonSerializer.Deserialize<SerialisableExperiment>(toDeserialise);
        var aiPlayers = deserialised.SerialisedAiPlayers
            .Select(x => AiPlayer.Deserialise(x))
            .ToArray();
        
        return new Experiment(deserialised.NumberOfReproducers, deserialised.NumberOfOffspring, deserialised.GamesPerGeneration, aiPlayers);
    }

    public Experiment(
        int numberOfReproducers,
        int numberOfOffspring,
        int gamesPerGeneration,
        int aiNumHiddenLayers,
        int aiHiddenLayerSize)
    {
        _numberOfReproducers = numberOfReproducers;
        _numberOfOffspring = numberOfOffspring;
        _gamesPerGeneration = gamesPerGeneration;

        Agents = Array.Of(
            () => new AiPlayer(aiNumHiddenLayers, aiHiddenLayerSize),
            numberOfReproducers * numberOfOffspring * InitialLayerSizeMultiplier
        );
    }

    public Experiment(
        int numberOfReproducers,
        int numberOfOffspring,
        int gamesPerGeneration,
        AiPlayer[] agents)
    {
        _numberOfReproducers = numberOfReproducers;
        _numberOfOffspring = numberOfOffspring;
        _gamesPerGeneration = gamesPerGeneration;

        Agents = agents;
    }
    
    public IEnumerable<double> Iterate()
    {
        var fitness = GetFitness();

        Agents = GetNextGen(fitness);

        return fitness.Values;
    }

    private Dictionary<IAgent, double> GetFitness()
    {
        var results = new MultiGame(Agents, _gamesPerGeneration).Play();
        
        return results.ToDictionary(x => x.Key, x => x.Value.Average());
    }

    private AiPlayer[] GetNextGen(Dictionary<IAgent, double> fitness)
    {
        var survivors = fitness
            .OrderByDescending(x => x.Value)
            .Take(_numberOfReproducers)
            .Select(x => (AiPlayer)x.Key);
        
        return survivors
            .SelectMany(x => x.Reproduce(_numberOfOffspring))
            .Concat(survivors)
            .ToArray();
    }

    public string Serialise()
    {
        var toSerialise = new SerialisableExperiment
        {
            NumberOfReproducers = _numberOfReproducers,
            NumberOfOffspring = _numberOfOffspring,
            GamesPerGeneration = _gamesPerGeneration,
            SerialisedAiPlayers = Agents.Select(x => x.Serialise()).ToList()
        };
            
        return JsonSerializer.Serialize(toSerialise);
    }
    
    private class SerialisableExperiment
    {
        public int NumberOfReproducers { get; set; }
        public int NumberOfOffspring { get; set; }
        public int GamesPerGeneration { get; set; }
        public List<string> SerialisedAiPlayers { get; set; }
    }
}