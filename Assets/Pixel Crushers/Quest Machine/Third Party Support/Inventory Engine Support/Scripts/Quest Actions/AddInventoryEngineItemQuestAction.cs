// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.InventoryEngineSupport;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Adds or removes items from an inventory.
    /// </summary>
    public class AddInventoryEngineItemQuestAction : QuestAction
    {

        [Tooltip("Name of inventory to add/remove item from.")]
        [SerializeField]
        private StringField m_inventoryName = new StringField();

        [Tooltip("Name of item to add or remove.")]
        [SerializeField]
        private StringField m_itemName = new StringField();

        [Tooltip("Amount to add or remove from the inventory. (Negative value removes from inventory.)")]
        [SerializeField]
        private QuestNumber m_amount = new QuestNumber();

        public StringField inventoryName
        {
            get { return m_inventoryName; }
            set { m_inventoryName = value; }
        }

        public StringField itemName
        {
            get { return m_itemName; }
            set { m_itemName = value; }
        }

        public QuestNumber amount
        {
            get { return m_amount; }
            set { m_amount = value; }
        }

        public override string GetEditorName()
        {
            var x = amount.GetValue(quest);
            return (x >= 0) ? ("Inventory Engine add " + x + " " + itemName + " to " + inventoryName) : ("Inventory Engine remove " + -x + " " + itemName + " from " + inventoryName);
        }

        public override void Execute()
        {
            base.Execute();
            var x = amount.GetValue(quest);
            if (x >= 0)
            {
                InventoryEngineUtils.mmAddItem(inventoryName.value, itemName.value, x);
            }
            else
            {
                InventoryEngineUtils.mmRemoveItem(inventoryName.value, itemName.value, -x);
            }
        }

    }
}
