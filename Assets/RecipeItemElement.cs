using System.Linq;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RecipeItemElement : MonoBehaviour
{
    [FormerlySerializedAs("ProductIcon")] [SerializeField]
    Image productIcon;
    [FormerlySerializedAs("ProductName")] [SerializeField]
    TMP_Text productName;
    [FormerlySerializedAs("ProductDescription")] [SerializeField]
    TMP_Text productDescription;
    [FormerlySerializedAs("_craftingStationType")] [SerializeField]
    TMP_Text craftingStationType;
    [FormerlySerializedAs("_craftingStationTypeIcon")] [SerializeField]
    Image craftingStationTypeIcon;
    [FormerlySerializedAs("_craftingIngredientPanel")] [SerializeField]
    CraftingIngredientPanel craftingIngredientPanel;
    Recipe _recipe;
    RecipeGroup _recipeGroup;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        craftingIngredientPanel = GetComponentInChildren<CraftingIngredientPanel>();
        Nullify();
    }

    public void Initialize(Recipe recipeLoc, RecipeGroup recipeGroupLoc)
    {
        _recipe = recipeLoc;
        _recipeGroup = recipeGroupLoc;
        if (craftingIngredientPanel != null) craftingIngredientPanel.Initialize(_recipe.Ingredients.ToList());
        if (productIcon != null)
        {
            productIcon.color = new Color(1, 1, 1, 1);
            productIcon.sprite = _recipe.Item.Icon;
        }

        if (productName != null) productName.text = _recipe.Item.ItemName;
        if (productDescription != null) productDescription.text = _recipe.Item.Description;
        if (craftingStationType != null) craftingStationType.text = _recipeGroup.RecipeType.ToString();

        if (craftingStationTypeIcon != null)
        {
            craftingStationTypeIcon.color = new Color(1, 1, 1, 1);
            switch (_recipeGroup.RecipeType)
            {
                case RecipeType.Cooking:
                    craftingStationTypeIcon.sprite = RecipeTypeIcons.CookingIcon;
                    break;
                case RecipeType.Crafting:
                    craftingStationTypeIcon.sprite = RecipeTypeIcons.CraftingIcon;
                    break;
                case RecipeType.Potion:
                    craftingStationTypeIcon.sprite = RecipeTypeIcons.PotionIcon;
                    break;
            }
        }
    }


    public void Nullify()
    {
        if (craftingIngredientPanel != null) craftingIngredientPanel.Nullify();
        if (productIcon != null)
        {
            productIcon.sprite = null;
            productIcon.color = new Color(0, 0, 0, 0);
        }

        if (productName != null) productName.text = "";
        if (productDescription != null) productDescription.text = "";
        if (craftingStationType != null) craftingStationType.text = "";
        if (craftingStationTypeIcon != null)
        {
            craftingStationTypeIcon.sprite = null;
            craftingStationTypeIcon.color = new Color(0, 0, 0, 0);
        }
    }
}
