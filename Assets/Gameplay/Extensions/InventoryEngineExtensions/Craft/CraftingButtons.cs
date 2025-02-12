using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Extensions.InventoryEngineExtensions.Craft
{
    public class CraftingButtons : MonoBehaviour
    {
        [Tooltip(
            "The inventory where the ingredients for crafting are found and where the resulting item will be added")]
        [SerializeField]
        public Inventory CraftingInventory;
        public Craft craftRecipes; // Changed from just 'Craft'
        GameObject _craftingButton;

        void Awake()
        {
            _craftingButton = transform.GetChild(0).gameObject;
        }

        void Start()
        {
            foreach (var recipe in craftRecipes.Recipes)
            {
                var craftingButton = Instantiate(_craftingButton, transform);
                craftingButton.transform.GetChild(0).GetComponent<Text>().text = recipe.Name;
                craftingButton.transform.GetChild(1).GetComponent<Text>().text = recipe.Item.ShortDescription;
                craftingButton.transform.GetChild(2).GetComponent<Image>().sprite = recipe.Item.Icon;
                craftingButton.transform.GetChild(3).GetComponent<Text>().text = recipe.IngredientsText;
                craftingButton.GetComponent<Button>().onClick.AddListener(() => CraftingInventory.Craft(recipe));
            }

            Destroy(_craftingButton);
        }
    }
}
