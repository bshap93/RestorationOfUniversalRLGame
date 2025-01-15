// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using PixelCrushers.InventoryEngineSupport;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Checks an inventory for a required quantity of an item.
    /// </summary>
    public class InventoryEngineHasItemsQuestCondition : QuestCondition, MMEventListener<MMInventoryEvent>
    {

        [Tooltip("Name of inventory to check.")]
        [SerializeField]
        private StringField m_inventoryName = new StringField();

        [Tooltip("Name of item to check.")]
        [SerializeField]
        private StringField m_itemName = new StringField();

        [Tooltip("How the required value applies to the item count.")]
        [SerializeField]
        private CounterValueConditionMode m_requiredValueMode = CounterValueConditionMode.AtLeast;

        [Tooltip("Required item count.")]
        [SerializeField]
        private QuestNumber m_requiredValue = new QuestNumber();

        [Tooltip("If assigned, keep this quest counter updated while waiting for this condition to be true. Inspect the quest's main info to view/edit counters.")]
        [SerializeField]
        private int m_counterIndex;

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

        public CounterValueConditionMode requiredValueMode
        {
            get { return m_requiredValueMode; }
            set { m_requiredValueMode = value; }
        }

        public QuestNumber requiredValue
        {
            get { return m_requiredValue; }
            set { m_requiredValue = value; }
        }

        public int counterIndex
        {
            get { return m_counterIndex; }
            set { m_counterIndex = value; }
        }

        private QuestCounter counter { get; set; }

        public override string GetEditorName()
        {
            return "Inventory Engine has " + requiredValueMode + " " + requiredValue.GetValue(quest) + " " + itemName + " in " + inventoryName;
        }

        public override void StartChecking(System.Action trueAction)
        {
            if (quest == null) return;
            base.StartChecking(trueAction);
            counter = quest.GetCounter(counterIndex);
            UpdateItemCount();
            if (isChecking)
            {
                this.MMEventStartListening<MMInventoryEvent>();
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            this.MMEventStopListening<MMInventoryEvent>();
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            switch (eventType.InventoryEventType)
            {
                case MMInventoryEventType.ContentChanged:
                    UpdateItemCount();
                    break;
            }
        }

        private void UpdateItemCount()
        {
            var itemCount = GetItemCount();
            UpdateCounterValue(itemCount);
            if (IsItemCountRequirementMet(itemCount))
            {
                SetTrue();
            }
        }

        private int GetItemCount()
        {
            return InventoryEngineUtils.mmGetQuantity(inventoryName.value, itemName.value);
        }

        private bool IsItemCountRequirementMet(int itemCount)
        {
            
            switch (requiredValueMode)
            {
                case CounterValueConditionMode.AtLeast:
                    return itemCount >= requiredValue.GetValue(quest);
                case CounterValueConditionMode.AtMost:
                    return itemCount <= requiredValue.GetValue(quest);
                default:
                    return false;
            }            
        }

        private void UpdateCounterValue(int itemCount)
        {
            if (counter != null) counter.currentValue = itemCount;
        }

    }
}
