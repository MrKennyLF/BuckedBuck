using System;
using System.Linq; // Necesario para contar la lista rápido
using UnityEngine;

namespace Project.Core
{
    public class ProbabilisticDealerAI : IDealerAI
    {
        public Target DecideTarget(GameContext context)
        {
            // 1. El Dealer lee la mente de la escopeta (sabe cuántas quedan, pero no el orden)
            int totalRounds = context.ShotgunChamber.Count;

            // Si por alguna razón no hay balas, falla seguro al jugador
            if (totalRounds == 0) return Target.Player;

            int liveRounds = context.ShotgunChamber.Count(bullet => bullet == true);
            int blankRounds = totalRounds - liveRounds;

            Debug.Log($"[IA Dealer] Calculando probabilidades... Vivas: {liveRounds}, Fogueo: {blankRounds}");

            // 2. La lógica de supervivencia
            if (blankRounds > liveRounds)
            {
                // Si la probabilidad dice que es fogueo, se dispara a sí mismo para robarte el turno
                Debug.Log("[IA Dealer] Mayor probabilidad de fogueo. Apuntando a: MÍ MISMO.");
                return Target.Dealer;
            }
            else
            {
                // Si hay más balas vivas, o están empatadas (50/50), es agresivo y te dispara a ti
                Debug.Log("[IA Dealer] Mayor o igual probabilidad de daño. Apuntando a: JUGADOR.");
                return Target.Player;
            }
        }
    }
}