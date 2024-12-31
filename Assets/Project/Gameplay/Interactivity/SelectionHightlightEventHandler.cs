using HighlightPlus;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.ItemManagement.Triggers;
using UnityEngine;

namespace Project.Gameplay.Interactivity
{
    public class SelectionHighlightEventHandler : MonoBehaviour
    {
        ItemPreviewTrigger _itemPreviewTrigger;
        void Start()
        {
            HighlightManager.instance.OnObjectSelected += OnObjectSelected;
            HighlightManager.instance.OnObjectUnSelected += OnObjectUnSelected;
        }

        bool OnObjectSelected(GameObject go)
        {
            _itemPreviewTrigger = go.GetComponentInParent<ItemPreviewTrigger>();

            if (_itemPreviewTrigger == null) return false;


            if (_itemPreviewTrigger != null) _itemPreviewTrigger.OnSelectedItem();

            return true;
        }

        bool OnObjectUnSelected(GameObject go)
        {
            if (_itemPreviewTrigger == null)
                _itemPreviewTrigger = go.GetComponentInParent<ItemPreviewTrigger>();

            if (_itemPreviewTrigger != null) _itemPreviewTrigger.OnUnSelectedItem();


            return true;
        }
    }
}
