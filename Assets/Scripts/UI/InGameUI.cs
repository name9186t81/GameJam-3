using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("Pause menu")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _time;
    [SerializeField] private Slider _volumeSlider;

    private bool pause = false;
    private float startTime;

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
