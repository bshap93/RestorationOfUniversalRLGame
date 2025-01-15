// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;

namespace PixelCrushers.QuestMachine
{

    [AddComponentMenu("Pixel Crushers/Quest Machine/Third Party/Inventory Engine/Inventory Engine Reward System")]
    public class InventoryEngineRewardSystem : RewardSystem
    {
        [Serializable]
        public class RewardItem
        {
            [Tooltip("Item that can be offered as a reward.")]
            public InventoryItem item;

            [Tooltip("Reward system point value of item.")]
            public int pointValue;
        }

        [Tooltip("Items to offer.")]
        [SerializeField]
        private List<RewardItem> m_items = new List<RewardItem>();
        public List<RewardItem> items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        [Tooltip("Inventory to add reward items to.")]
        [SerializeField]
        private StringField m_inventoryName = new StringField();
        public StringField inventoryName
        {
            get { return m_inventoryName; }
            set { m_inventoryName = value; }
        }

        // The quest generator will call this method to try to use up points
        // by choosing rewards to offer.
        public override int DetermineReward(int points, Quest quest)
        {
            var successInfo = quest.GetStateInfo(QuestState.Successful);

            // Offer items:
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var rewardItem = items[i];
                var itemValue = Mathf.Max(1, rewardItem.pointValue);
                if (itemValue <= points)
                {
                    items.Remove(rewardItem);

                    // Add some UI content to the quest's offerContentList:
                    if (rewardItem.item.Icon == null)
                    {
                        var itemText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
                        itemText.bodyText = new StringField(rewardItem.item.ItemName);
                        quest.offerContentList.Add(itemText);
                    }
                    else
                    {
                        var itemIcon = IconQuestContent.CreateInstance<IconQuestContent>();
                        itemIcon.image = rewardItem.item.Icon;
                        itemIcon.count = 1;
                        itemIcon.caption = new StringField(rewardItem.item.ItemName);
                        quest.offerContentList.Add(itemIcon);
                    }

                    // Add an InventoryProItemQuestAction action to the quest's Successful state:
                    var itemAction = AddInventoryEngineItemQuestAction.CreateInstance<AddInventoryEngineItemQuestAction>();
                    itemAction.inventoryName = new StringField(inventoryName);
                    itemAction.itemName = new StringField(rewardItem.item.ItemName);
                    itemAction.amount = new QuestNumber(1);
                    successInfo.actionList.Add(itemAction);

                    // Reduce points left:
                    points -= itemValue;
                    if (points <= 0) return 0;
                }
            }

            return points;
        }

    }
}
