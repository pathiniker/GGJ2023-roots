using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DepthData
{
    public DepthLevel Level;
    public int DepthY;

    public DepthData(DepthLevel level, int y)
    {
        Level = level;
        DepthY = y;
    }
}

public class GridGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int _gridWidth = 10;
    [SerializeField] int _gridHeight = 15;
    [SerializeField] float _ironChance = 0.25f;
    [SerializeField] List<DepthData> _depthLevels = new List<DepthData>();

    [Header("Transforms")]
    [SerializeField] Transform _gridParent;

    [Header("Prefabs")]
    [SerializeField] GridCell _standardCell;
    [SerializeField] List<GridCell> _oreCells = new List<GridCell>();

    Dictionary<DepthLevel, List<GridCell>> _gridDepthDictionary = new Dictionary<DepthLevel, List<GridCell>>();

    private void Awake()
    {
        Initialize();
        GenerateGrid();
    }

    void Initialize()
    {
        foreach (DepthData depth in _depthLevels)
        {
            List<GridCell> cells = new List<GridCell>();
            int levelIdx = (int)depth.Level;

            foreach (GridCell c in _oreCells)
            {
                int cellIdx = (int)c.Data.SpawnLevel;

                if (cellIdx <= levelIdx)
                    cells.Add(c);
            }

            _gridDepthDictionary.TryAdd(depth.Level, cells);
        }
    }

    GridCell GetCellToSpawn(DepthLevel level)
    {
        float chance = Random.Range(0f, 1f);

        if (chance > 0.5f)
        {
            // 50% chance always to just be dirt
            return _standardCell;
        }

        List<GridCell> eligible = new List<GridCell>();
        eligible.Add(_standardCell);

        _gridDepthDictionary.TryGetValue(level, out eligible);

        // Temp
        return eligible[Random.Range(0, eligible.Count)];
    }

    DepthData GetDepthDataForY(int y)
    {
        DepthData result = _depthLevels[0];

        foreach (DepthData data in _depthLevels)
        {
            if (Mathf.Abs(data.DepthY) >= Mathf.Abs(y))
                break;

            result = data;
        }

        return result;
    }

    public DepthLevel GetCurrentDepthLevel()
    {
        // TODO
        // Get current player Y
        // Return Depth Level of greatest value that is less than current player Y

        return DepthLevel.GroundLevel;
    }

    public void GenerateGrid()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            DepthData depthData = GetDepthDataForY(-y);

            for (int x = 0; x < _gridWidth; x++)
            {
                Vector3 pos = new Vector3(x, -y, 0);
                GridCell cell = Instantiate(GetCellToSpawn(depthData.Level), _gridParent);
                cell.transform.position = pos;
            }
        }
    }
}
