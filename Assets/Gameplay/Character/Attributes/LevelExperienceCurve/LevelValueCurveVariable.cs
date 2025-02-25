using System;
using UnityEngine;

namespace ProgressionSystem.Scripts.Variables
{
    [CreateAssetMenu(fileName = "LevelValueCurve", menuName = "Progression/Variables/LevelValueCurve")]
    public class LevelValueCurveVariable : ScriptableObject
    {
        [SerializeField] AnimationCurve LevelValueCurve = AnimationCurve.EaseInOut(1, 0, 100, 100000);

        [Header("Display")] public bool DisplayIntValues;
        public string ValueRepresents = "XP";
        public Action Changed;

        public int MinLevel => (int)LevelValueCurve.keys[0].time;
        public int MaxLevel => (int)LevelValueCurve.keys[LevelValueCurve.length - 1].time;
        void OnValidate()
        {
            Changed?.Invoke();
        }
        public int EvaluateInt(int level)
        {
            return (int)LevelValueCurve.Evaluate(level);
        }
        public float Evaluate(int level)
        {
            return LevelValueCurve.Evaluate(level);
        }

        public int GetLevelGivenExperience(float experience)
        {
            for (var i = 0; i < LevelValueCurve.length; i++)
                if (LevelValueCurve.Evaluate(i) > experience)
                    return i;

            return MaxLevel;
        }
    }
}
