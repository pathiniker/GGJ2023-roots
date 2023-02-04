using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DepthLevel
{
    GroundLevel = 0,
    TierOne = 1,
    TierTwo = 2,
    TierThree = 3,
    TierFour = 4,
    TierFive = 5
}

public class GridCell : MonoBehaviour
{
    [SerializeField] CellData _cellData;

    public CellData Data { get { return _cellData; } }
}
