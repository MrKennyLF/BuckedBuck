using System;
using UnityEngine;
using Project.Core; // Necesario para conocer IItem

namespace Project.Visuals
{
    // Asegúrate de que tus Prefabs tengan un Collider 3D (ej. BoxCollider) para que el ratón los detecte
    public class InteractableItem : MonoBehaviour
    {
        private IItem _logicalItem;

        // Este evento avisará hacia "arriba" cuando el jugador haga clic en este modelo 3D
        public event Action<IItem> OnItemClicked;

        // El VisualController llama a esto al instanciar el prefab
        public void Initialize(IItem item)
        {
            _logicalItem = item;
        }

        private void OnMouseDown()
        {
            if (_logicalItem != null)
            {
                Debug.Log($"[Vista] Clic en el objeto visual de: {_logicalItem.Name}");
                OnItemClicked?.Invoke(_logicalItem);
            }
        }
    }
}