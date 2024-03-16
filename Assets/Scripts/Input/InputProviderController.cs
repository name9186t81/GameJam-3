using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

            //if(Application.isMobilePlatform)
#if UNITY_ANDROID
            CurrentProvider = _mobileProvider;
#endif

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

        public abstract event Action<int, PointerEventData> AbilityUsed;
        public abstract event Action<ActionType> Action;

        public abstract void Init();
        public abstract void Tick();

        [SerializeField] private protected bool _usingMobileInput;
        public bool UsingMobileInput => _usingMobileInput;

        [System.Serializable]
        public enum ActionType
        {
            Pause
        }
    }
}