using MoreMountains.TopDownEngine;

namespace Gameplay.AI.Scripts
{
    public class CustomChaseAction : AIActionMoveTowardsTarget3D
    {
        protected CharacterMovement CharacterMovement;
        protected CharacterRun CharacterRun;

        public override void Initialization()
        {
            base.Initialization();
            var character = gameObject.GetComponentInParent<MoreMountains.TopDownEngine.Character>();
            CharacterMovement = character?.FindAbility<CharacterMovement>();
            CharacterRun = character?.FindAbility<CharacterRun>();
        }

        protected override void Move()
        {
            if (CharacterRun != null)
            {
                CharacterRun.enabled = true; // Enable running
                CharacterRun.RunStart(); // Start running
            }

            if (CharacterMovement != null) CharacterMovement.enabled = false; // Disable walking

            base.Move(); // Call the original move logic
        }
    }
}
