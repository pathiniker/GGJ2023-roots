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
    [SerializeField] UI_BossFight _bossFight;
    [SerializeField] GameObject _startScreen;

    public UI_BossFight BossFight { get { return _bossFight; } }

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

        _bossFight.gameObject.SetActive(false);
        ShowStartScreen(true);
    }

    public void ShowStartScreen(bool show)
    {
        _startScreen.gameObject.SetActive(show);
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
        //_depthLevelNameText.SetText(levelName);
        //_depthLevelNameText.SetText("");
        _depthLevelNameText.DOKill();
        _depthLevelNameText.DOText(levelName, 1f, scrambleMode: ScrambleMode.None);
    }

    public void DoBossFightDisplay()
    {
        _depthLevelNameText.SetText("");
        _coinsText.SetText("");
        _bossFight.gameObject.SetActive(true);
        _bossFight.SetHealthValueNormalized(1f);
    }
}
