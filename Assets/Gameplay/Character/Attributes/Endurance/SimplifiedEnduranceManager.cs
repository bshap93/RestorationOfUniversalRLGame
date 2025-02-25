using System;
using Core.Events;
using UnityEngine;

namespace Gameplay.Character.Attributes
{
    public class SimplifiedEnduranceManager :  BasePlayerAttributeManager
    {
        public EnduranceUIUpdater enduranceUIUpdater;

        public static void ResetPlayerEndurance()
        {
            throw new NotImplementedException();
        }
        public static void Initialize(CharacterStatProfile characterStatProfile)
        {
            throw new NotImplementedException();
        }
        protected override AttributeInQuestion AttributeType { get; }
        protected override string SaveFileName { get; }
        public override void ResetAttribute()
        {
            throw new NotImplementedException();
        }
    }
}
