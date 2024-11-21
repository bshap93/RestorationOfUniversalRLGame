using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Magic;
using UnityEngine;

public class MagicSystemInitializer : MonoBehaviour, MMEventListener<TopDownEngineEvent>
{
    public MagicResourceUI ResourceUI; // Reference to the UI component

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(TopDownEngineEvent engineEvent)
    {
        if (engineEvent.EventType == TopDownEngineEventTypes.SpawnComplete)
        {
            var player = engineEvent.OriginCharacter?.gameObject;
            if (player != null) LinkPlayerToUI(player);
        }
    }

    void LinkPlayerToUI(GameObject player)
    {
        var magicSystem = player.GetComponent<MagicSystem>();
        if (magicSystem != null && ResourceUI != null) ResourceUI.SetMagicSystem(magicSystem);
    }
}
