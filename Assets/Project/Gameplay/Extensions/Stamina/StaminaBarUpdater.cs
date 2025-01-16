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

        public void OnMMEvent(StaminaUpdateEvent @event)
        {
            if (@event.Target != Target) return;
            _bar.UpdateBar(@event.Stamina, 0, @event.MaxStamina);
        }

        public void OnMMEvent(TopDownEngineEvent @event)
        {
            if (@event.EventType == TopDownEngineEventTypes.SpawnCharacterStarts && !UseCustomTarget)
                Target = LevelManager.Instance.Players[0].gameObject;
        }
    }
}
