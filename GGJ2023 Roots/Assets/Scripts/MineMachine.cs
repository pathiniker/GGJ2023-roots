using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MineMachine : MonoBehaviour
{
    public const int MAX_DRILL_LEVEL = 3;

    [Header("Settings")]
    [SerializeField] float _baseMoveSpeed = 200f;
    [SerializeField] float _baseBoosterForce = 200f;
    [SerializeField] float _hitDelay = 0.2f;
    [SerializeField] float _baseMineStrength = 1f;
    [SerializeField] float _baseFuelCapacity = 100f;
    [SerializeField] int _baseStorageCapacity = 25;

    [Header("Fuel Settings")]
    [SerializeField] float _baseFuelBurnRate = 0.05f;
    [SerializeField] float _refuelRate = 0.2f;

    [Header("Machine Parts")]
    [SerializeField] GameObject _drillAnchor;
    [SerializeField] GameObject _drillTip;
    [SerializeField] List<GameObject> _wheels = new List<GameObject>();
    [SerializeField] GameObject _boostersParent;
    [SerializeField] Transform _sparkSpawnPoint;

    [Header("Components")]
    [SerializeField] Collider _collider;
    [SerializeField] UI_UpgradesDisplay _upgradesDisplay;

    [Header("FX Prefabs")]
    [SerializeField] GameObject _sparkPrefab;
    [SerializeField] GameObject _deathPrefab;

    bool _isRefueling = false;
    bool _shouldReRollUpgrades = true;
    float _timeSinceLastHit = 0f;
    Vector2 _bounds;
    int _currency = 0;
    bool _isMining = false;
    MineDirection _currentDirection;

    [Header("Run Time Miner Stats")]
    public int DrillLevel = 1;
    public float MoveSpeedMultiplier = 1f;
    public float BoosterForceMultiplier = 1f;
    public float CurrentFuelCapacity;
    public int CurrentStorageCapacity;

    public float RemainingFuel;
    public bool ShouldNotifyFuel = false;
    public bool HasMinedGold = false;
    public bool CanMove = true;

    Dictionary<string, int> _inventory = new Dictionary<string, int>();

    public int Currency { get { return _currency; } }

    private void Start()
    {
        _bounds = _collider.bounds.extents;
        CurrentFuelCapacity = _baseFuelCapacity;
        CurrentStorageCapacity = _baseStorageCapacity;
        RemainingFuel = CurrentFuelCapacity * 0.25f;
        _upgradesDisplay.Show(false);
        ShouldNotifyFuel = false;

        ToggleBoosters(false);
        UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
        UiController.Instance.SyncStorageDisplay(_inventory, GetCurrentWeight(), CurrentStorageCapacity);
    }

    public bool CanAfford(int requestedAmount)
    {
        return _currency >= requestedAmount;
    }

    public int GetCurrentWeight()
    {
        int total = 0;

        foreach (int value in _inventory.Values)
        {
            total += value;
        }

        return total;
    }

    public float GetMoveSpeed()
    {
        return _baseMoveSpeed * MoveSpeedMultiplier;
    }

    public float GetBoosterForce()
    {
        // Booster is weaker depending on storage!
        float storageFill = GetCurrentWeight() / CurrentStorageCapacity;
        float storageDrag = Mathf.Lerp(1f, 0.75f, storageFill);
        return _baseBoosterForce * BoosterForceMultiplier * storageDrag;
    }

    public float GetMineStrength()
    {
        return _baseMineStrength * DrillLevel;
    }

    public float GetNormalizedFuelLevel()
    {
        return RemainingFuel / CurrentFuelCapacity;
    }

    public float GetBurnFuelRate()
    {
        return _baseFuelBurnRate;
    }

    GridCell GetContactCell(MineDirection direction)
    {
        if (direction == MineDirection.NONE)
            return null;

        float checkDistance = 0f;
        float contactBufferDistance = 0.35f;
        Vector3 vectorDirection = Vector3.zero;

        switch (direction)
        {
            case MineDirection.Right:
                checkDistance = _bounds.x;
                vectorDirection = Vector3.right;
                break;

            case MineDirection.Left:
                checkDistance = _bounds.x;
                vectorDirection = Vector3.left;
                break;

            case MineDirection.Up:
                checkDistance = _bounds.y;
                vectorDirection = Vector3.up;
                contactBufferDistance = 0.5f;
                break;

            case MineDirection.Down:
                checkDistance = _bounds.y;
                vectorDirection = Vector3.down;
                contactBufferDistance = 0.5f;
                break;
        }

        RaycastHit hit;
        float hitDistance = checkDistance + contactBufferDistance;
        if (Physics.Raycast(transform.position, vectorDirection, out hit, hitDistance))
        {
            Debug.DrawRay(transform.position, vectorDirection * hitDistance, Color.yellow);
            GridCell cell = hit.collider.GetComponent<GridCell>();

            if (cell != null)
            {
                //Debug.Log($"Hit cell: {cell.name}");
                return cell;
            }
        }

        return null;
    }

    EvilTree GetEvilTreeContact(MineDirection direction)
    {
        if (direction == MineDirection.NONE)
            return null;

        float checkDistance = 0f;
        float contactBufferDistance = 0.35f;
        Vector3 vectorDirection = Vector3.zero;

        switch (direction)
        {
            case MineDirection.Right:
                checkDistance = _bounds.x;
                vectorDirection = Vector3.right;
                break;

            case MineDirection.Left:
                checkDistance = _bounds.x;
                vectorDirection = Vector3.left;
                break;

            case MineDirection.Up:
                checkDistance = _bounds.y;
                vectorDirection = Vector3.up;
                contactBufferDistance = 0.5f;
                break;

            case MineDirection.Down:
                checkDistance = _bounds.y;
                vectorDirection = Vector3.down;
                contactBufferDistance = 0.5f;
                break;
        }

        RaycastHit hit;
        float hitDistance = checkDistance + contactBufferDistance;
        if (Physics.Raycast(transform.position, vectorDirection, out hit, hitDistance))
        {
            Debug.DrawRay(transform.position, vectorDirection * hitDistance, Color.yellow);
            EvilTree tree = hit.collider.GetComponentInParent<EvilTree>();

            if (tree != null)
            {
                //Debug.Log($"Hit cell: {cell.name}");
                return tree;
            }
        }

        return null;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _bounds.y + 0.5f);
    }

    public void Refuel(bool doInstant = false, bool showText = false)
    {
        if (_isRefueling)
            return;

        if (doInstant)
        {
            RemainingFuel = CurrentFuelCapacity;
            UiController.Instance.SetFuelDisplay(1f);
        } else
        {
            StartCoroutine(DoRefuel(showText));
        }
    }

    public void AddItemToInventory(string id, int qty = 1)
    {
        if (string.IsNullOrEmpty(id))
            return;

        int availableCapacity = CurrentStorageCapacity - GetCurrentWeight();
        if (availableCapacity - qty < 0)
            qty = availableCapacity;

        if (GetCurrentWeight() >= CurrentStorageCapacity)
        {
            StoryController.Instance.DisplayStorageFullWarning();
            return;
        }

        StoryController.Instance.DisplayText($"+ {qty} {id}");

        if (_inventory.ContainsKey(id))
        {
            _inventory[id] += qty;
        } else
        {
            _inventory.Add(id, qty);
        }

        if (GetCurrentWeight() >= CurrentStorageCapacity)
        {
            StoryController.Instance.DisplayStorageFullWarning();
        }

        UiController.Instance.SyncStorageDisplay(_inventory, GetCurrentWeight(), CurrentStorageCapacity);
    }

    public void ShowUpgradesDisplay(bool show)
    {
        // Decide if we should reroll
        if (_shouldReRollUpgrades)
        {
            _upgradesDisplay.ReRollOptions();
            _shouldReRollUpgrades = false;
        }

        _upgradesDisplay.Show(show);
    }
    
    public void SellInventory()
    {
        int currencyToGain = 0;

        foreach (KeyValuePair<string, int> item in _inventory)
        {
            MinedItemData data = CollectionsManager.Instance.GetMinedItemDataById(item.Key);
            if (data == null)
                continue;

            currencyToGain += data.CurrencyValue * item.Value;

            if (data.ItemId == "Tree Heart")
            {
                // TRIGGER BATTLE
                StoryController.Instance.TriggerEndGame();
                _inventory.Clear();
                return;
            }
            // TODO:
            // Animate each sell item
        }

        _currency += currencyToGain;
        //Debug.Log($"Gain {currencyToGain} coins");

        if (currencyToGain > 0)
        {
            _shouldReRollUpgrades = true;
            StoryController.Instance.DisplayCoinsGained(currencyToGain);
        } else
        {
            string nothingText = "Come back with something to sell.";
            //if (_currency <= 0)
            //    nothingText = "... nothing to buy.";

            StoryController.Instance.DisplayText(nothingText);
        }

        UiController.Instance.SyncCurrencyDisplay(_currency);
        UiController.Instance.SetStorageCapacity(0, CurrentStorageCapacity);
        _inventory.Clear();
    }

    public void UpgradeDrill()
    {
        DrillLevel += 1;
        StoryController.Instance.DisplayText($"Drill upgraded to level 2.");
    }

    public void UpgradeFuel()
    {
        CurrentFuelCapacity *= 2;
        UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
        StoryController.Instance.DisplayText($"Doubled fuel capacity.");
    }

    public void UpgradeStorage()
    {
        CurrentStorageCapacity *= 2;
        UiController.Instance.SetStorageCapacity(GetCurrentWeight(), CurrentStorageCapacity);
        StoryController.Instance.DisplayText($"Doubled storage capacity.");
    }

    public void StopRefuel()
    {
        _isRefueling = false;
    }

    public void SpendCurrency(int amount, System.Action onCompletePurchaseCb = null)
    {
        if (!CanAfford(amount))
        {
            // TODO: Notify user can't afford
            return;
        }

        _currency -= amount;
        UiController.Instance.SyncCurrencyDisplay(_currency);
        onCompletePurchaseCb?.Invoke();
    }

    IEnumerator DoRefuel(bool showText = false)
    {
        _isRefueling = true;
        float rateMultiplier = 1f;
       
        while (RemainingFuel <= CurrentFuelCapacity)
        {
            if (!_isRefueling)
                break;

            RemainingFuel += _refuelRate * rateMultiplier;
            UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
            rateMultiplier *= 1.05f;
            yield return new WaitForSeconds(0.01f);
            yield return null;
        }

        if (showText)
            StoryController.Instance.DisplayText("Refueled.");

        ShouldNotifyFuel = true;

        yield break;
    }

    public void BurnFuel(float amount)
    {
        RemainingFuel -= amount;
        float normalizedLevel = GetNormalizedFuelLevel();
        UiController.Instance.SetFuelDisplay(normalizedLevel);

        if (normalizedLevel <= 0.25f && ShouldNotifyFuel)
        {
            ShouldNotifyFuel = false;
            StoryController.Instance.DisplayText($"Fuel getting low... Turn back soon.");
        }
    }

    void TryMineTree()
    {
        // Detect if hitting tree
        EvilTree tree = GetEvilTreeContact(_currentDirection);

        if (tree == null)
            return;

        float spinRate = 0.1f;
        float spinMultiplyer = 20f;
        Vector3 spin = Vector3.back * spinMultiplyer;
        _drillTip.transform.DOBlendableLocalRotateBy(spin, spinRate, RotateMode.LocalAxisAdd);

        if (_timeSinceLastHit < _hitDelay)
            return;

        bool doSpark = Random.Range(0f, 1f) < 0.45f;

        if (doSpark)
            Instantiate(_sparkPrefab, _sparkSpawnPoint); // Destroys self

        // MINE!
        //Debug.LogWarning($"MINE CELL: {cell.name}");
        _timeSinceLastHit = 0f;
        float dmg = GetMineStrength();
        tree.DealDamage(dmg);
    }

    public void TryMine()
    {
        if (StoryController.Instance.BeganEndGame)
        {
            TryMineTree();
            return;
        }

        // TODO: Check block based on current mine direction
        GridCell cell = GetContactCell(_currentDirection);

        _isMining = cell != null;

        if (cell == null)
        {
            return;
        }

        MinedItemData data = CollectionsManager.Instance.GetMinedItemDataById(cell.Data.MinedItemId);

        if (data != null)
        {
            if (data.RequiredDrillLevel > DrillLevel)
            {
                StoryController.Instance.DisplayText($"A better drill is required.");
                return;
            }
        }

        float spinRate = 0.1f;
        float spinMultiplyer = 20f;
        Vector3 spin = Vector3.back * spinMultiplyer;
        _drillTip.transform.DOBlendableLocalRotateBy(spin, spinRate, RotateMode.LocalAxisAdd);

        if (_timeSinceLastHit < _hitDelay)
            return;

        if (!string.IsNullOrEmpty(cell.Data.MinedItemId))
        {
            bool doSpark = Random.Range(0f, 1f) < 0.45f;

            if (doSpark)
                Instantiate(_sparkPrefab, _sparkSpawnPoint); // Destroys self
        }
        
        // MINE!
        //Debug.LogWarning($"MINE CELL: {cell.name}");
        _timeSinceLastHit = 0f;
        float dmg = GetMineStrength();
        cell.DealDamage(dmg);
        //Destroy(cell.gameObject);
    }

    public void DoDeathSequence()
    {
        Instantiate(_deathPrefab, transform);
    }

    public void SetDirection(MineDirection direction)
    {
        _currentDirection = direction;

        // TODO: Animate drill direction
        float rotate = direction switch
        {
            MineDirection.Left => -180f,
            MineDirection.Right => 0f,
            MineDirection.Down => -90f,
            MineDirection.Up => 90f,
            _ => 90f
        };

        float rotateTime = 0.25f;
        Vector3 toRotate = new Vector3(rotate, 0, 0);
        _drillAnchor.transform.DOLocalRotate(toRotate, rotateTime, RotateMode.Fast);
    }

    public void DoWheelRotation(MineDirection direction, bool isGrounded)
    {
        float rotValue = 5f * MoveSpeedMultiplier;

        if (!isGrounded)
            rotValue *= 2f;
        else if (_isMining)
            rotValue *= 0.1f;

        float engineMultiplyer = isGrounded ? 1f : 1.8f;
        AudioManager.Instance.NotifyEngineMoving(true, engineMultiplyer);

        for (int i = 0; i < _wheels.Count; i++)
        {
            GameObject o = _wheels[i];
            float x = direction == MineDirection.Left ? rotValue : -rotValue;
            Vector3 rot = new Vector3(x, 0, 0);
            o.transform.DOBlendableLocalRotateBy(rot, 0.1f);
        }
    }

    public void ToggleBoosters(bool on)
    {
        _boostersParent.SetActive(on);
    }

    private void Update()
    {
        _timeSinceLastHit += Time.deltaTime;
    }
}
