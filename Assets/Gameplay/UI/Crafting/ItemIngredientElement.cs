using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemIngredientElement : MonoBehaviour
{
    [FormerlySerializedAs("_qtyText")] [SerializeField]
    TMP_Text qtyText;
    [FormerlySerializedAs("_itemImage")] [SerializeField]
    Image itemImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        qtyText = GetComponentInChildren<TMP_Text>();
        itemImage = GetComponentInChildren<Image>();
    }

    public void Initialize(Sprite img, int qty)
    {
        // Make alpha 1
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = img;
        qtyText.text = "x" + qty;
    }

    public void Nullify()
    {
        itemImage.sprite = null;
        itemImage.color = new Color(0, 0, 0, 0);
        qtyText.text = "";
    }
}
