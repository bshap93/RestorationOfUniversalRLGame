﻿using HighlightPlus;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;

namespace Project.Gameplay.Interactivity
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

            if (_selectedObjectController == null) return false;
            if (_selectedObjectController is CookingStationController) Debug.Log("CookingStationController selected");


            _selectedObjectController?.OnSelectedItem();

            return true;
        }

        bool OnObjectUnSelected(GameObject go)
        {
            if (_selectedObjectController == null)
                _selectedObjectController = go.GetComponentInParent<ISelectableTrigger>();

            if (_selectedObjectController != null) _selectedObjectController.OnUnSelectedItem();


            return true;
        }
    }
}
