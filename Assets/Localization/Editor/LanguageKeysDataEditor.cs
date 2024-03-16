using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(LanguageKeysData))]
public class LanguageKeysDataEditor : Editor
{
    private SerializedProperty keys;
    List<string> l;
    List<string> l2;
    private int lastLength;
    private const float _widthLabel = 160f;
    private const float _offsetLabelBetweenText = 5f;
    private int lastSelectedId;
    private int deleteBy;

    private void OnEnable()
    {
        if (keys == null)
        {
            keys = serializedObject.FindProperty("_keys");
        }

        l = new List<string>();
        l2 = new List<string>();

        lastLength = 0;

        lastSelectedId = -1;
    }

    private void CheckList()
    {
        l.RemoveRange(0, l.Count);
        l2.RemoveRange(0, l2.Count);

        for (int i = 0; i < keys.arraySize; i++)
        {
            if (l.Contains(keys.GetArrayElementAtIndex(i).stringValue))
            {
                EditorGUILayout.HelpBox("Ёлемент " + keys.GetArrayElementAtIndex(i).stringValue + " содержит дубликат", MessageType.Warning);
                l2.Add("Ёлемент " + keys.GetArrayElementAtIndex(i).stringValue + " содержит дубликат");
            }
            else
            {
                l.Add(keys.GetArrayElementAtIndex(i).stringValue);
            }
        }
    }

    Rect rect;
    Rect rectKey;
    Rect rectText;
    SerializedProperty text;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        for(int i = 600; i < keys.arraySize; i++)
        {
            text = keys.GetArrayElementAtIndex(i);
            rect = EditorGUILayout.GetControlRect();

            rectKey = new Rect(rect.x, rect.y, _widthLabel, rect.height);
            EditorGUI.LabelField(rectKey, "Element " + i.ToString());

            rectText = new Rect(rectKey.xMax + _offsetLabelBetweenText, rect.y, rect.width - _offsetLabelBetweenText - _widthLabel, rect.height);

            if (GUIUtility.GetControlID(FocusType.Passive, rectText) == GUIUtility.keyboardControl - 1)
            {
                rectText.height *= 2f;
                text.stringValue = EditorGUI.TextArea(rectText, text.stringValue, EditorStyles.textArea);
                EditorGUILayout.Space((70f/5) * 1.5f);
                lastSelectedId = i;
            }
            else
            {
                text.stringValue = EditorGUI.TextField(rectText, text.stringValue);
            }
        }
        

        if (EditorGUILayout.DropdownButton(new GUIContent("Check"), FocusType.Passive))
        {
            CheckList();
        }

        
        if (EditorGUILayout.DropdownButton(new GUIContent("Save"), FocusType.Passive))
        {
            serializedObject.ApplyModifiedProperties();
        }

        if (EditorGUILayout.DropdownButton(new GUIContent("Add"), FocusType.Passive))
        {
            keys.arraySize++;

            keys.GetArrayElementAtIndex(keys.arraySize - 1).stringValue = "TM" + (int.Parse(keys.GetArrayElementAtIndex(keys.arraySize - 2).stringValue.Replace("TM", "")) + 1).ToString();
        }

        if (EditorGUILayout.DropdownButton(new GUIContent("Remove"), FocusType.Passive))
        {
            if (lastSelectedId != -1 && lastSelectedId < keys.arraySize)
            {
                keys.DeleteArrayElementAtIndex(lastSelectedId);
            }
            else
            {
                keys.arraySize--;
            }
        }

        int.TryParse(EditorGUI.TextArea(EditorGUILayout.GetControlRect(), deleteBy.ToString(), EditorStyles.textArea), out deleteBy);

        if (EditorGUILayout.DropdownButton(new GUIContent("RemoveBy"), FocusType.Passive))
        {
            keys.arraySize = deleteBy;
        }

        if (lastLength != keys.arraySize)
        {
            CheckList();
            lastLength = keys.arraySize;
        }
        
        foreach(var v in l2)
        {
           EditorGUILayout.HelpBox(v, MessageType.Warning);
        }
    }
}
