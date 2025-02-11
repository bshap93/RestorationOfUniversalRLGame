using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Quests.Scripts
{
    public class DialogueToInventoryAction : MonoBehaviour, MMEventListener<MMInventoryEvent>,
        MMEventListener<MMGameEvent>
    {
        const string _saveFolderName = "DialogueSystem/";
        const string _saveFileExtension = ".data";
        [Tooltip("Update the quest tracker when the inventory's content changes.")]
        public bool updateQuestTracker = true;

        [Tooltip("Save & load Dialogue System data when More Mountains SaveLoadManager requests.")]
        public bool handleMMSaveLoadEvents;

        public UnityEvent onContentChanged = new();

        public List<InventoryDialogueMapping> itemMappings = new();

        Inventory _inventory;

        protected virtual void Start()
        {
            _inventory = GetComponent<Inventory>();
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
            if (handleMMSaveLoadEvents) this.MMEventStartListening<MMGameEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
            if (handleMMSaveLoadEvents) this.MMEventStopListening<MMGameEvent>();
        }

        public virtual void OnMMEvent(MMGameEvent gameEvent)
        {
            if (gameEvent.EventName == "Save") SaveDialogueSystem();
            if (gameEvent.EventName == "Load") LoadDialogueSystem();
        }

        public virtual void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.TargetInventoryName != name) return;

            switch (eventType.InventoryEventType)
            {
                case MMInventoryEventType.ContentChanged:
                    UpdateDialogueSystemVariables();
                    if (updateQuestTracker) DialogueManager.SendUpdateTracker();
                    onContentChanged.Invoke();
                    break;
            }
        }

        void UpdateDialogueSystemVariables()
        {
            foreach (var mapping in itemMappings)
            {
                var hasItem = _inventory.Content.Any(item => item.name == mapping.itemName);
                var value = hasItem ? mapping.valueToSet : 0; // Set variable if item exists, otherwise reset to 0

                DialogueLua.SetVariable(mapping.dialogueVariable, value);
                Debug.Log($"Set Dialogue System variable '{mapping.dialogueVariable}' to {value}");
            }
        }


        public void SaveDialogueSystem()
        {
            var data = SaveSystem.hasInstance
                ? SaveSystem.Serialize(SaveSystem.RecordSavedGameData())
                : PersistentDataManager.GetSaveData();

            MMSaveLoadManager.Save(data, gameObject.name + _saveFileExtension, _saveFolderName);
        }

        public void LoadDialogueSystem()
        {
            var data = (string)MMSaveLoadManager.Load(
                typeof(string), gameObject.name + _saveFileExtension, _saveFolderName);

            if (SaveSystem.hasInstance)
                SaveSystem.ApplySavedGameData(SaveSystem.Deserialize<SavedGameData>(data));
            else
                PersistentDataManager.ApplySaveData(data);
        }

        // Map Inventory Items to Dialogue System Variables
        [Serializable]
        public class InventoryDialogueMapping
        {
            public string itemName; // Inventory Item Name
            public string dialogueVariable; // Dialogue System Variable to update
            public int valueToSet; // Value to assign when the item is added
        }
    }
}
