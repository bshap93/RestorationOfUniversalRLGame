using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Gameplay.ItemManagement
{
    public class CustomInventorySelectionMarker : InventorySelectionMarker
    {
        
        protected override void Update()
        {
            if (EventSystem.current == null)
            {
                return;
            } 
            _currentSelection = EventSystem.current.currentSelectedGameObject;
            if (_currentSelection == null)
            {
                return;
            }

            if (_currentSelection.gameObject.MMGetComponentNoAlloc<InventorySlot>() == null)
            {
                return;
            }

            if (Vector3.Distance(transform.position,_currentSelection.transform.position) > MinimalTransitionDistance)
            {
                if (_originIsNull)
                {
                    _originIsNull=false;
                    _originPosition = transform.position;
                    _originLocalScale = _rectTransform.localScale;
                    _originSizeDelta = _rectTransform.sizeDelta;
                    _originTime = Time.unscaledTime;
                } 
                _deltaTime =  (Time.unscaledTime - _originTime)*TransitionSpeed;
                transform.position= Vector3.Lerp(_originPosition,_currentSelection.transform.position,_deltaTime);
                _rectTransform.localScale = Vector3.Lerp(_originLocalScale, _currentSelection.GetComponent<RectTransform>().localScale,_deltaTime);
                _rectTransform.sizeDelta = Vector3.Lerp(_originSizeDelta, _currentSelection.GetComponent<RectTransform>().sizeDelta, _deltaTime);
            }
            else
            {
                _originIsNull=true;
            }
        }
    }
}
