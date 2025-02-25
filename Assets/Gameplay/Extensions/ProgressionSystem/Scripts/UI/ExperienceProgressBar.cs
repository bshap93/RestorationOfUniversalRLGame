using Gameplay.Extensions.ProgressionSystem.Scripts.Core;
using MoreMountains.Tools;
using UnityEngine;

namespace ProgressionSystem.Scripts.UI
{
    public class ExperienceProgressBar : MonoBehaviour
    {
        [SerializeField] Progression Progression;
        MMProgressBar _bar;

        void Awake()
        {
            _bar = GetComponent<MMProgressBar>();
        }
        void OnEnable()
        {
            _bar.SetBar(Progression.LevelExperience, 0, Progression.NextLevelExperience);
            Progression.Progressed += UpdateBar;
        }
        void OnDisable()
        {
            Progression.Progressed -= UpdateBar;
        }

        void UpdateBar()
        {
            if (Progression.NextLevelExperience > 0)
                _bar.UpdateBar(Progression.LevelExperience, 0, Progression.NextLevelExperience);
            else
                _bar.SetBar01(1);
        }
    }
}
