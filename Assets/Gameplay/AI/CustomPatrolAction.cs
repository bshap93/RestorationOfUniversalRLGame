using MoreMountains.TopDownEngine;
using TopDownEngine.Common.Scripts.Characters.CharacterAbilities;

namespace Project.Gameplay.AI
{
    public class CustomPatrolAction : AIActionMovePatrol3D
    {
        protected CharacterMovement CharacterMovement;
        protected CharacterRun CharacterRun;

        protected override void Awake()
        {
            base.Awake();
            CharacterMovement = _character?.FindAbility<CharacterMovement>();
            CharacterRun = _character?.FindAbility<CharacterRun>();
        }

        protected override void Patrol()
        {
            if (CharacterMovement != null) CharacterMovement.enabled = true; // Enable walking
            if (CharacterRun != null)
            {
                CharacterRun.RunStop(); // Ensure not running
                CharacterRun.enabled = false;
            }

            base.Patrol(); // Call the original patrol logic
        }
    }
}
