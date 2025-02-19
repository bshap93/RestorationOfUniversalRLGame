using System;
using UnityEngine;

namespace Project.Gameplay.Transitions
{
    public class FloorTriggerDetector : MonoBehaviour
    {
        FloorManager _floorManager;

        [Obsolete("Obsolete")]
        void Start()
        {
            _floorManager = FindObjectOfType<FloorManager>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (_floorManager == null) return;
            var floorCollider = other.GetComponent<FloorCollider>();
            if (floorCollider != null)
                _floorManager.SetFloorVisibility(floorCollider.FloorLevel.floorName);
        }
    }
}
