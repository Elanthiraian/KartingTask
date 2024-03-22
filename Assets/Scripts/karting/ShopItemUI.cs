using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image itemImage;
    public string itemName;

    public void SetItemDetails(string itemName, int itemPrice)
    {
        this.itemName = itemName; 
        nameText.text = itemName;
        priceText.text = "$" + itemPrice.ToString();
    }
    public void SetItemImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
    }

}
