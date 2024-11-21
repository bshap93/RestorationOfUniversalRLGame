using UnityEngine;

public class SaveStateManager : MonoBehaviour
{
    public static SaveStateManager Instance;

    [Tooltip("Is a valid save loaded?")] public bool IsSaveLoaded;

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
