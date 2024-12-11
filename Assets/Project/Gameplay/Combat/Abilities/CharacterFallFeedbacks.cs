using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Abilities
{
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Fall Feedbacks")]
    public class CharacterFallFeedbacks : CharacterAbility
    {
        [Header("Feedbacks")]
        [Tooltip("The feedbacks to play when the character starts falling.")]
        public MMFeedbacks FallStartFeedbacks;
        
        [Tooltip("The feedbacks to play when the character touches the ground. These feedbacks will change based on the distance of the fall.")]
        public MMFeedbacks GroundTouchFeedbacks;

        [Tooltip("The minimum fall distance to trigger the light landing feedbacks.")]
        public float LightLandingDistanceThreshold = 2f;

        [Tooltip("The minimum fall distance to trigger the heavy landing feedbacks.")]
        public float HeavyLandingDistanceThreshold = 5f;

        [Tooltip("The feedbacks to play for a light landing.")]
        public MMFeedbacks LightLandingFeedbacks;

        [Tooltip("The feedbacks to play for a heavy landing.")]
        public MMFeedbacks HeavyLandingFeedbacks;

        private Vector3 _fallStartPosition;
        private bool _isFalling;

        /// <summary>
        /// On init we initialize feedbacks
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            FallStartFeedbacks?.Initialization(this.gameObject);
            GroundTouchFeedbacks?.Initialization(this.gameObject);
            LightLandingFeedbacks?.Initialization(this.gameObject);
            HeavyLandingFeedbacks?.Initialization(this.gameObject);
        }

        /// <summary>
        /// This method monitors the character's state to detect when it starts falling and when it lands on the ground.
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_movement.CurrentState == CharacterStates.MovementStates.Falling && !_isFalling)
            {
                StartFalling();
            }

            if (_controller.JustGotGrounded && _isFalling)
            {
                StopFalling();
            }
        }

        /// <summary>
        /// This method is triggered when the character starts falling.
        /// </summary>
        private void StartFalling()
        {
            _isFalling = true;
            _fallStartPosition = this.transform.position;
            FallStartFeedbacks?.PlayFeedbacks(this.transform.position);
        }

        /// <summary>
        /// This method is triggered when the character stops falling and lands on the ground.
        /// </summary>
        private void StopFalling()
        {
            _isFalling = false;
            float fallDistance = _fallStartPosition.y - this.transform.position.y;

            if (fallDistance > HeavyLandingDistanceThreshold)
            {
                HeavyLandingFeedbacks?.PlayFeedbacks(this.transform.position);
            }
            else if (fallDistance > LightLandingDistanceThreshold)
            {
                LightLandingFeedbacks?.PlayFeedbacks(this.transform.position);
            }
            else
            {
                GroundTouchFeedbacks?.PlayFeedbacks(this.transform.position);
            }
        }

        /// <summary>
        /// Reset the fall start position and stop all feedbacks if required.
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            _isFalling = false;
            _fallStartPosition = Vector3.zero;
            StopAllFeedbacks();
        }

        /// <summary>
        /// Stops all feedbacks related to falling and landing.
        /// </summary>
        private void StopAllFeedbacks()
        {
            FallStartFeedbacks?.StopFeedbacks();
            GroundTouchFeedbacks?.StopFeedbacks();
            LightLandingFeedbacks?.StopFeedbacks();
            HeavyLandingFeedbacks?.StopFeedbacks();
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist.
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            // Add any required animator parameters here if needed.
        }

        /// <summary>
        /// Sends the current states to the animator.
        /// </summary>
        public override void UpdateAnimator()
        {
            // Update animator parameters if required.
        }

        /// <summary>
        /// When the character dies, we reset everything.
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            ResetAbility();
        }

        /// <summary>
        /// When the character respawns, we reset everything.
        /// </summary>
        protected override void OnRespawn()
        {
            base.OnRespawn();
            ResetAbility();
        }
    }
}
