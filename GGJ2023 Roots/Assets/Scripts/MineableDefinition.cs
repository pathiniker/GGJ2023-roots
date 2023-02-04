using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Mineable Definition")]
public class MineableDefinition : SerializedScriptableObject
{
    public MinedItemData Data;
}
