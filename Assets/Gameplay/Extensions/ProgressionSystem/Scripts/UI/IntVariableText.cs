using Gameplay.Extensions.ProgressionSystem.Scripts.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressionSystem.Scripts.UI
{
    public class IntVariableText : MonoBehaviour
    {
        [SerializeField] IntVariable IntVariable;
        [SerializeField] int Offset;
        [SerializeField] string Prefix = "LVL ";
        Text _text;

        void Awake()
        {
            _text = GetComponent<Text>();
        }

        void OnEnable()
        {
            UpdateText();
            IntVariable.Changed += UpdateText;
        }
        void OnDisable()
        {
            IntVariable.Changed -= UpdateText;
        }
        void UpdateText()
        {
            _text.text = Prefix + (IntVariable.Value + Offset);
        }
    }
}
