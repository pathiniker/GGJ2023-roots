using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UI_MachineHud : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject _fuelNeedle;
    [SerializeField] TextMeshProUGUI _storageText;
    [SerializeField] TextMeshProUGUI _depthText;

    public void SetFuelLevelNormalized(float level)
    {
        float animDuration = 0.15f;
        Vector2 zRange = new Vector2(93f, -93f);
        float zValue = Mathf.Lerp(zRange.x, zRange.y, level);
        Vector3 rotateTo = new Vector3(0, 0, zValue);
        _fuelNeedle.transform.DOLocalRotate(rotateTo, animDuration);
    }

    public void SetStorageText(int weight, int capacity)
    {
        _storageText.SetText($"{weight} / {capacity}");
    }

    public void SetDepthText(float depth)
    {
        _depthText.SetText(depth.ToString("0"));
    }
}
