using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;

    private void Start()
    {
        if(AudioListener.volume == 1)
            OnChangeVolumeSlider(_volumeSlider.value);
        else
            _volumeSlider.value = AudioListener.volume;
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnChangeVolumeSlider(float value)
    {
        AudioListener.volume = value;
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
