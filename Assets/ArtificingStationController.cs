using Core.Events;
using Gameplay.Crafting;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;

public class ArtificingStationController : MonoBehaviour, ISelectableTrigger,
    ICraftingStation, MMEventListener<RecipeEvent>, MMEventListener<RecipeGroupEvent>
{
    public CraftingButtons craftingButtons;
    public CanvasGroup craftingChoiceUIPanel;
    public CanvasGroup craftingProgressUIDisplayler;
    public MMFeedbacks artificeStoneworkFeedback;   
    
    public KeyCode interactionKey = KeyCode.F;
    public RecipeGroup stationSetRecipes;
    public bool stationHasSetRecipes;
    
    private bool _isArtificeStationReady;
    private bool _isInPlayerRange;
    
    public void ShowStationChoicePanel()
    {
        throw new System.NotImplementedException();
    }
    public void HideStationChoicePanel()
    {
        throw new System.NotImplementedException();
    }
    void ICraftingStation.OnSelectedItem()
    {
        throw new System.NotImplementedException();
    }
    void ICraftingStation.OnUnSelectedItem()
    {
        throw new System.NotImplementedException();
    }
    public void OnValidate()
    {
        throw new System.NotImplementedException();
    }
    void ISelectableTrigger.OnSelectedItem()
    {
        throw new System.NotImplementedException();
    }
    void ISelectableTrigger.OnUnSelectedItem()
    {
        throw new System.NotImplementedException();
    }
    public void OnMMEvent(RecipeEvent eventType)
    {
        throw new System.NotImplementedException();
    }
    public void OnMMEvent(RecipeGroupEvent eventType)
    {
        throw new System.NotImplementedException();
    }
}
