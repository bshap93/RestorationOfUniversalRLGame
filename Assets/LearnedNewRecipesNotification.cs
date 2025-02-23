using DG.Tweening;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using UnityEngine;

public class LearnedNewRecipesNotification : MonoBehaviour, IURPNotification
{
    [SerializeField] DOTweenAnimation headerDotweenAnimation;
    [SerializeField] RecipeGroup recipeGroup;

    [SerializeField] RecipeItemElement[] recipeItemElements;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (recipeGroup != null)
        {
            Initialize(recipeGroup);
            Debug.Log("Initialized recipe group");
        }
        else
        {
            Nullify();
            Debug.Log("Nullified recipe group");
        }
    }

    public void Hide()
    {
        headerDotweenAnimation.DOPause();
    }


    public void Initialize(RecipeGroup recipeGroup1)
    {
        recipeGroup = recipeGroup1;
        if (recipeItemElements.Length > 4)
        {
            Debug.LogError("More than 4 recipes is not allowed");
            return;
        }

        for (var i = 0; i < recipeItemElements.Length; i++)
            if (i < recipeGroup.Recipes.Length)
                recipeItemElements[i].Initialize(recipeGroup.Recipes[i], recipeGroup);
            else
                recipeItemElements[i].Nullify();

        headerDotweenAnimation.DORestart();
        headerDotweenAnimation.DOPlay();
    }

    public void Nullify()
    {
        for (var i = 0; i < recipeItemElements.Length; i++) recipeItemElements[i].Nullify();
    }
}
