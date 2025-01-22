using UnityEngine;

public class ChestPanelController : MonoBehaviour
{
    public GameObject chestPanelPrefab;
    public GameObject chestPanel;
    ChestPanelInstance _chestPanelInstance;


    void ShowPanel()
    {
        var canvasGroup = chestPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log("ShowPanel called, setting canvasGroup.alpha to 1");
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
