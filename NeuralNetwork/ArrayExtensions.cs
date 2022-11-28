namespace NeuralNetwork;

public static class ArrayExtensions
{
    private static float[] Copy(this float[] toCopy)
    {
        var copy = new float[toCopy.Length];
        toCopy.CopyTo(copy, 0);
        return copy;
    }
    
    public static float[][] DeepCopy(this float[][] toCopy)
    {
        var copy = new float[toCopy.Length][];

        for (var i = 0; i < toCopy.Length; i++)
        {
            copy[i] = toCopy[i].Copy();
        }

        return copy;
    }
    
    public static float[][][] DeepCopy(this float[][][] toCopy)
    {
        var copy = new float[toCopy.Length][][];

        for (var i = 0; i < toCopy.Length; i++)
        {
            copy[i] = toCopy[i].DeepCopy();
        }

        return copy;
    }
}