using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class StatusResourceUI : MonoBehaviour
    {
        public MMProgressBar HealthBar;
        public MMProgressBar PrimaryBar;
        public MMProgressBar SecondaryBar;

        float _lastHealth;
        float _lastPrimaryResource;
        float _lastSecondaryResource;
    }
}
