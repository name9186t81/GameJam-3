using UnityEngine;
using System.Collections.ObjectModel;
using System;
using SimpleJSON;

[CreateAssetMenu]
public class LanguageData : ScriptableObject
{
    [SerializeField] private LanguageType _languageType;
    [SerializeField] private int DrawElementsCount;
    [SerializeField] private string[] _texts;

    public LanguageType LanguageType => _languageType;

    public ReadOnlyCollection<string> Texts => System.Array.AsReadOnly(_texts);

    [InspectorButton("TranslateData")]
    public bool translate;
    public string translateFrom;
    public string translateTo;

    public void TranslateData()
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            int index = i;

            Translate.TranstaleText(_texts[i], delegate (string text)
            {
                _texts[index] = text;
            }, false, translateFrom, translateTo);
        }
    }
}