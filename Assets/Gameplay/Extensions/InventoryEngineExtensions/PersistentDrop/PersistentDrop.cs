using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Extensions.InventoryEngineExtensions.PersistentDrop
{
    public class PersistentDrop : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        const string _folder = "PersistentDrop";
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
        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "Save")
            {
                SaveToMemory();
                for (var i = 0; i < _count; i++) MMSaveLoadManager.Save(_data[i], _scenes[i], _folder);
                _count = 0;
            }
            else if (mmEvent.EventName == "Load")
            {
                var scene = Scene;
                var i = Array.IndexOf(_scenes, scene, 0, _count);
                if (i == -1) ((Data)MMSaveLoadManager.Load(typeof(Data), scene, _folder))?.Spawn();
                else _data[i].Spawn();
            }
            else if (mmEvent.EventName == "SaveToMemory")
            {
                SaveToMemory();
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

                _data[i].Set(FindObjectsOfType<ItemPicker>().Where(picker => picker.name.EndsWith("(Clone)")));
            }
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
            public List<Vector3> Position = new();
            public List<string> Item = new();
            public List<int> Quantity = new();
            public void Set(IEnumerable<ItemPicker> drops)
            {
                Position.Clear();
                Item.Clear();
                Quantity.Clear();
                foreach (var drop in drops)
                {
                    Position.Add(drop.transform.position);
                    Item.Add(drop.Item.ItemID);
                    Quantity.Add(drop.RemainingQuantity);
                }
            }
            public void Spawn()
            {
                for (var i = 0; i < Position.Count; i++)
                    Instantiate(
                        Resources.Load<InventoryItem>(Inventory._resourceItemPath + Item[i]).Prefab, Position[i],
                        Quaternion.identity).GetComponent<ItemPicker>().Quantity = Quantity[i];
            }
        }
    }
}
