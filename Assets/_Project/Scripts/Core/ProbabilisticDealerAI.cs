using System.Linq;
using UnityEngine;

namespace Project.Core
{
    public class ProbabilisticDealerAI : IDealerAI
    {
        // Memoria a corto plazo: ¿Usó la lupa en este turno?
        private bool _knowsNextRound = false;

        public IItem DecideItemToUse(GameContext context)
        {
            // 1. Instinto de supervivencia: Si tiene menos de 4 de vida, busca un cigarro.
            if (context.DealerHealth < 4)
            {
                var cigarette = context.DealerInventory.FirstOrDefault(i => i.Id == "item_cigarette");
                if (cigarette != null) return cigarette;
            }

            // 2. Inteligencia táctica: Si no sabe qué bala sigue (hay mezcla de vivas y fogueo), busca la lupa.
            int live = context.ShotgunChamber.Count(b => b == true);
            int blank = context.ShotgunChamber.Count(b => b == false);

            if (live > 0 && blank > 0 && !_knowsNextRound)
            {
                var magnifier = context.DealerInventory.FirstOrDefault(i => i.Id == "item_magnifier");
                if (magnifier != null)
                {
                    _knowsNextRound = true; // Se guarda el secreto en su memoria
                    return magnifier;
                }
            }

            return null; // Si no tiene nada útil, decide no usar nada.
        }

        public Target DecideTarget(GameContext context)
        {
            // ¡EL DEALER HACE TRAMPA! Si usó la lupa, ya sabe el futuro.
            if (_knowsNextRound)
            {
                _knowsNextRound = false; // Resetea su memoria tras disparar
                bool isLive = context.PeekNextRound(); // Mira qué bala viene

                Debug.Log($"[IA Dealer] He visto el futuro. La bala es {(isLive ? "VIVA" : "FOGUEO")}.");
                return isLive ? Target.Player : Target.Dealer;
            }

            // --- Lógica probabilística que ya teníamos (si no usó la lupa) ---
            int totalRounds = context.ShotgunChamber.Count;
            if (totalRounds == 0) return Target.Player;

            int liveRounds = context.ShotgunChamber.Count(b => b == true);
            int blankRounds = totalRounds - liveRounds;

            Debug.Log($"[IA Dealer] Calculando a ciegas... Vivas: {liveRounds}, Fogueo: {blankRounds}");

            if (blankRounds > liveRounds) return Target.Dealer;
            else return Target.Player;
        }
    }
}