using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Project.Core.Cameras
{
    [Obsolete("Obsolete")]
    public class CameraSetup : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;

        void Awake()
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetupCamera(Transform target)
        {
            if (virtualCamera == null)
            {
                Debug.LogError("No CinemachineVirtualCamera found!");
                return;
            }

            // Set the follow target
            virtualCamera.Follow = target;

            // Get or add the 3rd person follow component
            var thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            if (thirdPersonFollow == null)
                thirdPersonFollow = virtualCamera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();

            // Configure the follow settings
            thirdPersonFollow.ShoulderOffset = new Vector3(0f, 5f, -7f); // Adjust these values
            thirdPersonFollow.CameraDistance = 10f; // Distance from target
            thirdPersonFollow.Damping = new Vector3(0.2f, 0.2f, 0.2f); // Damping for camera movement
            thirdPersonFollow.VerticalArmLength = 0.5f; // Vertical arm length

            // Configure aim settings for rotation
            var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer == null) composer = virtualCamera.AddCinemachineComponent<CinemachineComposer>();

            composer.m_DeadZoneWidth = 0.1f; // Width of dead zone
            composer.m_DeadZoneHeight = 0.1f; // Height of dead zone
            composer.m_SoftZoneWidth = 0.5f; // Width of soft zone
            composer.m_SoftZoneHeight = 0.5f; // Height of soft zone
            composer.m_TrackedObjectOffset = new Vector3(0, 0, 0); // Offset from target
        }
    }
}
