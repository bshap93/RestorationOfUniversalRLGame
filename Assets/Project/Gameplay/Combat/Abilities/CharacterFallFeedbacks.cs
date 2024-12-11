using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Fall Feedbacks")]
    public class CharacterFallFeedbacks : CharacterAbility
    {
        [Header("Feedbacks")] [Tooltip("The feedbacks to play when the character starts falling.")]
        public MMFeedbacks FallStartFeedbacks;

        [Tooltip(
            "The feedbacks to play when the character touches the ground. These feedbacks will change based on the distance of the fall.")]
        public MMFeedbacks GroundTouchFeedbacks;

        [Tooltip("The minimum fall distance to trigger the light landing feedbacks.")]
        public float LightLandingDistanceThreshold = 2f;

        [Tooltip("The minimum fall distance to trigger the heavy landing feedbacks.")]
        public float HeavyLandingDistanceThreshold = 5f;

        [Tooltip("The feedbacks to play for a light landing.")]
        public MMFeedbacks LightLandingFeedbacks;

        [Tooltip("The feedbacks to play for a heavy landing.")]
        public MMFeedbacks HeavyLandingFeedbacks;

        Vector3 _fallStartPosition;
        bool _isFalling;

        /// <summary>
        ///     On init we initialize feedbacks
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            FallStartFeedbacks?.Initialization(gameObject);
            GroundTouchFeedbacks?.Initialization(gameObject);
            LightLandingFeedbacks?.Initialization(gameObject);
            HeavyLandingFeedbacks?.Initialization(gameObject);
        }

        /// <summary>
        ///     This method monitors the character's state to detect when it starts falling and when it lands on the ground.
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_movement.CurrentState == CharacterStates.MovementStates.Falling && !_isFalling) StartFalling();

            if (_controller.JustGotGrounded && _isFalling) StopFalling();
        }

        /// <summary>
        ///     This method is triggered when the character starts falling.
        /// </summary>
        void StartFalling()
        {
            _isFalling = true;
            _fallStartPosition = transform.position;
            FallStartFeedbacks?.PlayFeedbacks(transform.position);
        }

        /// <summary>
        ///     This method is triggered when the character stops falling and lands on the ground.
        /// </summary>
        void StopFalling()
        {
            _isFalling = false;
            var fallDistance = Mathf.Max(0, _fallStartPosition.y - transform.position.y);

            if (fallDistance >= HeavyLandingDistanceThreshold)
                HeavyLandingFeedbacks?.PlayFeedbacks(transform.position);
            else if (fallDistance >= LightLandingDistanceThreshold && fallDistance < HeavyLandingDistanceThreshold)
                LightLandingFeedbacks?.PlayFeedbacks(transform.position);
            else
                GroundTouchFeedbacks?.PlayFeedbacks(transform.position);
        }

        /// <summary>
        ///     Reset the fall start position and stop all feedbacks if required.
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            _isFalling = false;
            _fallStartPosition = Vector3.zero;
            StopAllFeedbacks();
        }

        /// <summary>
        ///     Stops all feedbacks related to falling and landing.
        /// </summary>
        void StopAllFeedbacks()
        {
            FallStartFeedbacks?.StopFeedbacks();
            GroundTouchFeedbacks?.StopFeedbacks();
            LightLandingFeedbacks?.StopFeedbacks();
            HeavyLandingFeedbacks?.StopFeedbacks();
        }

        /// <summary>
        ///     Adds required animator parameters to the animator parameters list if they exist.
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            // Add any required animator parameters here if needed.
        }

        /// <summary>
        ///     Sends the current states to the animator.
        /// </summary>
        public override void UpdateAnimator()
        {
            // Update animator parameters if required.
        }

        /// <summary>
        ///     When the character dies, we reset everything.
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            ResetAbility();
        }

        /// <summary>
        ///     When the character respawns, we reset everything.
        /// </summary>
        protected override void OnRespawn()
        {
            base.OnRespawn();
            ResetAbility();
        }
    }
}
