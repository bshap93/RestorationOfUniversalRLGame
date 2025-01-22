using System.Linq;
using Core.Events;
using Gameplay.ItemManagement.InventoryTypes;
using Gameplay.ItemManagement.InventoryTypes.Storage;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using TMPro;
using UnityEngine;

public class ContainerController : MonoBehaviour
{
    public Inventory playerInventory;
    public ContainerInventory containerInventory;

    public ContainerSO ContainerSO;

    public GameObject previewPanel;
    public TextMeshProUGUI previewText;

    public MMFeedbacks interactFeedbacks;

    readonly bool _isOpen = false;

    bool _isInPlayerRange;

    void Start()
    {
        InitializeInventory();
        if (previewPanel != null) previewPanel.SetActive(false);

        // Inventory is empty?
        if (!containerInventory.Content.Any(item => item != null)) Debug.Log("Container is empty");
    }

    void Update()
    {
        if (_isInPlayerRange && Input.GetKeyDown(KeyCode.F))
            if (playerInventory == null)
                InitializeInventory();

        // If something is to be done when f is pressed, add it here
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayerRange = true;

            if (ContainerSO.ContainerID == null)
            {
                Debug.LogError("Container ID is null");
                return;
            }

            ContainerEvent.Trigger("ContainerInRange", ContainerEventType.ContainerInRange, this);

            ShowPreview("Press F to take all.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayerRange = false;
            ContainerEvent.Trigger("ContainerOutOfRange", ContainerEventType.ContainerOutOfRange, this);
            HidePreview();
        }
    }

    void InitializeInventory()
    {
        if (playerInventory == null)
        {
            playerInventory = Inventory.FindInventory("MainPlayerInventory", "Player1");
            if (playerInventory == null) Debug.LogError("Player inventory not found");
        }
    }

    public void OnSelectedItem()
    {
        Debug.Log("ContainerSelected event triggered");
        ContainerEvent.Trigger("ContainerSelected", ContainerEventType.ContainerSelected, this);
    }

    public void OnUnSelectedItem()
    {
        ContainerEvent.Trigger("ContainerDeselected", ContainerEventType.ContainerDeselected, this);
    }
    public ContainerInventory GetInventory()
    {
        return containerInventory;
    }

    public void ShowPreview(string text)
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(true);
            previewText.text = text;
        }
        else
        {
            Debug.LogError("Preview panel is null");
        }
    }

    public void HidePreview()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }
}
