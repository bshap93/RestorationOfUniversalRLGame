using UnityEngine;

public class MinimizedPanelHandler : MonoBehaviour
{
    [Header("Minimize/Maximize Settings")] [Tooltip("Key to save minimize state in PlayerPrefs")]
    public GameObject minimizedPanel;
    public GameObject maximizedPanel;
    protected bool IsMinimized;

    void Start()
    {
        minimizedPanel.SetActive(IsMinimized);
        maximizedPanel.SetActive(!IsMinimized);
    }
    /// <summary>
    ///     Toggles between minimized and maximized states
    /// </summary>
    public virtual void ToggleMinimize()
    {
        IsMinimized = !IsMinimized;

        minimizedPanel.SetActive(IsMinimized);

        maximizedPanel.SetActive(!IsMinimized);

        if (IsMinimized)
            Debug.Log("Minimized");
        else
            Debug.Log("Maximized");
    }
}
