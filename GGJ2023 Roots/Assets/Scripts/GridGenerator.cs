using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[System.Serializable]
public class DepthData
{
    public DepthLevel Level;
    public int DepthY;
    public GridCell GroundCellPrefab;
    public string LevelName;
}

public class GridGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int _gridWidth = 10;
    [SerializeField] int _gridHeight = 15;
    [SerializeField, PropertyRange(0, 1)] float _dirtChance = 0.67f;
    [SerializeField] List<DepthData> _depthLevels = new List<DepthData>();

    [Header("Transforms")]
    [SerializeField] Transform _gridParent;

    [Header("Prefabs")]
    [SerializeField] GridCell _standardCell;
    [SerializeField] GridCell _heartCell;
    [SerializeField] List<GridCell> _oreCells = new List<GridCell>();
    [SerializeField] List<RockFragment> _fragmentPrefabs = new List<RockFragment>();

    bool _spawnedHeart = false;
    float _heartSpawnChance = 0.05f;
    int _maxHeartSpawnTries = 20;
    int _heartSpawnTries = 0;
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

    GridCell GetCellToSpawn(DepthData data, bool lastWasNull)
    {
        float chance = Random.Range(0f, 1f);

        if (chance <= _dirtChance)
        {
            return data.GroundCellPrefab;
        }

        List<GridCell> eligible = new List<GridCell>();
        _gridDepthDictionary.TryGetValue(data.Level, out eligible);

        if (data.Level == DepthLevel.TierFive)
        {
            if (!_spawnedHeart)
            {
                float heartRoll = Random.Range(0f, 1f);
                if (heartRoll <= _heartSpawnChance)
                {
                    return _heartCell;
                } else
                {
                    if (_heartSpawnTries > _maxHeartSpawnTries)
                        return _heartCell;

                    _heartSpawnChance *= 2f;
                    _heartSpawnTries++;
                }
            }
        }

        if (eligible.Count == 0)
            return data.GroundCellPrefab;

        float emptyChance = 0.08f;
        if (lastWasNull)
            emptyChance *= 3f;

        if (chance >= 1f - emptyChance)
            return null;

        // Temp
        return eligible[Random.Range(0, eligible.Count)];
    }

    public DepthData GetDepthDataForY(int y)
    {
        DepthData result = _depthLevels[0];

        if (y > 0)
            return result;

        foreach (DepthData data in _depthLevels)
        {
            if (Mathf.Abs(data.DepthY) >= Mathf.Abs(y))
                break;

            result = data;
        }

        return result;
    }

    public List<RockFragment> GetRockFragmentPrefabs(int count)
    {
        List<RockFragment> prefabs = new List<RockFragment>();

        if (_fragmentPrefabs.Count == 0)
            return prefabs;

        for (int i = 0; i < count; i++)
        {
            prefabs.Add(_fragmentPrefabs[Random.Range(0, _fragmentPrefabs.Count)]);
        }

        return prefabs;
    }

    public void GenerateGrid()
    {
        int xOffset = -5;

        for (int y = 0; y < _gridHeight; y++)
        {
            DepthData depthData = GetDepthDataForY(-y);
            bool lastWasNull = false;

            for (int x = 0; x < _gridWidth; x++)
            {
                Vector3 pos = new Vector3(x + xOffset, -y, 0);
                GridCell prefab = GetCellToSpawn(depthData, lastWasNull);

                if (prefab != null)
                {
                    if (prefab.Data.MinedItemId == "Tree Heart")
                    {
                        _spawnedHeart = true;
                    }
                }

                lastWasNull = prefab == null;
                if (prefab == null)
                    continue;

                GridCell cell = Instantiate(prefab, _gridParent);
                cell.transform.position = pos;
            }
        }

        Debug.Log($"Generated grid <i>({_gridWidth} x {_gridHeight})</i>");
    }
}
