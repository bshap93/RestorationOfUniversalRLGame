using Gameplay.Extensions.ProgressionSystem.Scripts.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace ProgressionSystem.Scripts.Events
{
    public class OnIntVariableChange : MonoBehaviour
    {
        [SerializeField] IntVariable IntVariable;
        [SerializeField] UnityEvent OnIntVariableChangeEvent;
        void OnEnable()
        {
            IntVariable.Changed += OnIntVariableChangeEvent.Invoke;
        }
        void OnDisable()
        {
            IntVariable.Changed -= OnIntVariableChangeEvent.Invoke;
        }
    }
}
