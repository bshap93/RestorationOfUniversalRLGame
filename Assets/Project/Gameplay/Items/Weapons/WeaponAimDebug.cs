using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponAimDebug : MonoBehaviour
{
    [FormerlySerializedAs("_brain")] public AIBrain brain;
    protected AltCharacterHandleWeapon _handleWeapon;
    protected WeaponAim _weaponAim;

    void Start()
    {
        _handleWeapon = GetComponent<AltCharacterHandleWeapon>();
    }

    void Update()
    {
        if (_handleWeapon?.CurrentWeapon == null) return;

        // Get and cache WeaponAim reference
        if (_weaponAim == null)
        {
            _weaponAim = _handleWeapon.CurrentWeapon.GetComponent<WeaponAim>();
            if (_weaponAim != null)
                Debug.Log(
                    "Weapon Aim found:\n" +
                    $"Aim Control: {_weaponAim.AimControl}\n" +
                    $"Rotation Mode: {_weaponAim.RotationMode}\n" +
                    $"Rotation Speed: {_weaponAim.WeaponRotationSpeed}");
        }

        // Debug target information
        if (brain?.Target != null)
        {
            var toTarget = (brain.Target.position - transform.position).normalized;
            Debug.DrawLine(transform.position, transform.position + toTarget * 3f, Color.red);

            if (_handleWeapon?.CurrentWeapon != null)
                Debug.DrawLine(
                    _handleWeapon.CurrentWeapon.transform.position,
                    _handleWeapon.CurrentWeapon.transform.position + _handleWeapon.CurrentWeapon.transform.right * 2f,
                    Color.blue);
        }
    }

    void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        if (_weaponAim != null)
        {
            GUILayout.Label($"Weapon Direction: {_weaponAim.CurrentAngle}");
            if (brain?.Target != null)
            {
                var toTarget = (brain.Target.position - transform.position).normalized;
                var targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                GUILayout.Label($"Target Angle: {targetAngle}");
            }
        }

        GUILayout.EndArea();
    }
}
