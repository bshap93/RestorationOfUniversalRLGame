using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.Interactivity.Food;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using Project.Gameplay.SaveLoad;
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
        public MMProgressBar cookingProgressBar;

        public RecipeHeader recipeHeader;

        [SerializeField] List<CookingRecipe> _cookableRecipes = new();
        CookingStationController _cookingStationController;

        CookingRecipe _currentRecipe;

        JournalPersistenceManager _journalPersistenceManager;

        readonly List<string> CookableRecipeIDs = new();


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
        public void OnMMEvent(RecipeEvent cookingStationEvent)
        {
            if (cookingStationEvent.EventType == RecipeEventType.ChooseRecipeFromCookable)
                ChooseRecipeFromCookableRecipes(_cookableRecipes.IndexOf(cookingStationEvent.RecipeParameter));
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


                if (_currentRecipe != null)
                {
                    recipeHeader.recipeName.text = _currentRecipe.recipeName;
                    recipeHeader.recipeImage.sprite =
                        _currentRecipe.finishedFoodItem.FinishedFood.Icon;
                }


                return result;
            }

            return false;
        }

        public void ChooseRecipeFromCookableRecipes(int index)
        {
            if (index < _cookableRecipes.Count)
            {
                _currentRecipe = _cookableRecipes[index];
                _cookingStationController.SetCurrentRecipe(_currentRecipe);
            }
        }

        public void StartCookingCurrentRecipe()
        {
            if (fuelInventory.IsBurning && _currentRecipe != null)
            {
                var cookingRecipeInProgress = new CookingRecipeInProgress(_currentRecipe);
                StartCoroutine(CookFood(cookingRecipeInProgress, 1));
            }
        }


        void TryDetectRecipeFromIngredientsInQueue(RawFood rawFood)
        {
            foreach (var recipe in _journalPersistenceManager.journalData.knownRecipes)
                if (recipe.CanBeCookedFrom(Content))
                {
                    if (CookableRecipeIDs.Contains(recipe.recipeID))
                        continue;

                    _cookableRecipes.Add(recipe);
                    RecipeEvent.Trigger(
                        "RecipeCookableWithCurrentIngredients", RecipeEventType.RecipeCookableWithCurrentIngredients,
                        recipe);

                    CookableRecipeIDs.Add(recipe.recipeID);

                    Debug.Log("Added recipe to cookableRecipes: " + recipe.recipeName);
                }
                else
                {
                    Debug.Log("Recipe: " + recipe.recipeName + " cannot be cooked from the ingredients in the queue.");
                }

            if (_cookableRecipes.Count == 1)
            {
                _currentRecipe = _cookableRecipes[0];
                _cookingStationController.SetCurrentRecipe(_currentRecipe);
            }
            else if (_cookableRecipes.Count > 1)
            {
                Debug.LogWarning("More than one recipe can be cooked from the ingredients in the queue.");
                _currentRecipe = _cookableRecipes.LastOrDefault();
            }
            else
            {
                _currentRecipe = rawFood.CookedSingleRawFoodRecipe;
                _cookingStationController.SetCurrentRecipe(_currentRecipe);
            }
        }


        public override bool MoveItem(int oldIndex, int newIndex)
        {
            return false;
        }

        IEnumerator CookFood(CookingRecipeInProgress cookingRecipeInProgress, int quantity)
        {
            if (!fuelInventory.IsBurning) yield break;
            float elapsedTime = 0;
            cookingStartsFeedback?.PlayFeedbacks();

            Debug.Log("CookingQueueInventory.CookFood: Cooking " + cookingRecipeInProgress.currentRecipe.recipeName);

            Debug.Log("Crafting time: " + _currentRecipe.CraftingTime);

            while (elapsedTime < _currentRecipe.CraftingTime)
            {
                cookingProgressBar.UpdateBar(
                    elapsedTime / _currentRecipe.CraftingTime,
                    0, 1);


                yield return null;

                elapsedTime += 0.1f;
            }

            foreach (var rawFoodItem in cookingRecipeInProgress.currentRecipe.requiredRawFoodItems)
                RemoveItemByID(rawFoodItem.item.ItemID, quantity);


            cookingDepositInventory.AddItem(
                cookingRecipeInProgress.currentRecipe.finishedFoodItem.FinishedFood, quantity);

            cookingEndsFeedback?.PlayFeedbacks();
        }
    }
}
