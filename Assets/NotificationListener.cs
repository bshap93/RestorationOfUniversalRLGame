using Core.Events;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class NotificationListener : MonoBehaviour, MMEventListener<RecipeEvent>, MMEventListener<RecipeGroupEvent>
{
    [FormerlySerializedAs("_craftedNewItemNotification")] [SerializeField]
    CraftedNewItemNotification craftedNewItemNotification;

    [SerializeField] LearnedNewRecipesNotification learnedNewRecipesNotification;
    [SerializeField] CanvasGroup learnedNewRecipesNotificationCanvasGroup;

    CanvasGroup _craftedNewItemNotificationCanvasGroup;


    void Start()
    {
        if (_craftedNewItemNotificationCanvasGroup == null)
            _craftedNewItemNotificationCanvasGroup = craftedNewItemNotification.gameObject.GetComponent<CanvasGroup>();

        Debug.Log("NotificationListener disabled craftedNewItemNotification");
        DisableCanvasGroup(_craftedNewItemNotificationCanvasGroup, craftedNewItemNotification);

        if (learnedNewRecipesNotificationCanvasGroup == null)
            learnedNewRecipesNotificationCanvasGroup =
                learnedNewRecipesNotification.gameObject.GetComponent<CanvasGroup>();

        Debug.Log("NotificationListener disabled learnedNewRecipesNotification");
        DisableCanvasGroup(learnedNewRecipesNotificationCanvasGroup, learnedNewRecipesNotification);
    }

    void OnEnable()
    {
        this.MMEventStartListening<RecipeGroupEvent>();
        this.MMEventStartListening<RecipeEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<RecipeGroupEvent>();
        this.MMEventStopListening<RecipeEvent>();
    }


    public void OnMMEvent(RecipeEvent recipeEvent)
    {
        if (recipeEvent.EventType == RecipeEventType.CraftingFinished)
            EnableCraftedNewItemCanvasGroup(_craftedNewItemNotificationCanvasGroup, recipeEvent);
    }

    public void OnMMEvent(RecipeGroupEvent recipeGroupEvent)
    {
        if (recipeGroupEvent.EventType == RecipeGroupEventType.RecipeGroupLearned)
            EnableLearnedNewRecipesCanvasGroup(learnedNewRecipesNotificationCanvasGroup, recipeGroupEvent);
    }
    public void DisableAllNotifications()
    {
        DisableCanvasGroup(_craftedNewItemNotificationCanvasGroup, craftedNewItemNotification);
        DisableCanvasGroup(learnedNewRecipesNotificationCanvasGroup, learnedNewRecipesNotification);
    }


    void DisableCanvasGroup(CanvasGroup canvasGroup, IURPNotification itemNotification1)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        itemNotification1.Hide();
    }

    void EnableCraftedNewItemCanvasGroup(CanvasGroup canvasGroup, RecipeEvent craftingEvent)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        craftedNewItemNotification.RestartWithNewItem(craftingEvent.RecipeParameter.Item);
    }

    void EnableLearnedNewRecipesCanvasGroup(CanvasGroup canvasGroup, RecipeGroupEvent recipeGroupEvent)
    {
        learnedNewRecipesNotification.Initialize(recipeGroupEvent.RecipeGroup);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
