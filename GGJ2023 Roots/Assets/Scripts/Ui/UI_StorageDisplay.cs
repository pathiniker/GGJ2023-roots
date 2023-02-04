using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_StorageDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI _capacityText;
    [SerializeField] Transform _itemContainer;

    [Header("Prefabs")]
    [SerializeField] UI_StorageItem _itemPrefab;

    Dictionary<string, UI_StorageItem> _spawnDictionary = new Dictionary<string, UI_StorageItem>();

    private void Start()
    {
        ClearInventory();
    }

    public void ClearInventory()
    {
        foreach (KeyValuePair<string, UI_StorageItem> kvp in _spawnDictionary)
        {
            Destroy(kvp.Value.gameObject);
        }

        _spawnDictionary.Clear();
    }

    public void SyncInventory(Dictionary<string, int> inventory, int totalWeight, int capacity)
    {
        foreach (KeyValuePair<string, int> kvp in inventory)
        {
            if (_spawnDictionary.ContainsKey(kvp.Key))
            {
                _spawnDictionary[kvp.Key].SyncTo(kvp.Key, kvp.Value);
            } else
            {
                UI_StorageItem newItem = Instantiate(_itemPrefab, _itemContainer);
                newItem.SyncTo(kvp.Key, kvp.Value);
                _spawnDictionary.Add(kvp.Key, newItem);
            }
        }

        _capacityText.SetText($"{totalWeight} / {capacity}");
    }
}
