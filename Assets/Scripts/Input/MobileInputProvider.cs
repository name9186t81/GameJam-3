using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInput
{
    public class MobileInputProvider : InputProvider
    {
        public override float Horizontal => throw new NotImplementedException();

        public override float Vertical => throw new NotImplementedException();

        public override event Action<int> AbilityUsed;

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Tick()
        {
            throw new NotImplementedException();
        }
    }
}