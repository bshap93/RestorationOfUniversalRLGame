using System.Collections;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Shields;
using UnityEngine;

namespace Project.Gameplay.Combat
{
    public class SpawnedCharacterEquipment : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        [Header("Inventory Names")] public string MainInventoryName = "MainInventory";
        public string EquipmentInventoryName = "EquipmentInventory";

        [Header("Starting Equipment")] [Tooltip("Shield to equip when character spawns")]
        public InventoryShieldItem StartingShield;

        protected Character _character;
        protected Inventory _equipmentInventory;
        protected bool _initialized;
        protected Inventory _mainInventory;

        protected virtual void Awake()
        {
            _character = GetComponent<Character>();
            FindInventories();
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening();
        }

        public virtual void OnMMEvent(MMGameEvent @event)
        {
            if (@event.EventName == "PlayerSpawn") StartCoroutine(InitializeEquipment());
        }

        protected virtual void FindInventories()
        {
            _mainInventory = Inventory.FindInventory(MainInventoryName, _character.PlayerID);
            _equipmentInventory = Inventory.FindInventory(EquipmentInventoryName, _character.PlayerID);

            if (_mainInventory == null || _equipmentInventory == null)
                Debug.LogWarning($"Could not find required inventories for {gameObject.name}");
        }

        protected virtual IEnumerator InitializeEquipment()
        {
            if (_initialized) yield break;

            // Wait a frame to ensure everything is ready
            yield return null;

            if (StartingShield != null)
            {
                Debug.Log($"Adding starting shield to inventory for {_character.name}");

                // Add to main inventory first
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.Pick, null,
                    MainInventoryName, StartingShield, 1, 0, _character.PlayerID);

                yield return null; // Wait for inventory to process

                // Then trigger equip
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.EquipRequest, null,
                    MainInventoryName, StartingShield, 1, 0, _character.PlayerID);
            }

            _initialized = true;
        }
    }
}
