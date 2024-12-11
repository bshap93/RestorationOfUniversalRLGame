using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class RevertTrigger : MonoBehaviour, MMEventListener<TopDownEngineEvent>
    {
        // Start is called before the first frame update
        public static void Revert()
        {
            MMGameEvent.Trigger("Revert");
        }
        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "PlayerDies")
            {
                Revert();
                Debug.Log("Player died, reverting level");
                var levelSelector = gameObject.GetComponent<LevelSelector>();
                levelSelector.ReloadLevel();
            }
            
        }
        public void OnMMEvent(TopDownEngineEvent eventType)
        {
            if (eventType.EventType == TopDownEngineEventTypes.GameOver || eventType.EventType == TopDownEngineEventTypes.PlayerDeath)
            {
                Debug.Log("Player died, reverting level");
                Revert();
                var levelSelector = gameObject.GetComponent<LevelSelector>();
                levelSelector.ReloadLevel();
            }
            
        }
    }
}
