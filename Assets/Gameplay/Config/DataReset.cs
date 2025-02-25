using Gameplay.Character.Attributes;
using Gameplay.ItemsInteractions;
using Gameplay.Player.Stats;
using Gameplay.SaveLoad;
using PixelCrushers;
using UnityEngine;

namespace Gameplay.Config
{
    public class DataReset : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log("PurePrototypeReset: Awake() called.");
            ClearAllSaveData();
        }
        public static void ClearAllSaveData()
        {
            // Reset Pickables
            PickableManager.ResetPickedItems();

            // Reset Journal Recipes
            CraftingRecipeManager.ResetLearnedCraftingGroups();

            // Reset Dispenser States
            DispenserManager.ResetDispenserStates();

            // Reset Inventory System
            InventoryPersistenceManager.Instance?.ResetInventory();

            // Reset  Mutable Stats
            PlayerStaminaManager.ResetPlayerStamina();
            PlayerHealthManager.ResetPlayerHealth();

            PlayerAttributesProgressionManager.ResetPlayerAttributesProgression();

            DestructibleManager.ResetDestroyedObjects();

            Debug.Log("Destuctable containers reset.");


            // Delete Dialogue System PlayerPrefs
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted Dialogue System PlayerPrefs.");

            SaveSystem.ClearSavedGameData();

            Debug.Log("All save data cleared successfully.");
        }
    }
}
