using System;
using UnityEngine;
using UnityEngine.InputSystem; // <-- Añadimos la librería nueva

namespace Project.Core
{
    public class InputReader : MonoBehaviour
    {
        public event Action<IItem> OnItemUseRequested;
        public event Action<Target> OnShootRequested;

        public void RaiseItemUseInput(IItem itemInstance)
        {
            OnItemUseRequested?.Invoke(itemInstance);
        }

        private void Update()
        {
            // Verificamos que haya un teclado conectado para evitar errores
            if (Keyboard.current == null) return;

            // Usamos la sintaxis del Nuevo Input System
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Debug.Log("[Input] Tecla W presionada: Disparar al Dealer");
                OnShootRequested?.Invoke(Target.Dealer);
            }
            else if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                Debug.Log("[Input] Tecla S presionada: Disparar a Mí Mismo");
                OnShootRequested?.Invoke(Target.Player);
            }
        }
    }
}