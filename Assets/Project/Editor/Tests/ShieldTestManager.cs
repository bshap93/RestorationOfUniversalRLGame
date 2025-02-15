using System.Linq;
using Gameplay.Combat.Shields;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Shields;
using UnityEngine;

namespace Project.Gameplay.Combat
{
    public class ShieldTestManager : MonoBehaviour
    {
        [Header("Test References")] public Character TestCharacter;
        public Shield TestShieldPrefab;


        [Header("Debug")] [MMInspectorButton("EquipShield")]
        public bool EquipShieldButton;
        [MMInspectorButton("UnequipShield")] public bool UnequipShieldButton;
        [MMInspectorButton("TestDamage")] public bool TestDamageButton;

        protected CharacterHandleShield _shieldHandler;

        protected virtual void Start()
        {
        }

        protected virtual void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            if (_shieldHandler?.CurrentShield != null)
                GUILayout.Label($"Shield Health: {_shieldHandler.CurrentShield.CurrentShieldHealth}");
            else
                GUILayout.Label("No Shield Equipped");

            GUILayout.EndArea();
        }

        public virtual void MarryPlayerToReference()
        {
            TestCharacter = FindObjectsOfType<Character>().Where(c => c.name == "Player1").FirstOrDefault();


            if (_shieldHandler == null) _shieldHandler = TestCharacter.FindAbility<CharacterHandleShield>();
        }

        public virtual void EquipShield()
        {
            if (_shieldHandler != null && TestShieldPrefab != null) _shieldHandler.EquipShield(TestShieldPrefab);
        }

        public virtual void UnequipShield()
        {
            if (_shieldHandler != null) _shieldHandler.EquipShield(null);
        }

        public virtual void TestDamage()
        {
            if (_shieldHandler?.CurrentShield != null) _shieldHandler.CurrentShield.ProcessDamage(20f);
        }
    }
}
