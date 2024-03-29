using GameLogic;
using Health;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayerInput;
using Core;
using PlayerAbilities;

public class InGameUI : MonoBehaviour, TimeScaleController.ITimeScaleMultiplyer
{
    [SerializeField] private SlimeActior _player;
    [SerializeField] private AbilitiesContainer _abilitiesContainer;

    [Header("Main UI")]
    [SerializeField] private Text _playerScore;
    [SerializeField] private float _visualScoreMult = 100f;
    [SerializeField] private float _scoreSmoothTime = 0.2f;
    [SerializeField] private ComboUI _comboUI;

    [Header("Pause menu")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _time;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private LocalizationString _timeString;
    [SerializeField] private LocalizationString _scoreString;

    [Header("End game menu")]
    [SerializeField] private GameObject _winText;
    [SerializeField] private GameObject _looseText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private GameObject _volumeSliderGO;

    private bool _pause = false;
    private float _startTime;
    private FloatSmoothDamp _scoreSmooth;

    public float TimeScale => _pause ? 0 : 1;

    public SlimeActior CurrentPlayer => _player;
    public AbilitiesContainer AbilitiesContainer => _abilitiesContainer;

    private float _maxScore = 0;

    private InputProvider _inputProvider;


    private void Start()
    {
        _startTime = Time.time;
        _scoreSmooth = new FloatSmoothDamp(_scoreSmoothTime);
        _player.Health.OnAddScore += delegate (float score) { _comboUI.OnCombo(score * _visualScoreMult); _maxScore = Mathf.Max(_maxScore, _player.CurrentScore); };
        TimeScaleController.Add(this);
        _player.Health.OnDeath += OnPlayerDeath;
        _inputProvider = ServiceLocator.Get<InputProvider>();
        _inputProvider.Action += OnAction;
    }

    private void OnDestroy()
    {
        _inputProvider.Action -= OnAction;
        TimeScaleController.Remove(this);
    }

    private void OnAction(InputProvider.ActionType type)
    {
        if(type == InputProvider.ActionType.Pause)
        {
            _pause = !_pause;
            if (_pause)
                OpenPauseMenu(true);
            else
                ClosePauseMenu();
        }
    }

    private void Update()
    {
        _playerScore.text = Mathf.Round(_scoreSmooth.Update(_player.CurrentScore * _visualScoreMult)).ToString();
    }

    public void OnPlayerWin()
    {
        _pause = true;
        OpenPauseMenu(false, true);
    }

    public void OnPlayerDeath(DamageArgs args)
    {
        _pause = true;
        OpenPauseMenu(false, false);
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

    //�� ��� ���� ������ ��������� ���� ����� ���� ������� � ������ ��� �� ������ ���� ����� ���!!
    private void SetPauseOrEndGameState(bool pause, bool win = false)
    {
        _winText.gameObject.SetActive(!pause && win);
        _looseText.gameObject.SetActive(!pause && !win);
        _scoreText.transform.parent.gameObject.SetActive(!pause);
        _resumeButton.SetActive(pause);
        _volumeSliderGO.SetActive(pause);
    }

    private void OpenPauseMenu(bool pause, bool win = false)
    {
        _pausePanel.SetActive(true);
        _volumeSlider.value = AudioListener.volume;

        _time.text = _timeString.Get() + ": " + (new TimeSpan(10000L * 1000L * (long)(Time.time - _startTime)).ToString()); //da
        _scoreText.text = _scoreString.Get() + ": " + MathF.Round(_maxScore * _visualScoreMult).ToString();

        SetPauseOrEndGameState(pause, win);
    }

    private void ClosePauseMenu()
    {
        _pausePanel.SetActive(false);
        _pause = false;
    }
}