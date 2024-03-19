using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComboCounter
{
    [SerializeField] private ComboConfig _config;

    public ComboConfig Config => _config;
    public float ÑountResetProgress => Mathf.Clamp01((_elapsed - _lastScoreTime) / _config.ComboResetTime);
    public int ComboCount { get; private set; }
    public event Action<int> ComboCountChanged;

    private float _lastScoreTime = -100;
    private float _elapsed;

    public void OnKill()
    {
        _lastScoreTime = _elapsed;
        ComboCount++;
        ComboCountChanged?.Invoke(ComboCount);
    }

    public void Update(float dt)
    {
        _elapsed += dt;
        if(ÑountResetProgress >= 1)
        {
            ComboCount = 0;
            ComboCountChanged?.Invoke(ComboCount);
        }
    }
}
