using UnityEngine;
using Project.Core;

namespace Project.Visuals
{
    public class InteractableItem : MonoBehaviour
    {
        private IItem _itemData;
        private InputReader _inputReader;

        public void Initialize(IItem itemData)
        {
            _itemData = itemData;
            // Buscamos el puente de controles en la escena
            _inputReader = FindFirstObjectByType<InputReader>();
        }

        // Esta función de Unity se dispara sola al hacer clic en el objeto
        // (Requiere que el objeto tenga un Collider, que los cubos/cilindros ya traen por defecto)
        private void OnMouseDown()
        {
            if (_itemData != null && _inputReader != null)
            {
                Debug.Log($"[Vista] Clic detectado en: {_itemData.Name}");
                _inputReader.RaiseItemUseInput(_itemData);
            }
        }
    }
}