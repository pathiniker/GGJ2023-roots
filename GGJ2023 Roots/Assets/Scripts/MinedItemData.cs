using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinedItemData
{
    public string ItemId;
    public float Weight = 1f;
    public int CurrencyValue = 1;
    public Color ItemColor = Color.white;
    public int RequiredDrillLevel = 1;
}
