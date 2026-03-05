using System;
using UnityEngine;

namespace Project.Core
{
    public class BeerItem : IItem
    {
        public string Id => "item_beer";
        public string Name => "Cerveza";

        public void Use(GameContext context, Action onComplete)
        {
            // Sacamos la bala pero NO hacemos daño
            bool isLive = context.ExtractNextRound();
            string tipoBala = isLive ? "VIVA" : "FOGUEO";

            Debug.Log($"<color=cyan>[Objeto Cerveza]</color> Has expulsado una bala de {tipoBala} sin disparar.");

            // Llamamos a la animación visual (pasará al instante si aún no hay modelos)
            context.RequestItemAnimation(Id, isLive, onComplete);
        }
    }
}