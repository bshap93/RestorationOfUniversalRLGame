using UnityEngine;

namespace Gameplay.ItemsInteractions.Informable
{
    public class InformableSelectableTrigger : MonoBehaviour
    {
        public InformableItem Informable;

        ManualInformationPicker _informationPicker;

        void Awake()
        {
            _informationPicker = GetComponent<ManualInformationPicker>();

            if (_informationPicker == null)
                _informationPicker = gameObject.AddComponent<ManualInformationPicker>();

            _informationPicker.Informable = Informable;
        }
    }
}
