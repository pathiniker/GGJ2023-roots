using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UiController : MonoBehaviour
{
    static public UiController Instance;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI _elevationText;
    [SerializeField] TextMeshProUGUI _coinsText;
    [SerializeField] UI_FuelGauge _fuelDisplay;
    [SerializeField] UI_StorageDisplay _storageDisplay;
    [SerializeField] UI_MachineHud _hud;
    [SerializeField] TextMeshProUGUI _depthLevelNameText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetElevationText(float elevation)
    {
        //_elevationText.SetText(elevation.ToString("0") + " ft");
        _hud.SetDepthText(elevation * 5);
    }

    public void SetFuelDisplay(float normalizedLevel)
    {
        _hud.SetFuelLevelNormalized(normalizedLevel);
        //_fuelDisplay.SetFuelLevelNormalized(normalizedLevel);
    }

    public void SyncStorageDisplay(Dictionary<string, int> inventory, int weight, int capacity)
    {
        _hud.SetStorageText(weight, capacity);
        //_storageDisplay.SyncInventory(inventory, weight, capacity);
    }

    public void SetStorageCapacity(int weight, int capacity)
    {
        //_storageDisplay.SyncCapacity(weight, capacity);
        _hud.SetStorageText(weight, capacity);
    }

    //public void ClearInventory()
    //{
    //    _storageDisplay.ClearInventory();
    //    _hud.SetStorageText(0, )
    //}

    public void SyncCurrencyDisplay(int currency)
    {
        _coinsText.SetText($"${currency}");
    }

    public void SetDepthLevelName(string levelName)
    {
        _depthLevelNameText.SetText(levelName);
    }
}
