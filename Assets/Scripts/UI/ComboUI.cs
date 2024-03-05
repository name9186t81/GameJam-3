using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private Animation _comboAnim;
    [SerializeField] private float _comboAnimPeakTime = 0.05f;
    [SerializeField] private Text _comboCountText;
    [SerializeField] private Text _comboScoreText;
    [SerializeField] private float _comboTime;
    [SerializeField] private AnimationCurve _alphaOverTime;
    [SerializeField] private float _scoreSmoothTime = 0.15f;
    [SerializeField] private float _comboSmoothTime = 0.15f;

    public int ComboCount { get; private set; }
    public float NewScoreCount { get; private set; }
    public event Action<int> OnComboCountChanged;

    private float _lastScoreTime = -100;
    private FloatSmoothDamp _scoreSmooth;
    private FloatSmoothDamp _comboSmooth;

    private void Awake()
    {
        _scoreSmooth = new FloatSmoothDamp(_scoreSmoothTime);
        _comboSmooth = new FloatSmoothDamp(_comboSmoothTime);
    }

    public void OnCombo(float score)
    {
        _lastScoreTime = Time.time;
        ComboCount++;
        NewScoreCount += score;

        OnComboCountChanged?.Invoke(ComboCount);

        if (!_comboAnim.isPlaying)
            _comboAnim.Play();
        else
            _comboAnim[_comboAnim.clip.name].time = _comboAnimPeakTime;
    }

    private void Update()
    {
        var elapsedTime = (Time.time - _lastScoreTime) / _comboTime;
        var alpha = _alphaOverTime.Evaluate(elapsedTime);
        _comboScoreText.color = new Color(_comboScoreText.color.r, _comboScoreText.color.g, _comboScoreText.color.b, alpha);

        if(elapsedTime > 1 && ComboCount != 0)
        {
            ComboCount = 0;
            NewScoreCount = 0;

            OnComboCountChanged?.Invoke(ComboCount);
        }

        _comboCountText.text = Mathf.Floor(Mathf.Max(ComboCount, _comboSmooth.Update(ComboCount))).ToString() + "X";

        _comboScoreText.text = "+" + Mathf.Round(_scoreSmooth.Update(NewScoreCount));
    }
}
