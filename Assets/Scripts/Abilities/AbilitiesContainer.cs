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

    public List<Ability> SelectedAbilities = new List<Ability>(); //публик не нужен но оставлю для дебага

    private int alreadySelectedAbilitiesIndex = -1;
    private int _selectionsPendingCount = 0; //чтобы не показывать следующий выбор способности пока предыдущий еще не выбран

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

        for (int i = 0; i < SelectedAbilities.Count; i++)
        {
            SelectedAbilities[i].update();
        }
    }

    private void OnAbilitySelected(Ability ability)
    {
        SelectedAbilities.Add(ability);
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
        [SerializeField] private protected KeyCode _useKey = KeyCode.None;
        private protected PlayerActor _player;
        private protected bool _selected = false;

        public void OnSelectedBy(AbilitySelection selection, PlayerActor player)
        {
            if(_useKey == KeyCode.None)
                _useKey = selection.AbilityKey;
            _selected = true;
            _player = player;
        }

        public abstract void update();
    }
}
