using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.Linq;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] private Dropdown LangugeDropdown;

    private void Awake()
    {
        LangugeDropdown.options = new List<Dropdown.OptionData>();
        var _languageToTexts = new ReadOnlyDictionary<LanguageType, LanguageData>(Resources.LoadAll<LanguageData>("Localization").ToDictionary((data) => data.LanguageType, (data) => data));

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

    private void OnValueChanged(int value)
    {
        Localization.ChangeLanguageType((LanguageType)value);
    }
}
