using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int _gridWidth = 10;
    [SerializeField] int _gridHeight = 15;

    [Header("Transforms")]
    [SerializeField] Transform _gridParent;

    [Header("Prefabs")]
    [SerializeField] GridCell _standardCell;
    [SerializeField] GridCell _ironCell;

    private void Start()
    {
        GenerateGrid();
    }

    GridCell GetCellToSpawn()
    {
        float ironSpawnChance = 0.25f;

        float chance = Random.Range(0f, 1f);

        if (chance <= ironSpawnChance)
        {
            return _ironCell;
        }

        return _standardCell;
    }

    public void GenerateGrid()
    {
        for (int h = 0; h < _gridHeight; h++)
        {
            for (int w = 0; w < _gridWidth; w++)
            {
                Vector3 pos = new Vector3(w, -h, 0);
                GridCell cell = Instantiate(GetCellToSpawn(), _gridParent);
                cell.transform.position = pos;
            }
        }
    }
}
