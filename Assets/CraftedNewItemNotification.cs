using DG.Tweening;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.UI;

public class CraftedNewItemNotification : MonoBehaviour
{
    [SerializeField] DOTweenAnimation headerDotweenAnimation;
    [SerializeField] Image itemImage;
    [SerializeField] DOTweenAnimation itemImageDotweenAnimation;
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
        headerDotweenAnimation.endValueString = item.ItemName;
        headerDotweenAnimation.DORestart();
        headerDotweenAnimation.DOPlay();

        itemImage.sprite = item.Icon;
        itemImageDotweenAnimation.DORestart();
        itemImageDotweenAnimation.DOPlay();
    }

    public void Hide()
    {
        headerDotweenAnimation.DOPause();
        itemImageDotweenAnimation.DOPause();
    }
}
