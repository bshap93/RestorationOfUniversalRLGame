using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Tools
{
    public class CharacterHandleTorch : CharacterAbility
    {
        [Header("Torch Settings")] [Tooltip("The initial torch that the character starts with.")]
        public GameObject InitialTorch;

        [Tooltip("The attachment point for the torch.")]
        public Transform TorchAttachment;

        [Tooltip("Automatically bind the animator to the torch.")]
        public bool AutomaticallyBindAnimator = true;

        [Tooltip("The time (in seconds) before the torch burns out.")]
        public float TorchBurnTime = 60f;

        [Header("Torch Feedbacks")] public MMFeedbacks TorchLitFeedback;
        public MMFeedbacks TorchExtinguishedFeedback;
        public MMFeedbacks TorchBurnOutFeedback;
        protected Animator _animator;
        protected float _burnTimer;

        protected GameObject _currentTorch;
        protected bool _torchActive;

        protected override void Initialization()
        {
            base.Initialization();
            AssignAnimator();
            InitializeTorch();
        }

        protected virtual void AssignAnimator()
        {
            if (_animator == null && AutomaticallyBindAnimator)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null) _animator = GetComponentInChildren<Animator>();
            }
        }

        protected virtual void InitializeTorch()
        {
            if (InitialTorch != null) EquipTorch(InitialTorch);
        }

        public virtual void EquipTorch(GameObject newTorch)
        {
            if (_currentTorch != null) Destroy(_currentTorch);

            if (newTorch != null)
            {
                _currentTorch = Instantiate(newTorch, TorchAttachment.position, TorchAttachment.rotation);
                _currentTorch.transform.SetParent(TorchAttachment);
                _currentTorch.transform.localPosition = Vector3.zero;
                _currentTorch.transform.localRotation = Quaternion.identity;

                _burnTimer = TorchBurnTime;
                _torchActive = false;
            }
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || _currentTorch == null) return;

            // Activate or deactivate the torch using a button (e.g., 'T')
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                if (_torchActive)
                    ExtinguishTorch();
                else
                    LightTorch();
            }
        }

        protected virtual void LightTorch()
        {
            if (_torchActive) return;

            _torchActive = true;
            PlayAbilityStartFeedbacks();
            TorchLitFeedback?.PlayFeedbacks();

            Debug.Log("Torch lit!");
        }

        protected virtual void ExtinguishTorch()
        {
            if (!_torchActive) return;

            _torchActive = false;
            PlayAbilityStopFeedbacks();
            TorchExtinguishedFeedback?.PlayFeedbacks();

            Debug.Log("Torch extinguished!");
        }

        protected virtual void BurnOutTorch()
        {
            _torchActive = false;
            TorchBurnOutFeedback?.PlayFeedbacks();

            Debug.Log("Torch burned out!");
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_torchActive)
            {
                _burnTimer -= Time.deltaTime;
                if (_burnTimer <= 0) BurnOutTorch();
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            ExtinguishTorch();
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            EquipTorch(InitialTorch);
        }
    }
}
