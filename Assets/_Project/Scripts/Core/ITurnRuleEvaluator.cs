using System;

namespace Project.Core
{
    public interface ITurnRuleEvaluator
    {
        Type EvaluateNextTurn(GameContext context, bool wasLiveRound);
    }
}