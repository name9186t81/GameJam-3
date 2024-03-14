using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSizeAbstraction : MonoBehaviour
{
    [Header("[0]  Vertical - - - Horizontal  [1]")]
    [Range(0f, 1f)]
    [SerializeField] private float _matchSize;
    public float orthographicSize;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        var verticalSize = orthographicSize;
        var horizontalSize = orthographicSize * (Screen.height / Screen.width);

        _camera.orthographicSize = _matchSize.map01(verticalSize, horizontalSize);
    }
}
