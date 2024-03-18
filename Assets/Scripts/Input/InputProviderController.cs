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


#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

        private void Awake()
        {
            CurrentProvider = _defaultProvider;

            var isMobile = false;

#if !UNITY_EDITOR && UNITY_WEBGL
            isMobile = IsMobile();
#endif

            if(Application.isMobilePlatform)
                isMobile = true;

            if(isMobile)
                CurrentProvider = _mobileProvider;

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