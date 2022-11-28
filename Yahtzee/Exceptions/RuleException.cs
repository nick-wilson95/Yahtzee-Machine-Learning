namespace Yahtzee.Exceptions;

public class RuleException : Exception
{
    public RuleException(string message) : base(message)
    {
    }
}