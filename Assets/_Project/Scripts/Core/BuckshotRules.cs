using System;

namespace Project.Core
{
    public class BuckshotRules : ITurnRuleEvaluator
    {
        public Type EvaluateNextTurn(GameContext context, bool wasLiveRound)
        {
            // 1. Condición de fin de juego
            if (context.PlayerHealth <= 0 || context.DealerHealth <= 0)
            {
                // Devolvemos null temporalmente hasta que creemos el GameOverState
                return null;
            }

            // 2. Condición de recarga
            if (context.ShotgunChamber.Count == 0)
            {
                return typeof(SetupRoundState);
            }

            // 3. Reglas de retención de turno (La mecánica principal)
            if (context.CurrentTurnOwner == TurnOwner.Player)
            {
                if (context.CurrentTarget == Target.Player && !wasLiveRound)
                    return typeof(PlayerTurnState); // Retiene turno

                return typeof(DealerTurnState); // Pierde turno
            }
            else if (context.CurrentTurnOwner == TurnOwner.Dealer)
            {
                if (context.CurrentTarget == Target.Dealer && !wasLiveRound)
                    return typeof(DealerTurnState); // Retiene turno

                return typeof(PlayerTurnState); // Pierde turno
            }

            return typeof(SetupRoundState); // Fallback de seguridad
        }
    }
}