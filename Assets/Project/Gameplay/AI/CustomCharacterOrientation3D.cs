using MoreMountains.TopDownEngine;
using UnityEngine.AI;

namespace Project.Gameplay.AI
{
    public class CustomCharacterOrientation3D : CharacterOrientation3D
    {
        NavMeshAgent _navMeshAgent;

        protected override void Start()
        {
            base.Start();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (!CharacterRotationAuthorized || _navMeshAgent == null)
            {
                RotateToFaceMovementDirection();
                RotateToFaceWeaponDirection();
                RotateModel();
            }
        }
    }
}
