﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.Interactivity.Food;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using Unity.VisualScripting;
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

    public class CookingQueueInventory : CraftingQueueInventory, MMEventListener<RecipeEvent>
    {
        public MMFeedbacks addedRawFoodFeedback;
        public MMFeedbacks cookingStartsFeedback;
        public MMFeedbacks cookingEndsFeedback;

        public FuelInventory fuelInventory;
        public CookingDepositInventory cookingDepositInventory;
        // public MMProgressBar cookingProgressBar;

        public RecipeHeader recipeHeader;


        [FormerlySerializedAs("_cookableRecipes")] [SerializeField]
        List<CookingRecipe> cookableRecipes = new();

        public MMFeedbacks CannotCookFeedback;

        CookingStationController _cookingStationController;

        CookingRecipe _currentRecipe;

        JournalPersistenceManager _journalPersistenceManager;

        public void Start()
        {
            if (recipeHeader == null)

                recipeHeader = FindObjectOfType<RecipeHeader>();

            if (_cookingStationController == null)
                _cookingStationController = gameObject.GetComponentInParent<CookingStationController>();

            if (_journalPersistenceManager == null)
                _journalPersistenceManager = FindObjectOfType<JournalPersistenceManager>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<RecipeEvent>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<RecipeEvent>();
        }
        public void OnMMEvent(RecipeEvent @event)
        {
            if (@event.EventType == RecipeEventType.ChooseRecipeFromCookable)
                ChooseRecipeFromCookableRecipes(@event.RecipeParameter);
        }

        public bool CookableRecipesContains(CookingRecipe recipe)
        {
            foreach (var cookableRecipe in cookableRecipes)
                if (cookableRecipe.recipeID == recipe.recipeID)
                    return true;

            return false;
        }


        public override bool AddItem(InventoryItem item, int quantity)
        {
            return AddItemAt(item, quantity, -1);
        }

        public override bool AddItemAt(InventoryItem item, int quantity, int index)
        {
            var result = false;
            if (item is RawFood rawFood)
            {
                addedRawFoodFeedback?.PlayFeedbacks();


                result = base.AddItemAt(item, quantity, index);

                TryDetectRecipeFromIngredientsInQueue(rawFood);


                // if (_currentRecipe != null)
                // {
                //     Debug.Log("CurrentRecipe: " + _currentRecipe);
                //     Debug.Log("Recipename: " + _currentRecipe.recipeName);
                //     recipeHeader.recipeName.text = _currentRecipe.recipeName;
                //     recipeHeader.recipeImage.sprite =
                //         _currentRecipe.finishedFoodItem.FinishedFood.Icon;
                // }


                return result;
            }

            return false;
        }

        public override bool RemoveItem(int index, int quantity)
        {
            var result = base.RemoveItem(index, quantity);


            TryDetectRecipeFromIngredientsInQueue(Content[index] as RawFood);


            return result;
        }

        public void ChooseRecipeFromCookableRecipes(CookingRecipe recipe)
        {
            if (cookableRecipes.Contains(recipe))
            {
                _currentRecipe = recipe;
                _cookingStationController.SetCurrentRecipe(_currentRecipe);
            }
            else
            {
                Debug.LogWarning("The selected recipe is not in the list of cookable recipes.");
            }
        }


        public void StartCookingCurrentRecipe()
        {
            if (!CookableRecipesContains(_currentRecipe))
            {
                Debug.Log("Tried to cook a recipe that is not cookable with the current ingredients.");
                CannotCookFeedback?.PlayFeedbacks();
                return;
            }

            if (fuelInventory.IsBurning && _currentRecipe != null)
            {
                var cookingRecipeInProgress = new CookingRecipeInProgress(_currentRecipe);
                Debug.Log("CookingQueueInventory.StartCookingCurrentRecipe: Cooking " + _currentRecipe.recipeName);
                StartCoroutine(CookFood(cookingRecipeInProgress, 1));
            }
        }


        void TryDetectRecipeFromIngredientsInQueue(RawFood rawFood)
        {
            cookableRecipes.Clear();

            // Should only update the relevant cooking station's dropdown
            RecipeEvent.Trigger(
                "ClearCookableRecipes", RecipeEventType.ClearCookableRecipes, null,
                _cookingStationController.CookingStation.CraftingStationId);

            foreach (var recipe in _journalPersistenceManager.JournalData.knownRecipes)
                if (recipe.CanBeCookedFrom(Content))
                {
                    if (CookableRecipesContains(recipe))
                        continue;

                    cookableRecipes.Add(recipe);
                    RecipeEvent.Trigger(
                        "RecipeCookableWithCurrentIngredients", RecipeEventType.RecipeCookableWithCurrentIngredients,
                        recipe, _cookingStationController.CookingStation.CraftingStationId);


                    Debug.Log("Added recipe to cookableRecipes: " + recipe.recipeName);
                }

            if (cookableRecipes.Count == 1)
            {
                TrySetCurrentRecipe(cookableRecipes[0]);
            }
            else if (cookableRecipes.Count > 1)
            {
                Debug.LogWarning("More than one recipe can be cooked from the ingredients in the queue.");
                TrySetCurrentRecipe(cookableRecipes.LastOrDefault());
            }
            else
            {
                if (rawFood != null) TrySetCurrentRecipe(rawFood.CookedSingleRawFoodRecipe);
            }
        }
        void TrySetCurrentRecipe(CookingRecipe recipe)
        {
            if (cookableRecipes.Contains(recipe))
            {
                _currentRecipe = recipe;
                _cookingStationController.SetCurrentRecipe(recipe);
            }
            else
            {
                Debug.LogWarning("The selected recipe is not in the list of cookable recipes.");
            }
        }


        public override bool MoveItem(int oldIndex, int newIndex)
        {
            Debug.Log("CookingQueueInventory.MoveItem");
            return false;
        }

        IEnumerator CookFood(CookingRecipeInProgress cookingRecipeInProgress, int quantity)
        {
            if (!CookableRecipesContains(cookingRecipeInProgress.currentRecipe))
            {
                Debug.LogError(
                    "Tried to cook a recipe: " + cookingRecipeInProgress.currentRecipe.recipeName +
                    " that is not in cookableRecipes: " + cookableRecipes.ToLineSeparatedString());

                yield break;
            }

            if (!fuelInventory.IsBurning) yield break;
            float elapsedTime = 0;
            cookingStartsFeedback?.PlayFeedbacks();

            Debug.Log("CookingQueueInventory.CookFood: Cooking " + cookingRecipeInProgress.currentRecipe.recipeName);

            Debug.Log("Crafting time: " + _currentRecipe.CraftingTime);

            while (elapsedTime < _currentRecipe.CraftingTime)
            {
                MMGameEvent.Trigger(
                    "UpdateCookingProgressBar",
                    stringParameter: _cookingStationController.CookingStation.CraftingStationId,
                    vector2Parameter: new Vector2(elapsedTime / _currentRecipe.CraftingTime, 0));

                // cookingProgressBar.UpdateBar(
                // elapsedTime / _currentRecipe.CraftingTime,
                // 0, 1);


                yield return null;

                elapsedTime += 0.1f;
            }

            foreach (var rawFoodItem in cookingRecipeInProgress.currentRecipe.requiredRawFoodItems)
                RemoveItemByID(rawFoodItem.item.ItemID, quantity);


            cookingDepositInventory.AddItem(
                cookingRecipeInProgress.currentRecipe.finishedFoodItem.FinishedFood, quantity);

            _currentRecipe = null;
            RecipeEvent.Trigger(
                "FinishedCookingRecipe", RecipeEventType.FinishedCookingRecipe, null,
                _cookingStationController.CookingStation.CraftingStationId);

            cookingEndsFeedback?.PlayFeedbacks();
        }
    }
}
