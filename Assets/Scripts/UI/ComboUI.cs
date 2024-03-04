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

    private float lastScoreTime = -100;
    private int comboCount;
    private float scoreCount;
    private FloatSmoothDamp _scoreSmooth;
    private FloatSmoothDamp _comboSmooth;

    private void Awake()
    {
        _scoreSmooth = new FloatSmoothDamp(_scoreSmoothTime);
        _comboSmooth = new FloatSmoothDamp(_comboSmoothTime);
    }

    public void OnCombo(float score)
    {
        lastScoreTime = Time.time;
        comboCount++;
        scoreCount += score;

        if (!_comboAnim.isPlaying)
            _comboAnim.Play();
        else
            _comboAnim[_comboAnim.clip.name].time = _comboAnimPeakTime;
    }

    private void Update()
    {
        var elapsedTime = (Time.time - lastScoreTime) / _comboTime;
        var alpha = _alphaOverTime.Evaluate(elapsedTime);
        _comboScoreText.color = new Color(_comboScoreText.color.r, _comboScoreText.color.g, _comboScoreText.color.b, alpha);

        if(elapsedTime > 1)
        {
            comboCount = 0;
            scoreCount = 0;
        }

        _comboCountText.text = Mathf.Floor(Mathf.Max(comboCount, _comboSmooth.Update(comboCount))).ToString() + "X";

        _comboScoreText.text = "+" + Mathf.Round(_scoreSmooth.Update(scoreCount));
    }
}
