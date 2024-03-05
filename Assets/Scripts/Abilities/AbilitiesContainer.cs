using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesContainer : MonoBehaviour
{
    [SerializeField] private AbilitySelection[] _abilities;
    [SerializeField] private PlayerActor _player;
    [SerializeField] private AbilitySelectPanel _selectPanel;

    [SerializeField] private List<Ability> _selectedAbilities = new List<Ability>();

    private int alreadySelectedAbilitiesIndex = -1;
    private int _selectionsPendingCount = 0; //чтобы не показывать следующий выбор способности пока предыдущий еще не выбран

    private void Start()
    {
        for (int i = 0; i < _selectedAbilities.Count; i++)
        {
            _selectedAbilities[i].OnSelectedBy(null, _player);
        }
    }

    private void Update()
    {
        var enoughScoreSelectionIndex = -1;
        var score = _player.CurrentScore;

        for (int i = 0; i < _abilities.Length; i++)
        {
            if(_abilities[i].NeededScore <= score)
                enoughScoreSelectionIndex = i;
            else
                break;
        }

        if(enoughScoreSelectionIndex > alreadySelectedAbilitiesIndex && _selectionsPendingCount == 0)
        {
            _abilities[enoughScoreSelectionIndex].Show(_selectPanel, OnAbilitySelected, _player);
            alreadySelectedAbilitiesIndex++;
            _selectionsPendingCount++;
        }

        for (int i = 0; i < _selectedAbilities.Count; i++)
        {
            _selectedAbilities[i].update();
        }
    }

    private void OnAbilitySelected(Ability ability)
    {
        _selectedAbilities.Add(ability);
        _selectionsPendingCount--;
    }

    [System.Serializable]
    public class AbilitySelection
    {
        public int NeededScore;
        public Ability LeftAbility;
        public Ability RightAbility;
        public KeyCode AbilityKey = KeyCode.None;

        public void Show(AbilitySelectPanel panel, Action<Ability> OnAbilitySelected, PlayerActor player)
        {
            panel.Init(LeftAbility.UIData, RightAbility.UIData, delegate (AbilitySelectPanel.AbilityUIData data) 
            {
                //да, говно, но лучше не придумал
                if(data.Equals(LeftAbility.UIData))
                {
                    LeftAbility.OnSelectedBy(this, player);
                    OnAbilitySelected?.Invoke(LeftAbility);
                }
                else if(data.Equals(RightAbility.UIData))
                {
                    RightAbility.OnSelectedBy(this, player);
                    OnAbilitySelected?.Invoke(RightAbility);
                }
            });
        }
    }

    public abstract class Ability : MonoBehaviour
    {
        public AbilitySelectPanel.AbilityUIData UIData;
        [SerializeField] private KeyCode _useKey = KeyCode.None;
        private protected PlayerActor _player;
        private protected bool _selected = false;

        [SerializeField] private float _reloadTime = 0.1f;
        private float _lastUseTime;
        private protected bool TryUse()
        {
            if(!_selected)
                return false;

            if(!Input.GetKey(_useKey))
                return false;

            bool result = Time.time - _lastUseTime > _reloadTime;
            if(result)
                _lastUseTime = Time.time;

            return result;
        }

        public float GetReloadProgress()
        {
            return Mathf.Clamp01((Time.time - _lastUseTime) / _reloadTime);
        }

        public void OnSelectedBy(AbilitySelection selection, PlayerActor player)
        {
            if(selection != null && _useKey == KeyCode.None)
                _useKey = selection.AbilityKey;
            _selected = true;
            _player = player;
        }

        public abstract void update();
    }
}
