using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.TDEExtensions.Stamina;
using UnityEngine;

namespace Stamina
{
    public class StaminaBarUpdater : MonoBehaviour, MMEventListener<StaminaUpdateEvent>,
        MMEventListener<TopDownEngineEvent>
    {
        [SerializeField] [Tooltip("if this is false, the player character will be set as target automatically")]
        bool UseCustomTarget;
        [MMCondition(nameof(UseCustomTarget), true)] [SerializeField]
        GameObject Target;

        MMProgressBar _bar;

        void Awake()
        {
            _bar = GetComponent<MMProgressBar>();
            this.MMEventStartListening<TopDownEngineEvent>();
        }

        void OnEnable()
        {
            this.MMEventStartListening<StaminaUpdateEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<StaminaUpdateEvent>();
        }

        public void OnMMEvent(StaminaUpdateEvent itemEvent)
        {
            if (itemEvent.Target != Target) return;
            _bar.UpdateBar(itemEvent.Stamina, 0, itemEvent.MaxStamina);
        }

        public void OnMMEvent(TopDownEngineEvent itemEvent)
        {
            if (itemEvent.EventType == TopDownEngineEventTypes.SpawnCharacterStarts && !UseCustomTarget)
                Target = LevelManager.Instance.Players[0].gameObject;
        }
    }
}
