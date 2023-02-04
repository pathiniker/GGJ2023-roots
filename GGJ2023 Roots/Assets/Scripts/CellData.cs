using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CellData
{
    public float SpawnChance = 0f;
    public DepthLevel SpawnLevel;
    public int StartingHealth = 1;

    [ValueDropdown("PopulateIdDropdown")]
    public string MinedItemId;

#if UNITY_EDITOR
    IEnumerable PopulateIdDropdown()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t: MineableDefinition");

        List<string> ids = new List<string>();

        foreach (string g in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
            MineableDefinition def = UnityEditor.AssetDatabase.LoadAssetAtPath<MineableDefinition>(path);
            if (def != null)
                ids.Add(def.Data.ItemId);
        }

        return ids;
    }
#endif
}
