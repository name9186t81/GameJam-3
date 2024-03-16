using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.Linq;

public class LanguageChanger : MonoBehaviour
{
    public Dropdown LangugeDropdown;
    private ReadOnlyDictionary<LanguageType, LanguageData> _languageToTexts;

    private void Awake()
    {
        LangugeDropdown.options = new List<Dropdown.OptionData>();
        _languageToTexts = new ReadOnlyDictionary<LanguageType, LanguageData>(Resources.LoadAll<LanguageData>("Localization").ToDictionary((data) => data.LanguageType, (data) => data));

        foreach (var v in _languageToTexts)
        {
            LangugeDropdown.options.Add(new Dropdown.OptionData(v.Key.ToString()));
        }

        LangugeDropdown.options.Reverse();

        for (int i = 0; i < LangugeDropdown.options.Count; i++)
        {
            if(LangugeDropdown.options[i].text == Localization.CurrentLanguageType.ToString())
            {
                LangugeDropdown.value = i;
            }
        }

        LangugeDropdown.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(int value)
    {
        Localization.ChangeLanguageType((LanguageType)value);
    }
}