using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.TDEExtensions.Stamina
{
    public struct StaminaUpdateEvent
    {
        public GameObject Target;
        public float Stamina;
        public float MaxStamina;

        static StaminaUpdateEvent e;
        public static void Trigger(GameObject target, float stamina, float maxStamina)
        {
            e.Target = target;
            e.Stamina = stamina;
            e.MaxStamina = maxStamina;
            MMEventManager.TriggerEvent(e);
        }
    }

    [AddComponentMenu("TopDown Engine Extensions/Stamina")]
    public class Stamina : MonoBehaviour, MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {
        const string _runStopMethodName = nameof(CharacterRun.RunStop);
        const string _dashStopMethodName = "DashStop";
        [Header("Status")] [SerializeField] [MMReadOnly] [Tooltip("the current stamina of the character")]
        float _currentStamina;

        [Header("Stamina")]
        [MMInformation(
            "Add this component to an object and it'll use stamina when running or dashing and won't be able to dash/run if it doesn't have enough of it.",
            MMInformationAttribute.InformationType.Info, false)]
        [Tooltip("the initial amount of stamina of the object")]
        public float InitialStamina = 100;
        [Tooltip("the maximum amount of stamina of the object")]
        public float MaximumStamina = 100;

        [Header("Usage")] [Tooltip("how much stamina running will consume (per second)")]
        public float RunningStaminaConsumption = 5;
        [Tooltip("how much stamina dashing will consume (per dash)")]
        public float DashingStaminaConsumption = 20;

        [Header("Recovery")] [Tooltip("the number of seconds it takes to start recovering stamina after using it")]
        public float StaminaRecoveryDelay = 5;
        [Tooltip("how much stamina will be recovered each second")]
        public float StaminaRecoveryRate = 20;

        [Header("Feedbacks")] public MMFeedbacks OutOfStaminaFeedbacks;
        bool _recovering;
        Coroutine _recovery;
        bool _running;

        public float CurrentStamina
        {
            get => _currentStamina;
            set
            {
                var oldValue = _currentStamina;
                if (value > MaximumStamina)
                    _currentStamina = MaximumStamina;
                else if (value < 0)
                    _currentStamina = 0;
                else
                    _currentStamina = value;

                StaminaUpdateEvent.Trigger(gameObject, _currentStamina, MaximumStamina);
                if (value >= oldValue) return;
                if (_currentStamina < 0.001f) OutOfStaminaFeedbacks?.PlayFeedbacks();
                if (_recovering)
                    StopCoroutine(_recovery);

                _recovery = StartCoroutine(RecoverStamina());

                IEnumerator RecoverStamina()
                {
                    _recovering = true;
                    yield return MMCoroutine.WaitFor(StaminaRecoveryDelay);
                    while (CurrentStamina < MaximumStamina)
                    {
                        CurrentStamina += StaminaRecoveryRate * Time.deltaTime;
                        yield return MMCoroutine.WaitForFrames(1);
                    }

                    _recovering = false;
                }
            }
        }

        void Start()
        {
            CurrentStamina = InitialStamina;
            OutOfStaminaFeedbacks?.Initialization();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> @event)
        {
            if (@event.Target != gameObject)
                return;

            if (@event.NewState != CharacterStates.MovementStates.Running) _running = false;
            switch (@event.NewState)
            {
                case CharacterStates.MovementStates.Running:
                    StartCoroutine(ConsumeRunningStamina());
                    break;
                case CharacterStates.MovementStates.Dashing:
                    if (CurrentStamina >= DashingStaminaConsumption)
                        CurrentStamina -= DashingStaminaConsumption;
                    else
                        StartCoroutine(StopDashing());

                    break;
            }

            IEnumerator StopDashing()
            {
                yield return MMCoroutine.WaitForFrames(1);
                BroadcastMessage(_dashStopMethodName);
            }

            IEnumerator ConsumeRunningStamina()
            {
                _running = true;
                while (_running)
                    if (CurrentStamina > 0)
                    {
                        CurrentStamina -= RunningStaminaConsumption * Time.deltaTime;
                        yield return MMCoroutine.WaitForFrames(1);
                    }
                    else
                    {
                        _running = false;
                        BroadcastMessage(_runStopMethodName);
                    }
            }
        }
    }
}
