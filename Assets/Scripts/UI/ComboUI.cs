using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private InGameUI _inGameUI;
    [SerializeField] private Animation _comboAnim;
    [SerializeField] private float _comboAnimPeakTime = 0.05f;
    [SerializeField] private Text _comboCountText;
    [SerializeField] private Text _comboScoreText;
    [SerializeField] private AnimationCurve _alphaOverTime;
    [SerializeField] private float _scoreSmoothTime = 0.15f;
    [SerializeField] private float _comboSmoothTime = 0.15f;

    private ComboCounter _currentCounter => _inGameUI.CurrentPlayer.ComboCounter;

    public static int ComboCount { get; private set; } //TODO: delete?
    public float NewScoreCount { get; private set; }

    private FloatSmoothDamp _scoreSmooth;
    private FloatSmoothDamp _comboSmooth;

    private void Awake()
    {
        _scoreSmooth = new FloatSmoothDamp(_scoreSmoothTime);
        _comboSmooth = new FloatSmoothDamp(_comboSmoothTime);
    }

    public void OnCombo(float score)
    {
        NewScoreCount += score;

        if (!_comboAnim.isPlaying)
            _comboAnim.Play();
        else
            _comboAnim[_comboAnim.clip.name].time = _comboAnimPeakTime;
    }

    private void Update()
    {
        ComboCount = _currentCounter.ComboCount;

        var progress = _currentCounter.ÑountResetProgress;
        var alpha = _alphaOverTime.Evaluate(progress);
        _comboScoreText.color = new Color(_comboScoreText.color.r, _comboScoreText.color.g, _comboScoreText.color.b, alpha);

        if(progress >= 1)
        {
            NewScoreCount = 0;
        }

        _comboCountText.text = Mathf.Floor(Mathf.Max(ComboCount, _comboSmooth.Update(ComboCount))).ToString() + "X";

        _comboScoreText.text = "+" + Mathf.Round(_scoreSmooth.Update(NewScoreCount));
    }
}
