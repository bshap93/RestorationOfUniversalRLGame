using System;
using Gameplay.Extensions.ProgressionSystem.Scripts.Variables;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace Gameplay.Extensions.ProgressionSystem.Scripts.Core
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Progression/Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] IntVariable Experience;
        [SerializeField] IntVariable Level;
        [SerializeField] LevelValueCurveVariable LevelExperienceCurve;
        [SerializeField] bool ResetExperienceOnEnable = true;
        public Action Progressed;
        public int LevelExperience => Experience.Value - LevelExperienceCurve.EvaluateInt(Level.Value);
        public int NextLevelExperience => LevelExperienceCurve.EvaluateInt(Level.Value + 1) -
                                          LevelExperienceCurve.EvaluateInt(Level.Value);

        void OnEnable()
        {
            if (ResetExperienceOnEnable)
                Experience.Value = LevelExperienceCurve.EvaluateInt(LevelExperienceCurve.MinLevel);

            Level.Value = 0;
            UpdateLevel();

            Experience.Changed += UpdateLevel;
            LevelExperienceCurve.Changed += UpdateLevel;
        }

        void OnDisable()
        {
            Experience.Changed -= UpdateLevel;
            LevelExperienceCurve.Changed -= UpdateLevel;
        }

        void UpdateLevel()
        {
            while (Experience.Value >= LevelExperienceCurve.EvaluateInt(Level.Value + 1) &&
                   Level.Value < LevelExperienceCurve.MaxLevel) Level.Value++;

            Progressed?.Invoke();
        }
    }
}
