using System.Collections;
using Gameplay.ItemsInteractions;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Player.Inventory
{
    public class SaveableLoot : Loot
    {
        static int _globalSpawnedItemCount;

        protected override void Awake()
        {
            base.Awake();
            _globalSpawnedItemCount = ES3.Load("GlobalSpawnedItemCount", 0);
        }

        void OnApplicationQuit()
        {
            ES3.Save("GlobalSpawnedItemCount", _globalSpawnedItemCount);
            Debug.Log($"Saved global loot counter: {_globalSpawnedItemCount}");
        }

        public string GetLootPrefabName()
        {
            var prefab = GetLootPrefab();
            return prefab?.name ?? "";
        }

        public GameObject GetLootPrefab()
        {
            switch (LootMode)
            {
                case LootModes.Unique:
                    return GameObjectToLoot;
                case LootModes.LootTable:
                    return LootTable?.ObjectsToLoot.Count > 0 ? LootTable.ObjectsToLoot[0].Loot : null;
                case LootModes.LootTableScriptableObject:
                    return LootTableSO?.LootTable.ObjectsToLoot.Count > 0
                        ? LootTableSO.LootTable.ObjectsToLoot[0].Loot
                        : null;
                default:
                    return null;
            }
        }

        protected override void Spawn(GameObject gameObjectToSpawn)
        {
            // Apply spawn properties first to set the correct position
            MMSpawnAround.ApplySpawnAroundProperties(gameObjectToSpawn, SpawnProperties, transform.position);

            // Now instantiate the object at the correct position
            _spawnedObject = PoolLoot ? GetPooledObject(gameObjectToSpawn) : Instantiate(gameObjectToSpawn);

            if (_spawnedObject != null)
            {
                var itemPicker = _spawnedObject.GetComponent<ManualItemPicker>();
                if (itemPicker != null)
                {
                    // Generate unique ID using base name and counter
                    var itemBaseName = itemPicker.Item?.ItemName?.Replace(" ", "") ?? "Item";
                    itemPicker.UniqueID = $"{itemBaseName}_{_globalSpawnedItemCount:D3}";


                    _globalSpawnedItemCount++;

                    // Save the position after the object has been properly positioned
                    if (!PickableManager.IsItemPicked(itemPicker.UniqueID))
                    {
                        var prefabName = gameObjectToSpawn.name;
                        PickableManager.SaveItemPosition(
                            itemPicker.UniqueID,
                            _spawnedObject.transform.position,
                            prefabName);

                        Debug.Log(
                            $"Saved loot {itemPicker.UniqueID} at actual position: {_spawnedObject.transform.position}");
                    }
                }

                _spawnedObject.gameObject.SetActive(true);
                _spawnedObject.SendMessage("OnInstantiate", SendMessageOptions.DontRequireReceiver);
            }
        }

        GameObject GetPooledObject(GameObject gameObjectToSpawn)
        {
            switch (LootMode)
            {
                case LootModes.Unique:
                    return _simplePooler.GetPooledGameObject();
                case LootModes.LootTable:
                case LootModes.LootTableScriptableObject:
                    return _multipleObjectPooler.GetPooledGameObject();
                default:
                    return null;
            }
        }

        protected override IEnumerator SpawnLootCo()
        {
            if (Delay > 0f) yield return new WaitForSeconds(Delay);

            // _spawnedItemCount = 0; // Reset counter before spawning batch
            var randomQuantity = Random.Range((int)Quantity.x, (int)Quantity.y + 1);
            for (var i = 0; i < randomQuantity; i++)
            {
                var objectToSpawn = GetObject();
                if (objectToSpawn != null) Spawn(objectToSpawn);
            }

            ES3.Save("GlobalSpawnedItemCount", _globalSpawnedItemCount);

            LootFeedback?.PlayFeedbacks();
        }
    }
}
