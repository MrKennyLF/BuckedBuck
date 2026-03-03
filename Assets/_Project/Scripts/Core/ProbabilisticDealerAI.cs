using System.Linq;
using UnityEngine;

namespace Project.Core
{
    public class ProbabilisticDealerAI : IDealerAI
    {
        public Target DecideTarget(GameContext context)
        {
            int totalRounds = context.ShotgunChamber.Count;
            if (totalRounds == 0) return Target.None;

            // La IA lee la recámara. En un diseño estricto de seguridad, la IA no debería tener
            // acceso directo a la lista para evitar que haga "trampas" (modificando las balas),
            // pero por ahora usaremos LINQ para contar.
            int liveRounds = context.ShotgunChamber.Count(isLive => isLive);
            int blankRounds = totalRounds - liveRounds;

            // Casos absolutos (Inteligencia 100%)
            if (liveRounds == 0)
            {
                Debug.Log("[Dealer AI] 100% Fogueo. Me disparo a mí mismo para turno extra.");
                return Target.Dealer;
            }
            if (blankRounds == 0)
            {
                Debug.Log("[Dealer AI] 100% Bala viva. Disparo al jugador.");
                return Target.Player;
            }

            // Probabilidad
            float liveProbability = (float)liveRounds / totalRounds;

            if (liveProbability >= 0.5f)
            {
                Debug.Log($"[Dealer AI] Probabilidad de viva ({liveProbability * 100}%). Disparo al jugador.");
                return Target.Player;
            }
            else
            {
                Debug.Log($"[Dealer AI] Probabilidad de fogueo alta. Me arriesgo por el turno extra.");
                return Target.Dealer;
            }
        }
    }
}