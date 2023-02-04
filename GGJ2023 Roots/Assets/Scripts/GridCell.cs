using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] CellData _cellData;

    public CellData Data { get { return _cellData; } }
}
