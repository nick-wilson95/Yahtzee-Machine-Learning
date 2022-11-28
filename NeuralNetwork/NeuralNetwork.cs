using System.Text.Json;

namespace NeuralNetwork;

public class NeuralNet : ISerialiseable<NeuralNet>
{
    private readonly float[][] _nodes;
    private readonly float[][][] _weights;
    
    private static readonly Random Random = new();

    private float[] InputLayer => _nodes[0];
    private float[] OutputLayer => _nodes[^1];

    public int NumHiddenLayers => _nodes.Length - 2;
    public int HiddenLayerSize => _nodes[1].Length;

    public static NeuralNet Deserialise(string toDeserialise)
    {
        var deserialised = JsonSerializer.Deserialize<SerialisableNeuralNet>(toDeserialise);
        return new NeuralNet(deserialised.Nodes, deserialised.Weights, x => x);
    }

    public NeuralNet(int numInputNodes, int numOutputNodes, int numHiddenLayers, int numHiddenLayerNodes)
    {
        _nodes = InitialiseNodes(numInputNodes, numOutputNodes, numHiddenLayers, numHiddenLayerNodes);
        _weights = InitialiseWeights(_nodes);
    }
    
    private NeuralNet(float[][] nodes, float[][][] weights, Func<float,float> weightEvolver)
    {
        _nodes = nodes.DeepCopy();
        _weights = weights.DeepCopy();
        
        Evolve(weightEvolver);
    }

    public string Serialise()
    {
        var toSerialise = new SerialisableNeuralNet
        {
            Weights = _weights,
            Nodes = _nodes
        };

        return JsonSerializer.Serialize(toSerialise);
    }

    public NeuralNet Spawn(Func<float,float> weightEvolver)
    {
        return new NeuralNet(_nodes, _weights, weightEvolver);
    }
    
    public float[] Activate(float[] input)
    {
        if (input.Length != InputLayer.Length)
        {
            throw new ArgumentException($"Expected input size {InputLayer.Length}, received size {input.Length}");
        }
        
        input.CopyTo(InputLayer, 0);

        for (var i = 1; i < _nodes.Length; i++)
        {
            var prevLayerNodes = _nodes[i - 1];
            var layerNodes = _nodes[i];
            var layerWeights = _weights[i - 1];
            
            for (var j = 0; j < layerWeights.Length; j++)
            {
                var nodeWeights = layerWeights[j];
                
                var sum = 0f;

                for (var k = 0; k < prevLayerNodes.Length; k++)
                {
                    sum += nodeWeights[k] * prevLayerNodes[k];
                }

                layerNodes[j] = MathF.Tanh(sum);
            }
        }

        return OutputLayer;
    }

    private static float[][] InitialiseNodes(int numInputNodes, int numOutputNodes, int numHiddenLayers, int numHiddenLayerNodes)
    {
        var nodes = new float[numHiddenLayers + 2][];
        
        nodes[0] = new float[numInputNodes];
        
        for (var i = 1; i < numHiddenLayers + 1; i++)
        {
            nodes[i] = new float[numHiddenLayerNodes + 1];
            nodes[i][^1] = 1f; // Constant nodes in the hidden layer
        }
        
        nodes[numHiddenLayers + 1] = new float[numOutputNodes];

        return nodes;
    }

    private static float[][][] InitialiseWeights(float[][] nodes)
    {
        var weights = new float[nodes.Length - 1][][];

        for (var i = 0; i < weights.Length; i++)
        {
            var layerSize = nodes[i + 1].Length;

            if (i < weights.Length - 1)
            {
                layerSize -= 1; // Don't need weights for the constant nodes
            }
            
            weights[i] = new float[layerSize][];
            
            var layerWeights = weights[i];
            
            for (var j = 0; j < layerWeights.Length; j++)
            {
                var prevLayerSize = nodes[i].Length;
                layerWeights[j] = new float[prevLayerSize];
                
                var nodeWeights = layerWeights[j];

                for (var k = 0; k < nodeWeights.Length; k++)
                {
                    nodeWeights[k] = Random.NextSingle() * 2 - 1;
                }
            }
        }

        return weights;
    }

    private void Evolve(Func<float,float> weightOperator)
    {
        foreach (var layerWeights in _weights)
        {
            foreach (var nodeWeights in layerWeights)
            {
                for (var i = 0; i < nodeWeights.Length; i++)
                {
                    nodeWeights[i] = weightOperator(nodeWeights[i]);
                }
            }
        }
    }
    
    private class SerialisableNeuralNet
    {
        public float[][] Nodes { get; set; }
        public float[][][] Weights  { get; set; }
    }
}