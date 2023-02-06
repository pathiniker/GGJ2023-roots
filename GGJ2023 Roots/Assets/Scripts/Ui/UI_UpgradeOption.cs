using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_UpgradeOption : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI _labelText;
    [SerializeField] TextMeshProUGUI _costText;

    System.Action _onClickCb;

    public void SyncTo(string label, int cost, System.Action onClickCb)
    {
        _labelText.SetText(label);
        _costText.SetText($"${cost}");
        _onClickCb = onClickCb;
    }

    #region UI Callbacks
    public void OnClick_Option()
    {
        _onClickCb?.Invoke();
        _onClickCb = null;

        gameObject.SetActive(false);
    }
    #endregion
}
