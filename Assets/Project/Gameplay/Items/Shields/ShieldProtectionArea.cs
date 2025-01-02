using System;
using UnityEngine;

namespace Project.Gameplay.Combat.Shields
{
    [Serializable]
    public class ShieldProtectionArea : MonoBehaviour
    {
        [Tooltip("The angle within which the shield blocks damage")]
        public float BlockAngle = 90f; // Blocking arc in degrees

        [Tooltip("The forward direction of the shield (relative to its rotation)")]
        public Transform ShieldForward;

        [Tooltip("Is the shield currently raised?")]
        public bool ShieldIsActive;

        [Tooltip("The distance the shield Gizmo should extend")]
        public float GizmoDistance = 2f;

        void Start()
        {
            OnShieldEquipped?.Invoke(this);
        }

        void OnDrawGizmos()
        {
            if (ShieldForward == null) ShieldForward = transform;

            // Draw the forward direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, ShieldForward.forward * GizmoDistance);

            // Draw the block angle arc
            Gizmos.color = new Color(0, 1, 0, 0.2f); // Semi-transparent green
            DrawArc(transform.position, ShieldForward.forward, BlockAngle, GizmoDistance);
        }

        /// <summary>
        ///     Draws an arc representing the shield's block angle.
        /// </summary>
        void DrawArc(Vector3 center, Vector3 forward, float angle, float radius)
        {
            var segments = 50; // Number of segments in the arc
            var step = angle / segments;
            var lastPoint = center + Quaternion.Euler(0, -angle / 2, 0) * forward * radius;

            for (var i = 1; i <= segments; i++)
            {
                var currentAngle = -angle / 2 + step * i;
                var nextPoint = center + Quaternion.Euler(0, currentAngle, 0) * forward * radius;

                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }


        public static event Action<ShieldProtectionArea> OnShieldEquipped;

        public bool IsBlocking(Vector3 attackPosition)
        {
            if (!ShieldIsActive) return false;

            if (ShieldForward == null) ShieldForward = transform; // Default to the shield's transform

            // Calculate the direction from the attack to the shield
            var attackDirection = (attackPosition - transform.position).normalized;

            // Check the angle between the attack direction and the shield's forward direction
            var angle = Vector3.Angle(ShieldForward.forward, attackDirection);

            // If BlockAngle is 360, always block; otherwise, compare
            return BlockAngle >= 360 || angle <= BlockAngle / 2;
        }
    }
}
