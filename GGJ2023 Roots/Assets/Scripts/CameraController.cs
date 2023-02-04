using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 _cameraOffset;

    [Header("Components")]
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _followObject;

    Vector3 GetFollowPosition()
    {
        return _followObject.transform.position + _cameraOffset;
    }

    private void Update()
    {
        _camera.transform.position = GetFollowPosition();
    }
}
