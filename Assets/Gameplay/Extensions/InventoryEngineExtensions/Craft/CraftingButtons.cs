using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Extensions.InventoryEngineExtensions.Craft
{
    public class CraftingButtons : MonoBehaviour
    {
        [SerializeField] string inventoryName = "MainPlayerInventory";
        [SerializeField] string playerID = "Player1";
        public Craft craftRecipes;

        [Header("Cooking Feedbacks")] public bool isCookingStation;
        public MMFeedbacks startCookingFeedback;
        public MMFeedbacks finishCookingFeedback;

        GameObject _craftingButton;
        Inventory _inventory;

        void Awake()
        {
            _craftingButton = transform.GetChild(0).gameObject;
            _craftingButton.SetActive(false);
        }

        void OnEnable()
        {
            CreateButtons();
        }

        void HandleCrafting(Recipe recipe)
        {
            if (_inventory == null) _inventory = Inventory.FindInventory(inventoryName, playerID);

            if (_inventory != null)
            {
                if (isCookingStation)
                    _inventory.CookRecipe(recipe, startCookingFeedback);
                else
                    _inventory.Craft(recipe);
            }
            else
            {
                Debug.LogError("Cannot craft: inventory not found");
            }
        }

        void CreateButtons()
        {
            // Deactivate template first
            _craftingButton.SetActive(false);

            // Clear old buttons except template
            for (var i = transform.childCount - 1; i > 0; i--) Destroy(transform.GetChild(i).gameObject);

            if (craftRecipes?.Recipes == null) return;

            _inventory = FindFirstObjectByType<MainInventory>();
            if (_inventory == null)
            {
                Debug.LogError("Could not find inventory");
                return;
            }

            foreach (var recipe in craftRecipes.Recipes)
            {
                var craftingButton = Instantiate(_craftingButton, transform);
                craftingButton.SetActive(true);

                // Setup button UI
                craftingButton.transform.GetChild(0).GetComponent<Text>().text = recipe.Name;
                craftingButton.transform.GetChild(1).GetComponent<Text>().text = recipe.Item.ShortDescription;
                craftingButton.transform.GetChild(2).GetComponent<Image>().sprite = recipe.Item.Icon;
                craftingButton.transform.GetChild(3).GetComponent<Text>().text = recipe.IngredientsText;

                var localRecipe = recipe;
                // Only one onClick listener
                craftingButton.GetComponent<Button>().onClick.AddListener(() => HandleCrafting(localRecipe));
            }
        }
    }
}
