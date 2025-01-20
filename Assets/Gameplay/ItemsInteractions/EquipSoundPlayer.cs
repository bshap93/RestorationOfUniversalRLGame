using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class EquipSoundPlayer : MonoBehaviour
{
    public AudioClip equipSwordSound;


    // Update is called once per frame
    public void EquipSwordSound()
    {
        if (equipSwordSound != null)
        {
            MMSoundManager.Instance.PlaySound(equipSwordSound, new MMSoundManagerPlayOptions());
        }
       
    }
}
