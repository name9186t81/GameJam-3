using UnityEngine;
using System.Collections.ObjectModel;

[CreateAssetMenu]
public class LanguageKeysData : ScriptableObject
{
    [SerializeField] private string[] _keys;

    public ReadOnlyCollection<string> Texts => System.Array.AsReadOnly(_keys);
}

