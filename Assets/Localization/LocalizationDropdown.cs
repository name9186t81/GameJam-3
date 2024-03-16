using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Dropdown))]
public class LocalizationDropdown : MonoBehaviour
{
    //[NonReorderable]
    [SerializeField] private LocalizationString[] Strings;
    private Dropdown dropdown;

    private void Awake()
    {
        if (dropdown == null)
        {
            dropdown = GetComponent<Dropdown>();
        }

        int lastValue = dropdown.value;

        if (Application.isPlaying)
        {
            UpdateText(Localization.CurrentLanguageType);
            Localization.OnLanguageChanged += UpdateText;
        }
        else
        {
            Dropdown.OptionData[] options = dropdown.options.ToArray();

            Strings = new LocalizationString[options.Length];

            for(int i = 0; i < Strings.Length; i++)
            {
                Strings[i] = new LocalizationString();
                Strings[i].CurrentText = options[i].text;
                Strings[i].CurrentObjectName = gameObject.name + "DropElement" + i.ToString();
            }
        }

        dropdown.value = lastValue;
    }

    private void OnDestroy()
    {
        Localization.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText(LanguageType language)
    {
        for (int i = 0; i < Strings.Length; i++)
        {
            dropdown.options[i].text = Strings[i].Get();
        }

        int lastValue = dropdown.value;
        dropdown.value = 0;
        dropdown.value = dropdown.options.Count - 1;
        dropdown.value = lastValue;
    }
}
