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

        [Header("Cooking Feedbacks")] public bool isCookingStation;
        public MMFeedbacks startCookingFeedback;
        public MMFeedbacks finishCookingFeedback;

        GameObject _craftingButton;
        Inventory _inventory;
        RecipeGroup _recipeGroupRecipes;

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

            _craftingButton.SetActive(false);

            RefreshButtons();
        }

        public void SetRecipeGroup(RecipeGroup recipeGroup)
        {
            _recipeGroupRecipes = recipeGroup;
            Debug.Log("Set recipe group: " + _recipeGroupRecipes.UniqueID);
        }

        public void RefreshButtons()
        {
            foreach (var recipe in _recipeGroupRecipes.Recipes)
                if (!_inventory.ContainsIngredientsForRecipe(recipe))
                {
                    var craftingButton = transform.Find(recipe.Name).gameObject;
                    craftingButton.GetComponent<Button>().interactable = false;
                    craftingButton.transform.GetChild(4).gameObject.SetActive(true);
                }
        }

        public void CreateButtons()
        {
            // Deactivate the template button
            _craftingButton.SetActive(false);

            // Clear old buttons except the template (child at index 0)
            for (var i = transform.childCount - 1; i > 0; i--) Destroy(transform.GetChild(i).gameObject);

            if (_recipeGroupRecipes?.Recipes == null) return;

            _inventory = FindFirstObjectByType<MainInventory>();
            if (_inventory == null)
            {
                Debug.LogError("Could not find inventory");
                return;
            }


            foreach (var recipe in _recipeGroupRecipes.Recipes)
            {
                var craftingButton = Instantiate(_craftingButton, transform);
                craftingButton.SetActive(true);

                craftingButton.name = recipe.Name;

                // Setup button UI
                craftingButton.transform.GetChild(0).GetComponent<Text>().text = recipe.Name;
                craftingButton.transform.GetChild(1).GetComponent<Text>().text = recipe.Item.ShortDescription;
                craftingButton.transform.GetChild(2).GetComponent<Image>().sprite = recipe.Item.Icon;
                craftingButton.transform.GetChild(3).GetComponent<Text>().text = recipe.IngredientsText;

                var greyOverlay = craftingButton.transform.GetChild(4).gameObject;


                if (!_inventory.ContainsIngredientsForRecipe(recipe))
                {
                    craftingButton.GetComponent<Button>().interactable = false;
                    greyOverlay.SetActive(true);
                }
                else
                {
                    craftingButton.GetComponent<Button>().interactable = true;
                    greyOverlay.SetActive(false);
                }

                var localRecipe = recipe;
                // Ensure only one onClick listener
                craftingButton.GetComponent<Button>().onClick.RemoveAllListeners();
                craftingButton.GetComponent<Button>().onClick.AddListener(() => HandleCrafting(localRecipe));
            }
        }
        public void SetCraftRecipes(RecipeGroup stationRecipes)
        {
            _recipeGroupRecipes = stationRecipes;
        }
    }
}
