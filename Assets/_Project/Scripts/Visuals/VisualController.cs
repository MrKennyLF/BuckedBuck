using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Core;

namespace Project.Visuals
{
    public class VisualController : MonoBehaviour
    {
        [Header("Dependencias Visuales")]
        [SerializeField] private ItemVisualCatalog _itemCatalog;

        [Header("Puntos de Aparición (Spawns)")]
        [SerializeField] private Transform _playerTable;
        [SerializeField] private Transform _dealerTable;

        // Tracking O(1) para enlazar la lógica pura con el modelo 3D instanciado
        private Dictionary<IItem, GameObject> _spawnedItems = new Dictionary<IItem, GameObject>();

        // --- GESTIÓN DE INVENTARIO VISUAL ---
        public void HandleItemAdded(TurnOwner owner, IItem itemInstance)
        {
            GameObject prefab = _itemCatalog.GetPrefab(itemInstance.Id);
            if (prefab == null) return;

            Transform targetParent = (owner == TurnOwner.Player) ? _playerTable : _dealerTable;
            GameObject spawnedModel = Instantiate(prefab, targetParent);

            // Guardamos el registro
            _spawnedItems[itemInstance] = spawnedModel;

            var interactable = spawnedModel.GetComponent<InteractableItem>();
            if (interactable != null)
            {
                interactable.Initialize(itemInstance);
            }
        }

        public void HandleItemConsumed(TurnOwner owner, IItem itemInstance)
        {
            if (_spawnedItems.TryGetValue(itemInstance, out GameObject visual))
            {
                var interactable = visual.GetComponent<InteractableItem>();
                if (interactable != null)
                {
                    // Si tuvieras que limpiar suscripciones extra de la vista, irían aquí
                }

                Destroy(visual);
                _spawnedItems.Remove(itemInstance);
            }
            else
            {
                Debug.LogWarning($"[Vista] Intentando destruir un item visual no registrado: {itemInstance.Name}");
            }
        }

        // --- ANIMACIONES DE OBJETOS ---
        public void PlayItemAnimation(string itemId, bool extraData, Action onComplete)
        {
            StartCoroutine(AnimateItemRoutine(itemId, extraData, onComplete));
        }

        private IEnumerator AnimateItemRoutine(string itemId, bool extraData, Action onComplete)
        {
            if (itemId == "item_magnifier")
            {
                Debug.Log("[Vista] Animación de Lupa INICIADA...");
                yield return new WaitForSeconds(2f);

                string tipoBala = extraData ? "VIVA (Roja)" : "FOGUEO (Azul)";
                Debug.Log($"[Vista] RESULTADO LUPA: La bala es {tipoBala}");

                yield return new WaitForSeconds(1f);
                Debug.Log("[Vista] Animación de Lupa TERMINADA.");
            }
            else if (itemId == "item_cigarette")
            {
                Debug.Log("[Vista] Encendiendo cigarrillo... FFFUUUU...");
                yield return new WaitForSeconds(1.5f); // Tiempo para fumar

                Debug.Log("[Vista] Ahhh... +1 de Vida.");
                yield return new WaitForSeconds(0.5f);
            }

            // Avisamos al estado que ya puede reanudar el juego
            onComplete?.Invoke();
        }

        // --- ANIMACIONES DEL NÚCLEO (DISPAROS Y DEALER) ---
        public void AnimateShot(Target target, bool isLive, Action onComplete)
        {
            StartCoroutine(ShotRoutine(target, isLive, onComplete));
        }

        private IEnumerator ShotRoutine(Target target, bool isLive, Action onComplete)
        {
            Debug.Log($"[Vista] POW! Disparando a {target}... ¿Viva?: {isLive}");
            yield return new WaitForSeconds(1.5f);
            onComplete?.Invoke();
        }

        public void SimulateDealerThinking(Action onComplete)
        {
            StartCoroutine(DealerThinkingRoutine(onComplete));
        }

        private IEnumerator DealerThinkingRoutine(Action onComplete)
        {
            Debug.Log("[Vista] El Dealer está decidiendo su jugada...");
            yield return new WaitForSeconds(2f);
            onComplete?.Invoke();
        }
    }
}