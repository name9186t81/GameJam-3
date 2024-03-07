using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InGameMusicController : MonoBehaviour
{
    [SerializeField] private float _pitchOverCombo = 0.01f;
    [SerializeField] private float _pitchMax = 1.2f;
    [SerializeField] private float _pitchSmoothTime = 0.3f;

    private FloatSmoothDamp _pitchDamp;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _pitchDamp = new FloatSmoothDamp(_pitchSmoothTime, 1);
    }

    private void Update()
    {
        _audioSource.pitch = _pitchDamp.Update(Mathf.Min(1 + _pitchOverCombo * ComboUI.ComboCount, _pitchMax));
    }
}
