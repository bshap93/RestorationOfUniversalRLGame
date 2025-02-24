using DG.Tweening;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using MoreMountains.InventoryEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Crafting
{
    public class CraftedNewItemNotification : MonoBehaviour, IURPNotification
    {
        [Header("Header")] [SerializeField] DOTweenAnimation headerDotweenAnimation;

        [Header("Item Image")] [SerializeField]
        Image itemImage;
        [SerializeField] DOTweenAnimation itemImageDotweenAnimation;

        [Header("Item Name")] [SerializeField] TMP_Text itemName;
        [SerializeField] DOTweenAnimation itemNameDotweenAnimation;

        [Header("Item Description")] [SerializeField]
        TMP_Text itemDescription;
        [SerializeField] DOTweenAnimation itemDescriptionDotweenAnimation;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Hide()
        {
            headerDotweenAnimation.DOPause();
            itemImageDotweenAnimation.DOPause();
            itemNameDotweenAnimation.DOPause();
            itemDescriptionDotweenAnimation.DOPause();
        }

        public void RestartWithNewItem(InventoryItem item)
        {
            headerDotweenAnimation.DORestart();
            headerDotweenAnimation.DOPlay();

            itemImage.sprite = item.Icon;
            itemImageDotweenAnimation.DORestart();
            itemImageDotweenAnimation.DOPlay();

            itemName.text = item.ItemName;
            itemNameDotweenAnimation.DORestart();
            itemNameDotweenAnimation.DOPlay();

            itemDescription.text = item.Description;
            itemDescriptionDotweenAnimation.DORestart();
            itemDescriptionDotweenAnimation.DOPlay();
        }
    }
}
