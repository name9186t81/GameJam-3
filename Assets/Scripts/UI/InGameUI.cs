using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private PlayerActor _player;

    [Header("Main UI")]
    [SerializeField] private Text _playerScore;
    [SerializeField] private float _playerScoreMult = 1f;
    [SerializeField][Range(0, 1)] private float _scoreLerp = 0.1f;

    [Header("Pause menu")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _time;
    [SerializeField] private Slider _volumeSlider;

    private bool pause = false;
    private float startTime;
    private float currentScore;

    private void Awake()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
            if(pause)
                OpenPauseMenu();
            else
                ClosePauseMenu();
        }
    }

    private void FixedUpdate()
    {
        currentScore = Mathf.Lerp(currentScore, _player.CurrentScore * _playerScoreMult, _scoreLerp);

        _playerScore.text = Mathf.Round(currentScore).ToString();
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
        Time.timeScale = 0f; //пока так
        _volumeSlider.value = AudioListener.volume;
        _time.text = "Time: " + (new TimeSpan(10000L * 1000L * (long)(Time.time - startTime)).ToString()); //da
    }

    private void ClosePauseMenu()
    {
        _pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
