// WeaponAnimationManager.cs

using UnityEngine;

namespace Project.Animation_Effects.CharacterAnimation
{
    public class WeaponAnimationManager : MonoBehaviour
    {
        RuntimeAnimatorController _currentAnimatorController;
        string _currentWeaponID;

        public void StoreCurrentWeapon(string weaponID, RuntimeAnimatorController controller)
        {
            _currentWeaponID = weaponID;
            _currentAnimatorController = controller;
        }

        public bool HasStoredWeapon()
        {
            return !string.IsNullOrEmpty(_currentWeaponID) && _currentAnimatorController != null;
        }

        public string GetCurrentWeaponID()
        {
            return _currentWeaponID;
        }

        public RuntimeAnimatorController GetCurrentAnimatorController()
        {
            return _currentAnimatorController;
        }

        public void ClearStoredWeapon()
        {
            _currentWeaponID = null;
            _currentAnimatorController = null;
        }
    }
}
