using System.Linq;
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
        if (other.CompareTag("Player")) _isInPlayerRange = true;
    }

    void InitializeInventory()
    {
        if (playerInventory == null)
        {
            playerInventory = Inventory.FindInventory("MainPlayerInventory", "Player1");
            if (playerInventory == null) Debug.LogError("Player inventory not found");
        }
    }
    public ContainerInventory GetInventory()
    {
        return containerInventory;
    }
}
