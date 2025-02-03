using System;
using System.Collections;
using Gameplay.ItemsInteractions;
using MoreMountains.TopDownEngine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Player.Inventory
{
    public class SaveableLoot : Loot
    {
        protected override IEnumerator SpawnLootCo()
        {
            if (Delay > 0f)
            {
                yield return new WaitForSeconds(Delay);
                Debug.Log("Delay complete. Spawning loot...");
            }

            var randomQuantity = Random.Range((int)Quantity.x, (int)Quantity.y + 1);
            for (var i = 0; i < randomQuantity; i++) SpawnOneLoot();

            LootFeedback?.PlayFeedbacks();

            yield return null;
        }

        public override void SpawnOneLoot()
        {
            _objectToSpawn = GetObject();
            if (_objectToSpawn == null) return;
            if (LimitedLootQuantity && RemainingQuantity <= 0) return;

            var spawnPosition = transform.position + new Vector3(0, 0.5f, 0);

            // Check if this loot was already picked up
            var lootKey = $"{gameObject.name}_{spawnPosition.x}_{spawnPosition.y}_{spawnPosition.z}";
            if (PickableManager.IsItemPicked(lootKey))
            {
                Debug.Log($"Loot {lootKey} was already picked up. Skipping spawn.");
                return;
            }

            _spawnedObject = Instantiate(_objectToSpawn, spawnPosition, Quaternion.identity);

            if (_spawnedObject != null)
            {
                var itemPicker = _spawnedObject.GetComponent<ManualItemPicker>();
                if (itemPicker != null)
                {
                    // Assign UniqueID if missing
                    if (string.IsNullOrEmpty(itemPicker.UniqueID))
                    {
                        itemPicker.UniqueID = Guid.NewGuid().ToString();
                        Debug.Log($"Assigned UniqueID to loot: {itemPicker.UniqueID}");
                    }

                    // Save loot spawn **after** it has actually spawned
                    SaveLootDrop(lootKey, spawnPosition);
                }
            }
        }

        void SaveLootDrop(string uniqueID, Vector3 position)
        {
            // Only save loot if it hasn’t already been picked
            if (!PickableManager.IsItemPicked(uniqueID))
            {
                ES3.Save(uniqueID, position, "PickedItems.es3");
                Debug.Log($"Saved loot {uniqueID} at {position}");
            }
        }
    }
}
