using System;
using UnityEngine; // Solo para el Mathf.Min

namespace Project.Core
{
    public class CigaretteItem : IItem
    {
        public string Id => "item_cigarette";
        public string Name => "Cigarette";

        public void Use(GameContext context, Action onComplete)
        {
            int maxHealth = 4; // Tu límite de salud

            // Lógica: Recuperamos 1 punto de vida al dueño del turno actual
            if (context.CurrentTurnOwner == TurnOwner.Player)
            {
                context.PlayerHealth = Mathf.Min(context.PlayerHealth + 1, maxHealth);
                Debug.Log($"[Lógica] Jugador se cura. Salud actual: {context.PlayerHealth}");
            }
            else if (context.CurrentTurnOwner == TurnOwner.Dealer)
            {
                context.DealerHealth = Mathf.Min(context.DealerHealth + 1, maxHealth);
                Debug.Log($"[Lógica] Dealer se cura. Salud actual: {context.DealerHealth}");
            }

            // Le pedimos a la vista que haga la animación de fumar
            // El 'false' es porque este objeto no necesita datos extra como la lupa
            context.RequestItemAnimation(this.Id, false, onComplete);
        }
    }
}