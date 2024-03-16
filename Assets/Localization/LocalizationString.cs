using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.ObjectModel;

[System.Serializable]
public class LocalizationString
{
    [SerializeField] public string Key;
    [SerializeField] public string CurrentText;
    [SerializeField] public string CurrentTextEng;
    [SerializeField] public string CurrentObjectName;
    [SerializeField] public int ID = -1;
    [SerializeField] public bool isInited;
    [SerializeField] public bool DontShowKey;
    private bool returnOther;
    private string other;

    public void ReturnOther(string s)
    {
        other = s;
        returnOther = true;
    }

    public LocalizationString()
    {

    }
    
    public LocalizationString(string key, string currentText, string currentTextEng)
    {
        CurrentObjectName = key;
        CurrentText = currentText;
        CurrentTextEng = currentTextEng;
    }

    public string Get()
    {
        return returnOther ? other : Localization.Get(Key);
    }

    public static implicit operator string(LocalizationString param)
    {
        return param.Get();
    }
}

