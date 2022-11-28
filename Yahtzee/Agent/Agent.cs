using Yahtzee.Game;

namespace Yahtzee.Agent;

/// <summary>
/// Agents are expected to know the rules
/// </summary>
public interface IAgent
{
    PlayOutput Play(GameContext input);
}