using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour, TimeScaleController.ITimeScaleMultiplyer
{
    [SerializeField] private PlayerActor _player;

    [Header("Main UI")]
    [SerializeField] private Text _playerScore;
    [SerializeField] private float _visualScoreMult = 100f;
    [SerializeField] private float _scoreSmoothTime = 0.2f;
    [SerializeField] private ComboUI _comboUI;

    [Header("Pause menu")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _time;
    [SerializeField] private Slider _volumeSlider;

    private bool _pause = false;
    private float _startTime;
    private FloatSmoothDamp _scoreSmooth;

    public float TimeScale => _pause ? 0 : 1;

    private void Awake()
    {
        _startTime = Time.time;
        _scoreSmooth = new FloatSmoothDamp(_scoreSmoothTime);
        _player.OnAddScore += delegate (float score) { _comboUI.OnCombo(score * _visualScoreMult); };
        TimeScaleController.Add(this);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _pause = !_pause;
            if(_pause)
                OpenPauseMenu();
            else
                ClosePauseMenu();
        }

        _playerScore.text = Mathf.Round(_scoreSmooth.Update(_player.CurrentScore * _visualScoreMult)).ToString();
    }

    public void OnRestartButton()
    {
        ClosePauseMenu();
        SceneManager.LoadScene(1);
    }

    public void OnMenuButton()
    {
        ClosePauseMenu();
        SceneManager.LoadScene(0);
    }

    public void OnResumeButton()
    {
        ClosePauseMenu();
    }

    public void OnVolumeSliderValueChanged(float value)
    {
        AudioListener.volume = value;
    }

    private void OpenPauseMenu()
    {
        _pausePanel.SetActive(true);
        _volumeSlider.value = AudioListener.volume;
        _time.text = "�����: " + (new TimeSpan(10000L * 1000L * (long)(Time.time - _startTime)).ToString()); //da
    }

    private void ClosePauseMenu()
    {
        _pausePanel.SetActive(false);
        _pause = false;
    }

    private void OnDestroy()
    {
        TimeScaleController.Remove(this);
    }
}