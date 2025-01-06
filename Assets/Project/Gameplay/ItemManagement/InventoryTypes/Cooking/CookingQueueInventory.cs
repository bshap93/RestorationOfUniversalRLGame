﻿using System;
using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Food;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    [Serializable]
    public class CookingRecipeInProgress
    {
        [FormerlySerializedAs("recipe")] public CookingRecipe currentRecipe;
        [FormerlySerializedAs("percentageComplete")]
        public float percentageCompleteFraction;
        public float craftingTime;

        public CookingRecipeInProgress(CookingRecipe currentRecipe)
        {
            this.currentRecipe = currentRecipe;
            percentageCompleteFraction = 0;
            craftingTime = currentRecipe.CraftingTime;
        }
    }

    public class CookingQueueInventory : CraftingQueueInventory
    {
        public MMFeedbacks addedRawFoodFeedback;
        public MMFeedbacks cookingStartsFeedback;
        public MMFeedbacks cookingEndsFeedback;

        public CookingRecipe currentRecipe;

        public FuelInventory fuelInventory;
        public CookingDepositInventory cookingDepositInventory;
        public MMProgressBar cookingProgressBar;

        public RecipeHeader recipeHeader;

        public void Start()
        {
            if (recipeHeader == null)

                recipeHeader = FindObjectOfType<RecipeHeader>();
        }


        public override bool AddItem(InventoryItem item, int quantity)
        {
            if (item is RawFood rawFood)
            {
                addedRawFoodFeedback?.PlayFeedbacks();

                if (currentRecipe == null)
                    // CheckIfRecipesExistForItemsNowInQueue();
                    currentRecipe = rawFood.CookedSingleRawFoodRecipe;

                recipeHeader.recipeName.text = currentRecipe.recipeName;
                recipeHeader.recipeImage.sprite =
                    currentRecipe.finishedFoodItem.FinishedFood.Icon;


                if (fuelInventory.IsBurning)
                {
                    var cookingRecipeInProgress = new CookingRecipeInProgress(currentRecipe);
                    StartCoroutine(CookFood(cookingRecipeInProgress, quantity));
                }

                return base.AddItem(item, quantity);
            }


            return false;
        }

        public override bool AddItemAt(InventoryItem item, int quantity, int index)
        {
            if (item is RawFood rawFood)
            {
                addedRawFoodFeedback?.PlayFeedbacks();

                if (currentRecipe == null)
                    // CheckIfRecipesExistForItemsNowInQueue();
                    currentRecipe = rawFood.CookedSingleRawFoodRecipe;

                recipeHeader.recipeName.text = currentRecipe.recipeName;
                recipeHeader.recipeImage.sprite =
                    currentRecipe.finishedFoodItem.FinishedFood.Icon;

                if (fuelInventory.IsBurning)
                {
                    var cookingRecipeInProgress = new CookingRecipeInProgress(currentRecipe);
                    StartCoroutine(CookFood(cookingRecipeInProgress, quantity));
                }

                return base.AddItemAt(item, quantity, index);
            }

            return false;
        }
        public override bool MoveItem(int oldIndex, int newIndex)
        {
            return true;
        }

        IEnumerator CookFood(CookingRecipeInProgress cookingRecipeInProgress, int quantity)
        {
            if (!fuelInventory.IsBurning) yield break;
            float elapsedTime = 0;
            cookingStartsFeedback?.PlayFeedbacks();

            while (elapsedTime < cookingRecipeInProgress.craftingTime)
            {
                cookingRecipeInProgress.percentageCompleteFraction = elapsedTime / cookingRecipeInProgress.craftingTime;
                cookingProgressBar.UpdateBar(
                    cookingRecipeInProgress.percentageCompleteFraction,
                    0, 1);

                yield return null;

                elapsedTime += Time.deltaTime;
            }

            RemoveItem(0, quantity);
            cookingDepositInventory.AddItem(
                cookingRecipeInProgress.currentRecipe.finishedFoodItem.FinishedFood, quantity);

            cookingEndsFeedback?.PlayFeedbacks();
        }
    }
}
