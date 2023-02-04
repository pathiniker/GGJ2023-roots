using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionsManager : MonoBehaviour
{
    static public CollectionsManager Instance;

    [Header("Collections")]
    [SerializeField] List<MineableDefinition> _minedItemDefinitions = new List<MineableDefinition>();

    Dictionary<string, MinedItemData> _minedItemDictionary = new Dictionary<string, MinedItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }

        InitializeCollections();
    }

    public MinedItemData GetMinedItemDataById(string id)
    {
        _minedItemDictionary.TryGetValue(id, out MinedItemData data);
        Debug.Assert(data != null, $"Failed to find MinedItemData by ID: {id}");
        return data;
    }

    public void InitializeCollections()
    {
        foreach (MineableDefinition def in _minedItemDefinitions)
        {
            if (def == null)
                continue;

            _minedItemDictionary.TryAdd(def.Data.ItemId, def.Data);
        }
    }
}
