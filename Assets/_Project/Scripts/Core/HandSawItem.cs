using System;
using UnityEngine;

namespace Project.Core
{
    public class HandSawItem : IItem
    {
        public string Id => "item_handsaw";
        public string Name => "Sierra";

        public void Use(GameContext context, Action onComplete)
        {
            Debug.Log("<color=cyan>[Objeto Sierra]</color> Has recortado el cañón. ¡El siguiente tiro hace daño doble!");

            // Alteramos el estado central del juego
            context.CurrentDamageMultiplier = 2;

            context.RequestItemAnimation(Id, true, onComplete);
        }
    }
}