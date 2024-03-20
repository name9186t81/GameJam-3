using Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    [CreateAssetMenu(fileName = nameof(AbilitiesConfiguration), menuName = "GameJam/Abilities Configuration")]
    public class AbilitiesConfiguration : ScriptableObject
    {
        public AbilityData[] DefaultAbilities;
        public AbilitySelection[] SelectableAbilities;

        [System.Serializable]
        public class AbilityData
        {
            public AbilityBuilder Builder;
            public AbilitySelectPanel.AbilityUIData UIData;
        }

        [System.Serializable]
        public class AbilitySelection
        {
            public float NeededScore;
            public AbilityData LeftAbility;
            public AbilityData RightAbility;
        }
    }
}