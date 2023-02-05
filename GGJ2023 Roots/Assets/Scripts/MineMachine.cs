using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MineMachine : MonoBehaviour
{
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

    bool _isRefueling = false;
    bool _shouldReRollUpgrades = true;
    float _timeSinceLastHit = 0f;
    Vector2 _bounds;
    int _currency = 0;
    bool _isMining = false;
    MineDirection _currentDirection;

    [Header("Run Time Miner Stats")]
    public float MoveSpeedMultiplier = 1f;
    public float BoosterForceMultiplier = 1f;
    public float MineStrengthMultiplier = 1f;
    public float CurrentFuelCapacity;
    public int CurrentStorageCapacity;

    public float RemainingFuel;

    Dictionary<string, int> _inventory = new Dictionary<string, int>();

    private void Start()
    {
        _bounds = _collider.bounds.extents;
        CurrentFuelCapacity = _baseFuelCapacity;
        CurrentStorageCapacity = _baseStorageCapacity;
        RemainingFuel = CurrentFuelCapacity;
        _upgradesDisplay.Show(false);

        ToggleBoosters(false);
        UiController.Instance.SetFuelDisplay(1f);
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
        float storageDrag = Mathf.Lerp(1f, 0.5f, storageFill);
        return _baseBoosterForce * BoosterForceMultiplier * storageDrag;
    }

    public float GetMineStrength()
    {
        return _baseMineStrength * MineStrengthMultiplier;
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

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _bounds.y + 0.5f);
    }

    public void Refuel(bool doInstant = false)
    {
        if (_isRefueling)
            return;

        //RemainingFuel = CurrentFuelCapacity;
        //UiController.Instance.SetFuelDisplay(1f);

        if (doInstant)
        {
            RemainingFuel = CurrentFuelCapacity;
            UiController.Instance.SetFuelDisplay(1f);
        } else
        {
            StartCoroutine(DoRefuel());
        }
    }

    public void AddItemToInventory(string id, int qty = 1)
    {
        if (string.IsNullOrEmpty(id))
            return;

        if (GetCurrentWeight() >= CurrentStorageCapacity)
        {
            // TODO: Alert storage is full!
            Debug.LogWarning("Storage full! (Create UI)");
            return;
        }

        if (_inventory.ContainsKey(id))
        {
            _inventory[id] += qty;
        } else
        {
            _inventory.Add(id, qty);
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

            currencyToGain += data.CurrencyValue;

            // TODO:
            // Animate each sell item
        }

        _currency += currencyToGain;
        //Debug.Log($"Gain {currencyToGain} coins");

        if (currencyToGain > 0)
            _shouldReRollUpgrades = true;

        UiController.Instance.SyncCurrencyDisplay(_currency);
        UiController.Instance.SetStorageCapacity(0, CurrentStorageCapacity);
        _inventory.Clear();
    }

    public void UpgradeDrill()
    {
        MineStrengthMultiplier += 1f;
    }

    public void UpgradeFuel()
    {
        CurrentFuelCapacity += 100;
        UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
    }

    public void UpgradeStorage()
    {
        CurrentStorageCapacity += 25;
        UiController.Instance.SetStorageCapacity(GetCurrentWeight(), CurrentStorageCapacity);
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

    IEnumerator DoRefuel()
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

        yield break;
    }

    public void BurnFuel(float amount)
    {
        RemainingFuel -= amount;
        UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
    }

    public void TryMine()
    {
        // TODO: Check block based on current mine direction
        GridCell cell = GetContactCell(_currentDirection);

        _isMining = cell != null;

        if (cell == null)
            return;

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
