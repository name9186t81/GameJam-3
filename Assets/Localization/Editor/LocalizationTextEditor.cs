using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections.ObjectModel;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(LocalizationString))]
public class LocalizationTextEditor : PropertyDrawer
{
    private SerializedObject _keys;
    private SerializedProperty keys;
    private SerializedProperty key;
    private SerializedProperty ID;
    private const string _pathToKeys = "Assets/Localization/Resources/Localization/Keys.asset";
    private static ReadOnlyDictionary<LanguageType, SerializedObject> _languageToTexts;
    private const float _widthLabel = 60f;
    private const float _offsetLabelBetweenText = 0f;
    private List<SerializedProperty> allObjects = new List<SerializedProperty>();
    bool changed;
    bool needWarn;

    private void FindID()
    {
        bool finded = false;
        for (int i = 0; i < keys.arraySize; i++)
        {
            if (keys.GetArrayElementAtIndex(i).stringValue == key.stringValue)
            {
                ID.intValue = i;
                finded = true;
                break;
            }
        }

        if(!finded)
        {
            ID.intValue = -1;
        }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            foreach (var v in allObjects)
            {
                v.FindPropertyRelative("isInited").boolValue = false;
            }

            Debug.Log(allObjects.Count);
            allObjects.RemoveRange(0, allObjects.Count);
        }
    }

    void UpdateKeys()
    {
        _keys = new SerializedObject(AssetDatabase.LoadMainAssetAtPath(_pathToKeys));
        keys = _keys.FindProperty("_keys");
        _languageToTexts = new ReadOnlyDictionary<LanguageType, SerializedObject>(Resources.LoadAll<LanguageData>("Localization").ToDictionary((data) => data.LanguageType, (data) => new SerializedObject(data)));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Application.isPlaying)
        {
            return;
        }

        bool finded = false;

        foreach (var v in allObjects)
        {
            if (v.propertyPath == property.propertyPath)
            {
                finded = true;
                break;
            }
        }

        if (!finded)
        {
            allObjects.Add(property);
            changed = true;
        }

        key = property.FindPropertyRelative("Key");
        ID = property.FindPropertyRelative("ID");

        if (keys == null)
        {
            UpdateKeys();
        }

        if (_languageToTexts == null)
        {
            _languageToTexts = new ReadOnlyDictionary<LanguageType, SerializedObject>(Resources.LoadAll<LanguageData>("Localization").ToDictionary((data) => data.LanguageType, (data) => new SerializedObject(data)));
        }

        if (!Application.isPlaying && !property.FindPropertyRelative("isInited").boolValue && ID.intValue == -1 && key.stringValue == "")
        {
            key.stringValue = property.FindPropertyRelative("CurrentObjectName").stringValue;

            FindID();

            property.FindPropertyRelative("isInited").boolValue = true;

            changed = true;

            if (key.stringValue == "" && ID.intValue == -1)
            {
                key.stringValue = property.FindPropertyRelative("CurrentObjectName").stringValue;
                AddCurrentKey(property.FindPropertyRelative("CurrentObjectName").stringValue);

                foreach (var v in _languageToTexts)
                {
                    if (v.Key == LanguageType.Russian)
                    {
                        v.Value.FindProperty("_texts").GetArrayElementAtIndex(keys.arraySize - 1).stringValue = property.FindPropertyRelative("CurrentText").stringValue;
                    }
                    else if (v.Key == LanguageType.English)
                    {
                        v.Value.FindProperty("_texts").GetArrayElementAtIndex(keys.arraySize - 1).stringValue = property.FindPropertyRelative("CurrentTextEng").stringValue;
                    }
                    else
                    {
                        v.Value.FindProperty("_texts").GetArrayElementAtIndex(keys.arraySize - 1).stringValue = "";
                    }
                    v.Value.ApplyModifiedProperties();
                }
            }
            else
            {
                FindID();
            }

            //serializedObject.ApplyModifiedProperties();
            _keys.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();
        }

        fieldPixels = 0;
        fieldPixels2 = 0;
        fieldCount = 0;
        _currentChoicePropertyPath = "";

        position.height = EditorGUIUtility.singleLineHeight * 3;


        //if (GUIUtility.GetControlID(FocusType.Passive, position) == GUIUtility.keyboardControl - 1)
        //{
        //position.height = EditorGUIUtility.singleLineHeight * 3;
        //}

        if (ID.intValue == -1 || ID.intValue >= keys.arraySize)
        {
            FindID();
        }

        if(ID.intValue == -1)
        {
            //AddCurrentKey(key.stringValue);
            //FindID();
        }

        var nameFieldRect = GetControlRect(EditorGUIUtility.singleLineHeight, position, false);
        float idTextSize = -30f;
        nameFieldRect.size -= idTextSize * new Vector2(1,0);
        nameFieldRect.center -= idTextSize * new Vector2(0.5f,0);
        EditorGUI.LabelField(nameFieldRect, property.displayName, EditorStyles.boldLabel);

        bool singleLineFiled = property.name.Contains("Name") || property.name.Contains("Desc") || property.name.Contains("Short");

        //EditorGUI.PropertyField(GetControlRect(EditorGUIUtility.singleLineHeight, position), key);
        EditorGUI.PropertyField(GetControlRect(EditorGUIUtility.singleLineHeight, position), ID);
        //EditorGUI.PropertyField(GetControlRect(EditorGUIUtility.singleLineHeight, position), property.FindPropertyRelative("isInited"));

        if (ID.intValue != -1)
        {
            DrawTextAndFieldProperty("Key", keys.GetArrayElementAtIndex(ID.intValue), _keys, position, property, false);
        }

        if (ID.intValue != -1)
        {
            if (false && key.stringValue != "" && key.stringValue != keys.GetArrayElementAtIndex(ID.intValue).stringValue)
            {
                //for (int i = 0; i < keys.arraySize; i++)
                        
            }

            key.stringValue = keys.GetArrayElementAtIndex(ID.intValue).stringValue;
        }


        if (changed)
        {
            needWarn = false;
            changed = false;

            for (int i = 0; i < keys.arraySize; i++)
            {
                if (keys.GetArrayElementAtIndex(i).stringValue == key.stringValue && i != ID.intValue)
                {
                    needWarn = true;

                    break;
                }
            }
        }

        if (needWarn)
        {
            EditorGUI.LabelField(GetControlRect(EditorGUIUtility.singleLineHeight, position), "Ошибка: такой ключ уже существует. Измените его");

            if (ID.intValue + 1 == keys.arraySize)
            {
                if (GUI.Button(GetControlRect(EditorGUIUtility.singleLineHeight, position), "Удалить из ключей"))
                {
                    RemoveCurrentKey();
                    changed = true;
                }
            }
        }

        if (ID.intValue != -1)
        {
            foreach (var v in _languageToTexts)
            {
                SerializedProperty text = v.Value.FindProperty("_texts").GetArrayElementAtIndex(ID.intValue);
                DrawTextAndFieldProperty(v.Key.ToString(), text, _languageToTexts[v.Key], position, property, !singleLineFiled);
            }
        }

        bool doEverything = false;

        string customRuText = "";

        var rect1 = position;
        rect1.width /= 2;
        var rect2 = rect1;
        rect2.x += rect1.width;

        if (GUI.Button(GetControlRect(EditorGUIUtility.singleLineHeight, rect1, false), "Вставить и сделать все"))
        {
            var textEditor = new TextEditor();
            textEditor.multiline = true;
            if (textEditor.CanPaste())
            {
                textEditor.Paste();
                var text = textEditor.text;
                customRuText = text;
                //property.FindPropertyRelative("CurrentText").stringValue = text;
                //var v = _languageToTexts[LanguageType.Russian];
                //SerializedProperty prop = v.FindProperty("_texts").GetArrayElementAtIndex(ID.intValue);
                //prop.stringValue = text;
                //v.ApplyModifiedProperties();
                doEverything = true;
                //тут крч надо вызвать создание нового ключа, его рандом, и перевод, все это по кнопкам так что кнопки в методы выводить? хз
            }
        }

        if (GUI.Button(GetControlRect(EditorGUIUtility.singleLineHeight, rect2), "Новый ключ") || doEverything)
        {
            UpdateKeys();

            property.FindPropertyRelative("isInited").boolValue = false;

            if (!doEverything)
            {
                property.FindPropertyRelative("CurrentText").stringValue = _languageToTexts[LanguageType.Russian].FindProperty("_texts").GetArrayElementAtIndex(ID.intValue).stringValue;
                property.FindPropertyRelative("CurrentTextEng").stringValue = _languageToTexts[LanguageType.English].FindProperty("_texts").GetArrayElementAtIndex(ID.intValue).stringValue;
            }
            else
            {
                property.FindPropertyRelative("CurrentText").stringValue = customRuText;
                property.FindPropertyRelative("CurrentTextEng").stringValue = "";
            }

            ID.intValue = -1;

            property.FindPropertyRelative("CurrentObjectName").stringValue = key.stringValue + " (duplicate)";

            key.stringValue = "";

            key.stringValue = property.FindPropertyRelative("CurrentObjectName").stringValue;
            AddCurrentKey(property.FindPropertyRelative("CurrentObjectName").stringValue);

            foreach (var v in _languageToTexts)
            {
                if (v.Key == LanguageType.Russian)
                {
                    v.Value.FindProperty("_texts").GetArrayElementAtIndex(keys.arraySize - 1).stringValue = property.FindPropertyRelative("CurrentText").stringValue;
                }
                else if (v.Key == LanguageType.English)
                {
                    v.Value.FindProperty("_texts").GetArrayElementAtIndex(keys.arraySize - 1).stringValue = property.FindPropertyRelative("CurrentTextEng").stringValue;
                }
                else
                {
                    v.Value.FindProperty("_texts").GetArrayElementAtIndex(keys.arraySize - 1).stringValue = "";
                }
                v.Value.ApplyModifiedProperties();
            }

            _keys.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();
        }

        if (GUI.Button(GetControlRect(EditorGUIUtility.singleLineHeight, rect1, false), "Перевести с RU") || doEverything)
        {
            var original = _languageToTexts[LanguageType.Russian].FindProperty("_texts").GetArrayElementAtIndex(ID.intValue).stringValue;

            foreach (var v in _languageToTexts)
            {
                if(v.Key == LanguageType.Russian)
                {
                    continue;
                }

                SerializedProperty text = v.Value.FindProperty("_texts").GetArrayElementAtIndex(ID.intValue);

                Translate.TranstaleText(original, delegate (string result) 
                {
                    text.stringValue = result;
                    Debug.Log("Saved");
                    changed = true;
                    _languageToTexts[v.Key].ApplyModifiedProperties();
                }, 
                false, LanguageType.Russian.GetTransalteKey(), v.Key.GetTransalteKey());
            }
        }

        if (GUI.Button(GetControlRect(EditorGUIUtility.singleLineHeight, rect2), "Случайный ключ") || doEverything)
        {
            var field = keys.GetArrayElementAtIndex(ID.intValue);
            field.stringValue = RandomString(6);

            _keys.ApplyModifiedProperties();
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    private static System.Random random = new System.Random();
    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    private Rect GetControlRect(float height, Rect controlRect, bool addHeight = true)
    {
        Rect textRect = controlRect;
        textRect.height = height;
        textRect.y += fieldPixels;

        if (addHeight)
        {
            fieldPixels += textRect.height + EditorGUIUtility.standardVerticalSpacing;
            fieldPixels2 += textRect.height + EditorGUIUtility.standardVerticalSpacing;
            fieldCount++;
        }

        return textRect;
    }

    float fieldPixels = 0;
    float fieldPixels2 = 0;
    float fieldCount = 0;

    private string _currentChoicePropertyPath;

    private void RemoveCurrentKey()
    {
        keys.DeleteArrayElementAtIndex(ID.intValue);

        foreach (var v in _languageToTexts)
        {
            v.Value.FindProperty("_texts").DeleteArrayElementAtIndex(ID.intValue);
        }

        _keys.ApplyModifiedProperties();

        FindID();
    }

    private void AddCurrentKey(string key)
    {
        int id = keys.arraySize;
        keys.arraySize++;
        keys.GetArrayElementAtIndex(id).stringValue = key;

        foreach (var v in _languageToTexts)
        {
            v.Value.FindProperty("_texts").arraySize++;
            if (ID.intValue != -1)
            {
                v.Value.FindProperty("_texts").GetArrayElementAtIndex(id).stringValue = v.Value.FindProperty("_texts").GetArrayElementAtIndex(ID.intValue).stringValue;
            }
            v.Value.ApplyModifiedProperties();
        }

        ID.intValue = id;
    }

    private void DrawTextAndFieldProperty(string text, SerializedProperty field, SerializedObject toApply, Rect ControlRect, SerializedProperty mainProperty, bool drawTextArea)
    {
        Rect rect = ControlRect;

        if (!drawTextArea)
            rect.height /= 3f;

        rect.y += fieldPixels + EditorGUIUtility.standardVerticalSpacing;

        Rect rectKey = new Rect(rect.x, rect.y, _widthLabel + 10, rect.height); //yeah 10 for some reason
        EditorGUI.LabelField(rectKey, text);

        Rect rectText = new Rect(rectKey.xMax + _offsetLabelBetweenText, rect.y, rect.width - _offsetLabelBetweenText - _widthLabel, rect.height);

        fieldPixels2 += rectText.height + EditorGUIUtility.standardVerticalSpacing;

        string lastV = field.stringValue;

        field.stringValue = EditorGUI.TextArea(rectText, field.stringValue);

        if (lastV != field.stringValue)
        {
            Debug.Log("Saved");
            changed = true;
            toApply.ApplyModifiedProperties();
        }

        fieldPixels += rectText.height + EditorGUIUtility.standardVerticalSpacing;

        fieldCount++;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(fieldCount == 0)
        {
            OnGUI(new Rect(), property, label);
        }

        //return fieldCount - EditorGUIUtility.standardVerticalSpacing;

        float height = fieldPixels2;

        if (_currentChoicePropertyPath == property.propertyPath)
        {
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4f;
        }

        //height += EditorGUIUtility.singleLineHeight;
        height += EditorGUIUtility.standardVerticalSpacing;

        if(Application.isPlaying)
        {
            height = 0;
        }

        return height;
    }
}
