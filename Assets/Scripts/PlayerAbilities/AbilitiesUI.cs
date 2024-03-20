using Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class AbilitiesUI : MonoBehaviour
    {
        [SerializeField] private AbilityUIPart _part;
        [SerializeField] private AbilitiesContainer _container;

        public event Action<AbilityUIPart, int> OnPartSpawned;

        private void Awake()
        {
            _part.gameObject.SetActive(false);
        }

        public void Init()
        {
            gameObject.SetActive(true);
            _container.AbilitySelected += OnAbilitySelected;
        }

        private void OnAbilitySelected(IAbility ability, AbilitySelectPanel.AbilityUIData data, int id)
        {
            var part = SpawnNewPart(ability, data);
            OnPartSpawned?.Invoke(part, id);
        }

        private AbilityUIPart SpawnNewPart(IAbility ability, AbilitySelectPanel.AbilityUIData data)
        {
            var part = Instantiate(_part, _part.transform.parent);

            part.Init(ability, data);

            return part;
        }
    }
}