using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboConfig", menuName = "GameJam/Combo Config")]
public class ComboConfig : ScriptableObject
{
    [SerializeField] private float _comboResetTime = 3;

    [SerializeField] private ComboMultiplerValue _speedMultipler = new ComboMultiplerValue(0.024f, 1.8f);
    [SerializeField] private ComboMultiplerValue _abilityReloadSpeedMultipler = new ComboMultiplerValue(0.03f, 1.8f);

    public float ComboResetTime => _comboResetTime;
    public ComboMultiplerValue SpeedMultipler => _speedMultipler;
    public ComboMultiplerValue AbilityReloadSpeedMultipler => _abilityReloadSpeedMultipler;

    [System.Serializable]
    public class ComboMultiplerValue
    {
        [SerializeField] private float _addPerCombo;
        [SerializeField][Min(1)] private float _multLimit;

        public ComboMultiplerValue(float addPerCombo, float multLimit)
        {
            _addPerCombo = addPerCombo;
            _multLimit = multLimit;
        }

        public float GetValue(int comboCount) => Mathf.Min(1 + _addPerCombo * comboCount, _multLimit);
    }
}
