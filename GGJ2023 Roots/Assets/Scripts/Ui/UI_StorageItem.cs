using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_StorageItem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI _idText;
    [SerializeField] TextMeshProUGUI _qtyText;

    public void SyncTo(string id, int qty)
    {
        _idText.SetText(id);
        _qtyText.SetText(qty.ToString());
    }
}
