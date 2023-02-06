using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_BossFight : MonoBehaviour
{
    [SerializeField] Image _healthBar;

    public void SetHealthValueNormalized(float value)
    {
        _healthBar.DOFillAmount(value, 0.25f);
    }
}
