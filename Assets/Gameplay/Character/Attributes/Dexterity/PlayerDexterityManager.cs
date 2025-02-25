using System;
using Gameplay.Character.Attributes.Dexterity;
using UnityEngine;

namespace Gameplay.Character.Attributes
{
    public class PlayerDexterityManager : MonoBehaviour, IPlayerAttributeManager
    {
        public static int PlayerDexterityLevel;
        public static int PlayerDexterityExperiencePoints;
        public static int PlayerDexterityExperiencePointsToNextLevel;

        public static int InitialDexterityLevel;
        public static int InitialDexterityExperiencePoints;
        public static int InitialDexterityExperiencePointsToNextLevel;

        public DexterityBarUpdater dexterityBarUpdater;
        public int GetAttributeValue()
        {
            throw new NotImplementedException();
        }
        public void Awake()
        {
            throw new NotImplementedException();
        }
        public void Start()
        {
            throw new NotImplementedException();
        }
        public void LoadPlayerAttribute()
        {
            throw new NotImplementedException();
        }
        public bool HasSavedData()
        {
            throw new NotImplementedException();
        }


        public static void ResetPlayerDexterity()
        {
            throw new NotImplementedException();
        }
        public static void Initialize(CharacterStatProfile characterStatProfile)
        {
            throw new NotImplementedException();
        }
    }
}
