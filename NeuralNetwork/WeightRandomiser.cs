namespace NeuralNetwork;

public static class Randomiser
{
    private static readonly Random Random = new();
    
    public static float Randomise(float initialValue)
    {
        var random = Random.NextSingle();

        var perturbation = Random.NextSingle();

        return random switch
        {
            < 0.02f => initialValue + perturbation,
            < 0.04f => initialValue - perturbation,
            < 0.06f => 0f,
            < 0.07f => initialValue * -1f,
            _ => initialValue
        };
    }
}