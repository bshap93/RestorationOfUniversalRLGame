using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Project.Gameplay.Navigation
{
    public class MoveToTarget : Action
    {
        EnemyNavMeshController _controller;
        public SharedGameObject target;

        public override void OnStart()
        {
            _controller = GetComponent<EnemyNavMeshController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null) return TaskStatus.Failure;

            _controller.SetDestination(target.Value.transform.position);
            return TaskStatus.Running;
        }
    }
}
