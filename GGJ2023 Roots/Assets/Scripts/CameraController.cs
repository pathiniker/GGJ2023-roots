using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 _cameraOffset;
    [SerializeField] float _smoothSpeed = 0.1f;

    [Header("Components")]
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _followObject;

    Vector3 GetFollowPosition()
    {
        return _followObject.transform.position + _cameraOffset;
    }

    //private void Update()
    //{
    //    Vector3 toPosition = GetFollowPosition();
    //    Vector3 currentPosition = _camera.transform.position;
    //    Vector3 moveBy = Vector3.Lerp(currentPosition, toPosition, 0.5f);
    //    _camera.transform.DOBlendableMoveBy(moveBy, 0.15f);
    //    //_camera.transform.position = GetFollowPosition();
    //}

    void FollowPlayer()
    {
        Vector3 targetPos = _followObject.transform.position + _cameraOffset;
        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, _smoothSpeed);

        transform.position = smoothFollow;
        transform.LookAt(_followObject.transform);
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }
}
