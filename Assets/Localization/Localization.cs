using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Linq;

public enum LanguageType
{
    Russian,
    English,
    Chinese
}

public static class Localization
{
    public static event System.Action<LanguageType> OnLanguageChanged;

    private static ReadOnlyDictionary<string, string> _keysToText;// = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

    public static LanguageType CurrentLanguageType;
    private static ReadOnlyDictionary<LanguageType, ReadOnlyCollection<string>> _languageToTexts;
    private static ReadOnlyCollection<string> _keysForLocalisationText;

    private const string PrefsKey = "LocalizationLanguage";

    private const string eng = "a b c d e f g h i j k l m n o p q r s t u v w x y z";
    private const string ru =  "а б к д е ф г х и ж к л м н о п кю р с т у в в кс и з";

    private const string eng2 = "а б в г д е ё ж з и й к л м н о п р с т у ф х ц ч ш щ ы э ю я";
    private const string ru2 =  "a b v g d e e zh z i y k l m n o p r s t y f x c ch sh sh y y u ya";

    private static SystemLanguage[] languages = { SystemLanguage.Russian, SystemLanguage.English, SystemLanguage.Chinese };

    public static string GetTransalteKey(this LanguageType language)
    {
        switch(language)
        {
            case LanguageType.Russian: return "ru";
            case LanguageType.English: return "en";
            case LanguageType.Chinese: return "zh-CN";
        }

        return "auto";
    }

    public static string Get(string key, bool recursion = true)
    {
        if (_keysToText.TryGetValue(key, out string value))
        {
            if (!useTranslit)
            {
                return value;
            }

            //рофл
            var text = value;

            string res = "";

            for(int i = 0; i < text.Length; i++)
            {
                bool breaked = false;
                for (int j = 0; j < en.Count; j++)
                {
                    if(text[i] == en[j][0])
                    {
                        res += r[j];
                        breaked = true;
                        break;
                    }
                }

                if(!breaked)
                {
                    res += text[i];
                }
            }

            return res;
            
        }
        else if(recursion)
        {
            //Debug.LogError("it actually happends???");
            //UpdateTexts();
            return Localization.Get(key, false);
        }
        else
        {
            //Debug.LogError("Ошибка! В языке " + CurrentLanguageType.ToString() + " не найден ключ " + key + " ");
            //return "error";
            return key;
        }
    }

    static List<string> r;
    static List<string> en;

    public static bool useTranslit = false;

    static Localization()
    {
        en = eng2.Split(' ').ToList();
        r = ru2.Split(' ').ToList();
        en.AddRange(eng2.ToUpper().Split(' '));
        r.AddRange(ru2.ToUpper().Split(' '));

        UpdateTexts();
    }

    private static void UpdateTexts()
    {
        /*
        var d = new ReadOnlyDictionary<LanguageType, ReadOnlyCollection<string>>(Resources.LoadAll<LanguageData>("Localization")
            .ToDictionary((data) => data.LanguageType, (data) => data.Texts));

        _languageToTexts = d;

        foreach (var v in d)
        {
            if(v.Key == lang)
            {
                //_languageToTexts = new ReadOnlyDictionary<LanguageType, ReadOnlyCollection<string>>(new KeyValuePair<LanguageType, ReadOnlyCollection<string>> { v });
            }
        }
        */
        var loadedKeys = Resources.Load<LanguageKeysData>("Localization/Keys");

        _keysForLocalisationText = loadedKeys.Texts;

        Resources.UnloadAsset(loadedKeys);

        if (!PlayerPrefs.HasKey(PrefsKey))
        {
            int index = 1;

            for (int i = 0; i < languages.Length; i++)
            {
                if(Application.systemLanguage == languages[i])
                {
                    index = i;
                    break;
                }    
            }

            PlayerPrefs.SetInt(PrefsKey, index);
        }
        else
        {
            /*
            if (PlayerPrefs.GetInt(PrefsKey) >= LanguageType.)
            {
                PlayerPrefs.SetInt(PrefsKey, 0);
            }
            */
        }

        var lang = (LanguageType)PlayerPrefs.GetInt(PrefsKey);

        ChangeLanguageType(lang);
    }

    public static void ChangeLanguageType(LanguageType languageType)
    {
        bool b = CurrentLanguageType != languageType;

        CurrentLanguageType = languageType;

        PlayerPrefs.SetInt(PrefsKey, (int)CurrentLanguageType);

        var loadedResource = Resources.Load<LanguageData>("Localization/" + languageType.ToString());

        ReadOnlyCollection<string> texts = loadedResource.Texts;

        Resources.UnloadAsset(loadedResource);

        Dictionary<string, string> keysForLocalisationText = new Dictionary<string, string>();

        for (int i = 0; i < _keysForLocalisationText.Count; ++i)
        {
            try
            {
                //Debug.Log(i.ToString() + " - " + _keysForLocalisationText[i] + " - " + texts[i]);
                keysForLocalisationText.Add(_keysForLocalisationText[i], texts[i]);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        _keysToText = new ReadOnlyDictionary<string, string>(keysForLocalisationText);

        if (b)
        {
            OnLanguageChanged?.Invoke(languageType);
        }
    }
}
