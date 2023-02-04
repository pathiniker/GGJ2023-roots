using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    static public UiController Instance;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI _elevationText;
    [SerializeField] TextMeshProUGUI _coinsText;
    [SerializeField] UI_FuelGauge _fuelDisplay;
    [SerializeField] UI_StorageDisplay _storageDisplay;

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
        _elevationText.SetText(elevation.ToString("0") + " ft");
    }

    public void SetFuelDisplay(float normalizedLevel)
    {
        _fuelDisplay.SetFuelLevelNormalized(normalizedLevel);
    }

    public void SyncStorageDisplay(Dictionary<string, int> inventory, int weight, int capacity)
    {
        _storageDisplay.SyncInventory(inventory, weight, capacity);
    }

    public void ClearInventory()
    {
        _storageDisplay.ClearInventory();
    }

    public void SyncCurrencyDisplay(int currency)
    {
        _coinsText.SetText($"${currency}");
    }
}
