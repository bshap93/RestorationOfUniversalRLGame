using MoreMountains.TopDownEngine;

namespace Project.Gameplay.AI
{
    public class CustomPatrolAction : AIActionMovePatrol3D
    {
        protected CharacterMovement _characterMovement;
        protected CharacterRun _characterRun;

        protected override void Awake()
        {
            base.Awake();
            _characterMovement = _character?.FindAbility<CharacterMovement>();
            _characterRun = _character?.FindAbility<CharacterRun>();
        }

        protected override void Patrol()
        {
            if (_characterMovement != null) _characterMovement.enabled = true; // Enable walking
            if (_characterRun != null)
            {
                _characterRun.RunStop(); // Ensure not running
                _characterRun.enabled = false;
            }

            base.Patrol(); // Call the original patrol logic
        }
    }
}
