using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public class AbilitiesUI : MonoBehaviour
    {
        [SerializeField] private AbilityUIPart _part;
        [SerializeField] private AbilitiesContainer _container;

        private void Awake()
        {
            _container.OnAbilitySelected += OnAbilitySelected;
        }

        private void OnAbilitySelected(AbilitiesContainer.Ability ability)
        {
            SpawnNewPart(ability);
        }

        private void SpawnNewPart(AbilitiesContainer.Ability ability)
        {
            var part = Instantiate(_part, _part.transform.parent);
            part.Init(ability);
        }
    }
}