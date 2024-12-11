using UnityEngine;

public class CamerasTopLevel : MonoBehaviour
{
    // Singleton instance
    public static CamerasTopLevel Instance { get; private set; }

    // Do not destroy on load
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
