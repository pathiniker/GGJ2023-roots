using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FuelGauge : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Color _maxColor;
    [SerializeField] Color _minColor;

    [Header("Components")]
    [SerializeField] Image _fillImage;

    Color GetFillColor(float level)
    {
        return Color.Lerp(_minColor, _maxColor, level);
    }

    public void SetFuelLevelNormalized(float level)
    {
        float animDuration = 0.15f;
        _fillImage.DOFillAmount(level, animDuration);
        _fillImage.DOColor(GetFillColor(level), animDuration);
    }
}
