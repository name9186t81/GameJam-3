using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraSizeAbstraction))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private SlimeActior _actor;

    [SerializeField] private float _cameraSizeMult = 5;
    [SerializeField] private float _cameraSizeSmoothTime = 0.1f;
    [SerializeField] private AnimationCurve _cameraSize;

    private CameraSizeAbstraction _camera;
    private float _cameraSizeVelocity = 0f;
    private float _zOffset;

    private void Awake()
    {
        _camera = GetComponent<CameraSizeAbstraction>();
        _zOffset = transform.position.z;
    }

    void LateUpdate()
    {
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _cameraSize.Evaluate(_actor.Size) * _cameraSizeMult, ref _cameraSizeVelocity, _cameraSizeSmoothTime);
        transform.position = ((Vector3)_actor.TransformPosition) + Vector3.forward * _zOffset;
    }
}
