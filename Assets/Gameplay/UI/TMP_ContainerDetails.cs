using Gameplay.ItemManagement.InventoryTypes.Storage;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMP_ContainerDetails : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public TMP_Text Title;
    public TMP_Text ContainerID;
    public TMP_Text ContainerType;
    public TMP_Text ContainerSize;

    public Image Icon;

    // Defaults
    public string DefaultTitle = "Container Details";
    public string DefaultContainerID = "Container ID: ";
    public string DefaultContainerType = "Container Type: ";
    public string DefaultContainerSize = "Container Size: ";

    public string PreviewEventName = "PreviewContainer";

    CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMGameEvent mmEvent)
    {
        if (mmEvent.EventName == PreviewEventName) gameObject.SetActive(true);
    }

    public void DisplayPreview(ContainerSO containerSO)
    {
        if (containerSO == null)
        {
            FillWithDefaults();
            return;
        }

        Title.text = containerSO.ContainerName;
        ContainerID.text = DefaultContainerID + containerSO.ContainerID;
        ContainerType.text = DefaultContainerType + containerSO.ContainerType;
        ContainerSize.text = DefaultContainerSize + containerSO.ContainerSize;
        Icon.sprite = containerSO.Icon;

        Show();
    }

    public void Show()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    void FillWithDefaults()
    {
        Title.text = DefaultTitle;
        ContainerID.text = DefaultContainerID;
        ContainerType.text = DefaultContainerType;
        ContainerSize.text = DefaultContainerSize;
        Icon.sprite = null;
    }
}
