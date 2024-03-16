using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(LanguageData))]
public class LanguageDataEditor : Editor
{
    private const string _pathToKeys = "Assets/!Localization/Resources/Localization/Keys.asset";
    private const float _widthLabel = 160f;
    private const float _offsetLabelBetweenText = 5f;

    private SerializedObject _keys;
    SerializedProperty texts;
    SerializedProperty keys;
    SerializedProperty num;
    SerializedProperty DrawElementsCount;
    LanguageData[] r;

    private void OnEnable()
    {
        _keys = new SerializedObject(AssetDatabase.LoadMainAssetAtPath(_pathToKeys));

        texts = serializedObject.FindProperty("_texts");

        keys = _keys.FindProperty("_keys");

        num = serializedObject.FindProperty("_languageType");
        DrawElementsCount = serializedObject.FindProperty("DrawElementsCount");


        r = Resources.LoadAll<LanguageData>("Localization");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.PropertyField(EditorGUILayout.GetControlRect(), num);
        EditorGUI.PropertyField(EditorGUILayout.GetControlRect(), DrawElementsCount);

        int a = serializedObject.FindProperty("_languageType").enumValueIndex;

        for(int i = 0; i < r.Length; i++)
        {
            if (r[i] != target)
            {
                if (((int)r[i].LanguageType) == a)
                {
                    EditorGUILayout.LabelField("Ошибка: SO с таким языком уже существует. Измените язык или удалите SO");
                    break;
                }
            }
        }

        if (texts.arraySize != keys.arraySize)
        {
            texts.arraySize = keys.arraySize;
        }

        for (int i = DrawElementsCount.intValue; i < keys.arraySize; ++i)
        {
            SerializedProperty text = texts.GetArrayElementAtIndex(i);
            Rect rect = EditorGUILayout.GetControlRect();

            Rect rectKey = new Rect(rect.x, rect.y, _widthLabel, rect.height);
            EditorGUI.LabelField(rectKey, keys.GetArrayElementAtIndex(i).stringValue);

            Rect rectText = new Rect(rectKey.xMax + _offsetLabelBetweenText, rect.y, rect.width - _offsetLabelBetweenText - _widthLabel, rect.height);

            string ls = text.stringValue;

            text.stringValue = EditorGUI.DelayedTextField(rectText, text.stringValue);
            //text.stringValue = EditorGUI.DelayedTextField(rectText, text.stringValue, EditorStyles.textArea);

            if(text.stringValue != ls)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}