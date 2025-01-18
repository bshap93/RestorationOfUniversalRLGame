using HighlightPlus;
using UnityEngine;

namespace Project.Gameplay.Interactivity
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) hm.SelectObject(objectToSelect);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) hm.ToggleObject(objectToSelect);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) hm.UnselectObject(objectToSelect);
        }
    }
}
