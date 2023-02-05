using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFragment : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Renderer _rend;

    public void SyncTo(string minedItemId)
    {
        MinedItemData data = CollectionsManager.Instance.GetMinedItemDataById(minedItemId);
        if (data == null)
            return;

        _rend.material.color = data.ItemColor;
    }
}
