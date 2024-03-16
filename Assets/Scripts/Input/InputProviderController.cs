using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInput
{
    [DefaultExecutionOrder(-5000)]
    public class InputProviderController : MonoBehaviour
    {
        [SerializeField] private InputProvider _defaultProvider;
        [SerializeField] private InputProvider _mobileProvider;

        private InputProvider CurrentProvider;

        private void Awake()
        {
            CurrentProvider = _defaultProvider;

            if(Application.isMobilePlatform)
            {
                CurrentProvider = _mobileProvider;
            }

            CurrentProvider.Init();

            ServiceLocator.Register(CurrentProvider);
        }

        private void Update()
        {
            CurrentProvider.Tick();
        }
    }

    [System.Serializable]
    public abstract class InputProvider : MonoBehaviour, IService
    {
        public abstract float Horizontal { get; }
        public abstract float Vertical { get; }

        public abstract event Action<int> AbilityUsed;

        public abstract void Init();
        public abstract void Tick();
    }
}