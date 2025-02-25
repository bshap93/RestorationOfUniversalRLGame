using Gameplay.Character.Attributes.LevelExperienceCurve;
using Gameplay.Extensions.ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Updaters
{
    public abstract class LevelBasedVariableUpdater : ScriptableObject
    {
        [SerializeField] protected IntVariable Level;
        [SerializeField] protected LevelValueCurveVariable LevelValueCurve;

        void OnEnable()
        {
            UpdateVariable();
            Level.Changed += UpdateVariable;
        }
        void OnDisable()
        {
            Level.Changed -= UpdateVariable;
        }

        protected abstract void UpdateVariable();
    }
}
