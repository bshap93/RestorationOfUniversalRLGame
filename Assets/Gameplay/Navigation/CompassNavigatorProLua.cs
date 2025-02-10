using CompassNavigatorPro;
using MoreMountains.Feedbacks;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Gameplay.Navigation
{
    public class CompassNavigatorProLua : MonoBehaviour
    {
        [Tooltip("Typically leave unticked so temporary Dialogue Managers don't unregister your functions.")]
        public bool unregisterOnDisable;

        public MMFeedbacks poiMadeVisibleFeedback;
        public MMFeedbacks poiMadeVisitedFeedback;

        void OnEnable()
        {
            Lua.RegisterFunction(
                nameof(poiSetVisible), this, SymbolExtensions.GetMethodInfo(() => poiSetVisible("", false)));

            Lua.RegisterFunction(
                nameof(poiSetVisited), this, SymbolExtensions.GetMethodInfo(() => poiSetVisited("", false)));

            Lua.RegisterFunction(nameof(poiIsVisited), this, SymbolExtensions.GetMethodInfo(() => poiIsVisited("")));
        }

        void OnDisable()
        {
            if (unregisterOnDisable)
            {
                // Remove the functions from Lua: (Replace these lines with your own.)
                Lua.UnregisterFunction(nameof(poiSetVisible));
                Lua.UnregisterFunction(nameof(poiSetVisited));
                Lua.UnregisterFunction(nameof(poiIsVisited));
            }
        }

        CompassProPOI FindPOI(string poiName)
        {
            var go = GameObjectUtility.GameObjectHardFind(poiName);
            var poi = go != null ? go.GetComponentInChildren<CompassProPOI>() : null;
            if (poi != null) return poi;
            var poiSavers = FindObjectsOfType<POISaver>();
            for (var i = 0; i < poiSavers.Length; i++)
            {
                var poiSaver = poiSavers[i];
                if (!string.IsNullOrEmpty(poiSaver.key) && string.Equals(poiSaver.key, poiName))
                    return poiSaver.GetComponent<CompassProPOI>();
            }

            return null;
        }


        public void poiSetVisible(string poiName, bool value)
        {
            var poi = FindPOI(poiName);
            if (poi == null)
            {
                Debug.LogWarning($"Dialogue System: poiSetVisible({poiName}, {value}) can't find POI.");
            }
            else
            {
                poi.enabled = value;
                poi.isVisible = value;
                if (value) poiMadeVisibleFeedback?.PlayFeedbacks();
            }
        }

        public void poiSetVisited(string poiName, bool value)
        {
            var poi = FindPOI(poiName);
            if (poi == null)
            {
                Debug.LogWarning($"Dialogue System: poiSetVisited({poiName}, {value}) can't find POI.");
            }
            else
            {
                poi.isVisited = value;
                if (value) poiMadeVisitedFeedback?.PlayFeedbacks();
            }
        }

        public bool poiIsVisited(string poiName)
        {
            var poi = FindPOI(poiName);
            if (poi == null)
            {
                Debug.LogWarning($"Dialogue System: poiIsVisited({poiName}) can't find POI.");
                return false;
            }

            return poi.isVisited;
        }
    }
}
