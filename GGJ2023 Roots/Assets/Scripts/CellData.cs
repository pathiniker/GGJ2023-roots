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
}
