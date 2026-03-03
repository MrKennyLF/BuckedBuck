using System;
using UnityEngine;

namespace Project.Core
{
    public class MagnifyingGlassItem : IItem
    {
        public string Name => "Magnifying Glass";

        // Evento que escuchará la UI o el VisualController para mostrar el cartucho
        public event Action<bool> OnRoundRevealed;

        public void Use(GameContext context, Action onComplete)
        {
            if (context.ShotgunChamber.Count > 0)
            {
                bool isLive = context.PeekNextRound();
                Debug.Log($"[Item] Lupa usada. El cartucho es {(isLive ? "REAL" : "FOGUEO")}.");

                // Avisamos a la capa visual
                OnRoundRevealed?.Invoke(isLive);
            }
            else
            {
                Debug.LogWarning("[Item] Escopeta vacía. La lupa no revela nada.");
            }

            // Simulamos que el uso es instantáneo a nivel lógico, pero la vista tomará su tiempo
            // Devolvemos el control inmediatamente al estado
            onComplete?.Invoke();
        }
    }
}