using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPRecipeDetails : MonoBehaviour, MMEventListener<RecipeEvent>
{
    public Image recipeImage;
    public TMP_Text recipeName;
    public TMP_Text recipeDescription;


    [SerializeField] GameObject requiredMaterialsEntryPrefab;
    [SerializeField] GameObject toolsEntryPrefab;
    [SerializeField] GameObject requiredRawFoodItemsEntryPrefab;

    [SerializeField] GameObject requiredMaterialsParent;
    [SerializeField] GameObject toolsParent;
    [SerializeField] GameObject requiredRawFoodItemsParent;


    CookingRecipe _recipe;

    void Start()
    {
        HidePanel();
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(RecipeEvent recipeEvent)
    {
        if (recipeEvent.EventType == RecipeEventType.ShowRecipeDetails)
        {
            ShowPanel();
            _recipe = recipeEvent.RecipeParameter;
            SetRecipeDetails();
        }
    }

    public void ShowPanel()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    void SetRecipeDetails()
    {
        Debug.Log("Setting recipe details for: " + _recipe.recipeName);
        recipeName.text = _recipe.recipeName;
        recipeImage.sprite = _recipe.finishedFoodItem.FinishedFood.Icon;

        if (_recipe.recipeDescription != null)
            recipeDescription.text = _recipe.recipeDescription;
        else
            recipeDescription.text = "No description available.";

        // Clear existing UI elements to avoid duplicates
        foreach (Transform child in requiredMaterialsParent.transform) Destroy(child.gameObject);
        foreach (Transform child in toolsParent.transform) Destroy(child.gameObject);
        foreach (Transform child in requiredRawFoodItemsParent.transform) Destroy(child.gameObject);

        foreach (var requiredMaterial in _recipe.requiredMaterials)
        {
            var requiredMaterialsEntry = Instantiate(requiredMaterialsEntryPrefab, requiredMaterialsParent.transform);
            var requiredMaterialsEntryScript = requiredMaterialsEntry.GetComponent<MaterialEntry>();
            if (requiredMaterialsEntryScript != null)
                requiredMaterialsEntryScript.SetMaterial(requiredMaterial);
        }

        if (_recipe.RequiresTools)
        {
            var toolsEntry = Instantiate(toolsEntryPrefab, toolsParent.transform);
            var toolsEntryScript = toolsEntry.GetComponent<ToolEntry>();
            if (toolsEntryScript != null)
                toolsEntryScript.SetTool(_recipe.RequiredTools[0]);
        }

        foreach (var requiredRawFoodItem in _recipe.requiredRawFoodItems)
        {
            var requiredRawFoodItemsEntry = Instantiate(
                requiredRawFoodItemsEntryPrefab, requiredRawFoodItemsParent.transform);

            var requiredRawFoodItemsEntryScript = requiredRawFoodItemsEntry.GetComponent<MaterialEntry>();
            if (requiredRawFoodItemsEntryScript != null)
                requiredRawFoodItemsEntryScript.SetMaterial(requiredRawFoodItem);
        }
    }
}
