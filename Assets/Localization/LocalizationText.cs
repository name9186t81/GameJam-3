using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.ObjectModel;

[ExecuteInEditMode]
public class LocalizationText : MonoBehaviour
{
    [SerializeField] private LocalizationString String = new LocalizationString();
    [HideInInspector] [SerializeField] private string Key;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            UpdateText(Localization.CurrentLanguageType);
            Localization.OnLanguageChanged += UpdateText;
        }
        else
        {
            if (TryGetComponent(out TMP_Text tmpText))
            {
                String.CurrentText = tmpText.text;
            }
            else if (TryGetComponent(out Text uiText))
            {
                String.CurrentText = uiText.text;
            }
            String.CurrentObjectName = gameObject.name;
        }
    }

    private void UpdateText(LanguageType language)
    {
        if (String.Key == null)
        {
            return;
        }

        string text = Localization.Get(String.Key);

        if (TryGetComponent(out TMP_Text tmpText))
        {
            tmpText.text = text;
        }
        else if (TryGetComponent(out Text uiText))
        {
            uiText.text = text;
        }
    }

    private void OnDestroy()
    {
        Localization.OnLanguageChanged -= UpdateText;
    }
}

