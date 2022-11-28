namespace NeuralNetwork;

public interface ISerialiseable<T>
{
    static abstract T Deserialise(string toDeserialise);
    string Serialise();
}