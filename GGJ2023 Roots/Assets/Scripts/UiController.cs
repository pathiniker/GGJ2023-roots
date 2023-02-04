using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    static public UiController Instance;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI _elevationText;

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
}
