using System;
using UnityEngine;

namespace Project.Core
{
    public class BuckshotRules : ITurnRuleEvaluator
    {
        public Type EvaluateNextTurn(GameContext context, bool wasLiveRound)
        {
            // 1. REGLA DE MUERTE: ¿Alguien se quedó sin vida?
            if (context.PlayerHealth <= 0 || context.DealerHealth <= 0)
            {
                return typeof(GameOverState);
            }

            // 2. REGLA DE RECARGA: ¿Se vació la escopeta?
            if (context.ShotgunChamber.Count == 0)
            {
                Debug.Log("<color=yellow>[Reglas] Escopeta vacía. Volviendo a fase de preparación.</color>");
                return typeof(SetupRoundState); // AQUÍ es el único lugar donde se reinician los objetos
            }

            // 3. REGLA DE TURNOS: Si aún hay balas, decidimos quién sigue
            Target selfTarget = context.CurrentTurnOwner == TurnOwner.Player ? Target.Player : Target.Dealer;

            if (!wasLiveRound && context.CurrentTarget == selfTarget)
            {
                // Si te disparaste a ti mismo con fogueo, CONSERVAS TU TURNO
                Debug.Log("[Reglas] Disparo de fogueo a sí mismo. El jugador conserva el turno.");
                return context.CurrentTurnOwner == TurnOwner.Player ? typeof(PlayerTurnState) : typeof(DealerTurnState);
            }

            // En cualquier otro caso (bala viva, o dispararle al rival), el turno cambia
            Debug.Log("[Reglas] Cambio de turno.");
            return context.CurrentTurnOwner == TurnOwner.Player ? typeof(DealerTurnState) : typeof(PlayerTurnState);
        }
    }
}