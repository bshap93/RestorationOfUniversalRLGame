using System;
using UnityEngine;

namespace Project.Gameplay.Interactivity
{
    public class ManualInteractablePicker : MonoBehaviour
    {
        public string UniqueID { get; set; }

        void Awake()
        {
            UniqueID = Guid.NewGuid().ToString(); // Generate a unique ID
        }
    }
}
