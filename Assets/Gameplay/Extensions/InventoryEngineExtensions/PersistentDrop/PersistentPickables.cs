using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using Project.Gameplay.Player.Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Gameplay.Extensions.InventoryEngineExtensions.PersistentDrop
{
    public class PersistentPickables : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        const string _folder = "PersistentPickables";
        const int _maxScenes = 30;
        static readonly Data[] _data = new Data[_maxScenes];
        static readonly string[] _scenes = new string[_maxScenes];
        static int _count;
        static string Scene => SceneManager.GetActiveScene().name;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent itemEvent)
        {
            if (itemEvent.EventName == "Save")
            {
                SaveToMemory();
                for (var i = 0; i < _count; i++) MMSaveLoadManager.Save(_data[i], _scenes[i], _folder);
                _count = 0;
            }
            else if (itemEvent.EventName == "Load")
            {
                var scene = Scene;
                var i = Array.IndexOf(_scenes, scene, 0, _count);
                if (i == -1)
                    ((Data)MMSaveLoadManager.Load(typeof(Data), scene, _folder))?.ProcessPickables();
                else
                    _data[i].ProcessPickables();
            }
            else if (itemEvent.EventName == "SaveToMemory")
            {
                SaveToMemory();
            }
        }

        void SaveToMemory()
        {
            var scene = Scene;
            var i = Array.IndexOf(_scenes, scene, 0, _count);
            if (i == -1)
            {
                i = _count++;
                _scenes[i] = scene;
            }

            _data[i].Set(
                FindObjectsOfType<ManualItemPicker>()
                    .Where(picker => picker.gameObject.scene.name == scene));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _count = 0;
            for (var i = 0; i < _maxScenes; i++) _data[i] = new Data();
        }

        [Serializable]
        class Data
        {
            public List<string> UniqueID = new();
            public List<bool> WasPickedUp = new();

            public void Set(IEnumerable<ManualItemPicker> pickables)
            {
                // Only store new pickables that we haven't tracked before
                foreach (var pickable in pickables)
                {
                    if (pickable == null || string.IsNullOrEmpty(pickable.UniqueID))
                        continue;

                    // If we're not already tracking this pickable, add it
                    if (!UniqueID.Contains(pickable.UniqueID))
                    {
                        UniqueID.Add(pickable.UniqueID);
                        // Default to not picked up for new items
                        WasPickedUp.Add(false);
                    }
                }
            }

            [Obsolete("Obsolete")]
            public void ProcessPickables()
            {
                var pickables = FindObjectsOfType<ManualItemPicker>();

                foreach (var pickable in pickables)
                {
                    if (pickable == null || string.IsNullOrEmpty(pickable.UniqueID))
                        continue;

                    var index = UniqueID.IndexOf(pickable.UniqueID);
                    if (index != -1 && WasPickedUp[index])
                        // If this item was previously picked up, disable it
                        pickable.gameObject.SetActive(false);
                }
            }

            // Add this method to mark an item as picked up
            public void MarkAsPickedUp(string uniqueId)
            {
                var index = UniqueID.IndexOf(uniqueId);
                if (index != -1) WasPickedUp[index] = true;
            }
        }
    }
}
