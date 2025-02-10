using HighlightPlus;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class MouseSelectionHighlight : MonoBehaviour
    {
        public Transform objectToSelect;

        HighlightManager hm;

        void Start()
        {
            hm = Misc.FindObjectOfType<HighlightManager>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) hm.SelectObject(objectToSelect);
            if (Input.GetKeyDown(KeyCode.Alpha2)) hm.ToggleObject(objectToSelect);
            if (Input.GetKeyDown(KeyCode.Alpha3)) hm.UnselectObject(objectToSelect);
        }
    }
}
