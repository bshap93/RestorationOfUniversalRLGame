using UnityEngine;

public class PortableSystems : MonoBehaviour
{
    // Singleton pattern
    public static PortableSystems Instance { get; private set; }

    // Don't destroy this object when loading a new scene
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
