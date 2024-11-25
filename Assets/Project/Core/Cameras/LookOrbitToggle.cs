using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class LookOrbitController : MonoBehaviour
{
    [FormerlySerializedAs("OrbitalFollow")] [Tooltip("Reference to the CinemachineOrbitalFollow component")]
    public CinemachineInputAxisController cinemachineInputAxisController;

    [Tooltip("Value to set when Look Orbit X is enabled")]
    public float EnabledValue = 10f;

    [Tooltip("Value to set when Look Orbit X is disabled")]
    public float DisabledValue;

    [Tooltip("Whether recentering should be enabled when Look Orbit X is active")]
    public bool EnableRecenteringWhenActive = true;

    void Awake()
    {
        // Ensure the OrbitalFollow component is assigned
        if (cinemachineInputAxisController == null)
        {
            cinemachineInputAxisController = GetComponent<CinemachineInputAxisController>();
            

            if (cinemachineInputAxisController == null)
                Debug.LogError("CinemachineOrbitalFollow component not found on this GameObject!");
        }
    }

    /// <summary>
    ///     Toggles Look Orbit X and sets relevant properties dynamically.
    /// </summary>
    /// <param name="isEnabled">True to enable Look Orbit X, false to disable.</param>
    public void ToggleLookOrbitX(bool isEnabled)
    {




    }
}
