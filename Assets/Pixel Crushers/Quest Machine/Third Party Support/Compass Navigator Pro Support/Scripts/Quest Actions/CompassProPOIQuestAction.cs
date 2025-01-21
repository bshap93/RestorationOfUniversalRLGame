// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    public class CompassProPOIQuestAction : QuestAction
    {

        [Tooltip("GameObject name or POI Saver key of POI to change.")]
        [SerializeField]
        private StringField m_poiName;

        public enum POIOperation { SetVisible, SetInvisible, SetVisited, SetUnvisited }

        [SerializeField]
        private POIOperation m_operation = POIOperation.SetVisible;

        public StringField poiName
        {
            get { return m_poiName; }
            set { m_poiName = value; }
        }

        public POIOperation operation
        {
            get { return m_operation; }
            set { m_operation = value; }
        }

        public override string GetEditorName()
        {
            return operation + " " + poiName;
        }

        public override void Execute()
        {
            base.Execute();
            var poi = FindPOI();
            if (poi == null)
            {
                Debug.LogWarning("Quest Machine: CompassProPOIQuestAction can't find a POI GameObject or POI Saver key named '" + poiName + "'.");
            }
            else
            {
                if (QuestMachine.debug) Debug.LogWarning("Quest Machine: " + operation + " POI '" + poiName + "'.");
                switch (operation)
                {
                    case POIOperation.SetVisible:
                        poi.enabled = true;
                        break;
                    case POIOperation.SetInvisible:
                        poi.enabled = false;
                        break;
                    case POIOperation.SetVisited:
                        poi.isVisited = true;
                        break;
                    case POIOperation.SetUnvisited:
                        poi.isVisited = false;
                        break;
                }
            }
        }

        private CompassNavigatorPro.CompassProPOI FindPOI()
        {
            var searchFor = StringField.GetStringValue(poiName);
            var go = GameObjectUtility.GameObjectHardFind(searchFor);
            var poi = (go != null) ? go.GetComponentInChildren<CompassNavigatorPro.CompassProPOI>() : null;
            if (poi != null) return poi;
            var poiSavers = FindObjectsOfType<POISaver>();
            for (int i = 0; i < poiSavers.Length; i++)
            {
                var poiSaver = poiSavers[i];
                if (!string.IsNullOrEmpty(poiSaver.key) && string.Equals(poiSaver.key, searchFor))
                {
                    return poiSaver.GetComponent<CompassNavigatorPro.CompassProPOI>();
                }
            }
            return null;
        }

    }

}
