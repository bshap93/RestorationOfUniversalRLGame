using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryTypes.Storage
{
    
    public enum ContainerType
    {
        Chest,

    }
    /// </summary>
    [CreateAssetMenu(
        fileName = "ContainerSO", menuName = "Storage/Container", order = 1)]
    public class ContainerSO : ScriptableObject
    {
        public string ContainerID;
        
        public string SourceInventoryName;
        
        public string TargetInventoryName;
        
        public string ContainerInventoryName;
        
        public int ContainerSize;
        
        public ContainerType ContainerType;
        
        public string ContainerName;
        
        
        public Sprite Icon;
        
        public MMFeedbacks InteractionFeedbacks;
        
        protected Inventory _containerInventory;
        

        
    }
}
