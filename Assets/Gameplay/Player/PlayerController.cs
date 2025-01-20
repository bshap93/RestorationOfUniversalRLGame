using Project.Core.SaveSystem;
using UnityEngine;


namespace Project.Gameplay.Player
{
    public class NewPlayerController : MonoBehaviour, ISaveable
    {
        private CharacterController controller;
    
        // Movement settings
        [SerializeField] private float moveSpeed = 5f;
    
        void Start()
        {
            controller = GetComponent<CharacterController>();
        
            // TODO: Initialize these systems once implemented
            // InitializeInventory();
            // InitializeHealth();
            // InitializeInput();
        }

        void Update()
        {
            // Basic movement until input system is implemented
            float horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
            float vertical = UnityEngine.Input.GetAxisRaw("Vertical");
            Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * (moveSpeed * Time.deltaTime);
            controller.Move(movement);
        }

        public void SaveState(SaveData saveData)
        {
            // Save basic transform data
            saveData.playerData.position = transform.position;
            saveData.playerData.rotation = transform.rotation;
        
            // TODO: Save these once implemented
            // saveData.playerData.health = healthSystem.GetCurrentHealth();
            // saveData.playerData.inventory = inventory.GetInventoryData();
        }

        public void LoadState(SaveData saveData)
        {
            // Load basic transform data
            transform.position = saveData.playerData.position;
            transform.rotation = saveData.playerData.rotation;
        
            // TODO: Load these once implemented
            // healthSystem.SetHealth(saveData.playerData.health);
            // inventory.LoadInventoryData(saveData.playerData.inventory);
        }
    }
}
