using MoreMountains.TopDownEngine;

namespace Project.Gameplay.AI
{
    public class CustomChaseAction : AIActionMoveTowardsTarget3D
    {
        protected CharacterMovement _characterMovement;
        protected CharacterRun _characterRun;

        public override void Initialization()
        {
            base.Initialization();
            var character = gameObject.GetComponentInParent<Character>();
            _characterMovement = character?.FindAbility<CharacterMovement>();
            _characterRun = character?.FindAbility<CharacterRun>();
        }

        protected override void Move()
        {
            if (_characterRun != null)
            {
                _characterRun.enabled = true;      // Enable running
                _characterRun.RunStart();         // Start running
            }
            if (_characterMovement != null)
            {
                _characterMovement.enabled = false;  // Disable walking
            }
        
            base.Move();  // Call the original move logic
        }
    }
}
