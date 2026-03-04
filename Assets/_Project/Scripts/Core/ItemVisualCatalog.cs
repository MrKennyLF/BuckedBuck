using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item Visual Catalog")]
public class ItemVisualCatalog : ScriptableObject, ISerializationCallbackReceiver
{
    [Serializable]
    public struct ItemEntry
    {
        public string id;
        public GameObject prefab;
    }

    [SerializeField] private List<ItemEntry> items = new List<ItemEntry>();

    // El diccionario no se serializa
    private Dictionary<string, GameObject> _lookup = new Dictionary<string, GameObject>();

    // Se ejecuta automáticamente antes de serializar (guardar en disco) - No lo necesitamos aquí
    public void OnBeforeSerialize() { }

    // Se ejecuta automáticamente DESPUÉS de que Unity carga la lista en memoria
    public void OnAfterDeserialize()
    {
        _lookup.Clear();
        foreach (var entry in items)
        {
            if (string.IsNullOrWhiteSpace(entry.id)) continue;

            if (!_lookup.ContainsKey(entry.id))
            {
                _lookup.Add(entry.id, entry.prefab);
            }
        }
    }

    public GameObject GetPrefab(string id)
    {
        if (_lookup.TryGetValue(id, out var prefab))
            return prefab;

        Debug.LogError($"[Catálogo] No existe prefab para ID '{id}'");
        return null;
    }
}