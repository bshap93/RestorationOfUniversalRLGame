using UnityEngine;

public class CameraApparat : MonoBehaviour
{
    // Make a singleton
    public static CameraApparat Instance;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // If the instance is not assigned
        if (Instance == null)
        {
            // Assign this instance to the singleton
            Instance = this;
            // Do not destroy this object when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy the object if the instance is already assigned
            Destroy(gameObject);
        }
    }
}
