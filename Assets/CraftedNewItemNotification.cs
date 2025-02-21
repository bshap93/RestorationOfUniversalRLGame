using DG.Tweening;
using MoreMountains.InventoryEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftedNewItemNotification : MonoBehaviour
{
    [SerializeField] DOTweenAnimation headerDotweenAnimation;
    [SerializeField] Image itemImage;
    [SerializeField] DOTweenAnimation itemImageDotweenAnimation;

    [SerializeField] TMP_Text itemName;
    [SerializeField] DOTweenAnimation itemNameDotweenAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void Hide()
    {
        headerDotweenAnimation.DOPause();
        itemImageDotweenAnimation.DOPause();
        itemNameDotweenAnimation.DOPause();
    }
}
