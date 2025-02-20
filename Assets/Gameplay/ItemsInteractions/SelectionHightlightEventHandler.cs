﻿using Gameplay.Crafting.Cooking;
using HighlightPlus;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class SelectionHighlightEventHandler : MonoBehaviour
    {
        public bool SelectionByClickEnabled;
        ISelectableTrigger _selectedObjectController;
        void Start()
        {
            HighlightManager.instance.OnObjectSelected += OnObjectSelected;
            HighlightManager.instance.OnObjectUnSelected += OnObjectUnSelected;
        }

        bool OnObjectSelected(GameObject go)
        {
            _selectedObjectController = go.GetComponentInParent<ISelectableTrigger>();

            if (_selectedObjectController == null)
            {
                Debug.Log("No selectable trigger found");
                return false;
            }

            if (_selectedObjectController is CookingStationController controller)
            {
                Debug.Log("ShowStation");
                controller.ShowStationChoicePanel();
            }


            _selectedObjectController?.OnSelectedItem();

            return true;
        }

        bool OnObjectUnSelected(GameObject go)
        {
            if (_selectedObjectController == null)
                _selectedObjectController = go.GetComponentInParent<ISelectableTrigger>();

            if (_selectedObjectController is CookingStationController controller) controller.HideStationChoicePanel();


            if (_selectedObjectController != null) _selectedObjectController.OnUnSelectedItem();


            return true;
        }
    }
}
