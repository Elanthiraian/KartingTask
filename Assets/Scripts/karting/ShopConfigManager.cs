using UnityEngine;
using UnityEngine.UI;
using Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Networking;
using System.Collections;


public class ShopConfigManager : MonoBehaviour
{
    public List<Item> helmets;
    public List<Item> karts;
    public GameObject itemPrefab;
    public Transform contentParent;

    private void Start()
    {
        FetchShopData();
    }

    private void FetchShopData()
    {
        Debug.Log("Fetching shop data...");
        FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync().ContinueWithOnMainThread((Task<bool> task) =>
        {
            if (task.IsCompleted)
            {
                string json = FirebaseRemoteConfig.DefaultInstance.GetValue("shop_config_data").StringValue;
                Debug.Log("JSON string received: " + json);

                ParseShopDataJson(json);
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch shop data: " + task.Exception);
            }
        });
    }

    private void ParseShopDataJson(string json)
    {
        Dictionary<string, object> shopData = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
        if (shopData != null && shopData.ContainsKey("shop_data"))
        {
            Dictionary<string, object> shopDataDict = shopData["shop_data"] as Dictionary<string, object>;
            if (shopDataDict != null)
            {
                LoadItemsFromJson("helmets", shopDataDict, helmets);
                LoadItemsFromJson("karts", shopDataDict, karts);
            }
            else
            {
                Debug.LogError("shop_data key not found in JSON data");
            }
        }
        else
        {
            Debug.LogError("shop_data key not found in JSON data");
        }
    }

    private void LoadItemsFromJson(string itemType, Dictionary<string, object> shopDataDict, List<Item> itemList)
    {
        if (shopDataDict.ContainsKey(itemType))
        {
            List<object> itemsList = shopDataDict[itemType] as List<object>;
            if (itemsList != null)
            {
                itemList.Clear();
                foreach (var itemObj in itemsList)
                {
                    Dictionary<string, object> itemDict = itemObj as Dictionary<string, object>;
                    if (itemDict != null)
                    {
                        Item item = new Item();
                        item.name = itemDict["name"] as string;
                        item.price = Convert.ToInt32(itemDict["price"]);
                        itemList.Add(item);
                        CreateUIItem(item, itemType);
                        StartCoroutine(LoadImage(itemDict["url"] as string, item));
                    }
                }
            }
            else
            {
                Debug.LogError(itemType + " key not found or not an array in shop_data");
            }
        }
        else
        {
            Debug.LogError(itemType + " key not found in shop_data");
        }
    }

    private IEnumerator LoadImage(string url, Item item)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                item.sprite = sprite;

                GameObject itemPanel = FindUIItem(item.name);
                if (itemPanel != null)
                {
                    ShopItemUI shopItemUI = itemPanel.GetComponent<ShopItemUI>();
                    if (shopItemUI != null)
                    {
                        shopItemUI.SetItemImage(sprite);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to download image: " + www.error);
            }
        }
    }

    private GameObject FindUIItem(string itemName)
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            GameObject itemPanel = contentParent.GetChild(i).gameObject;
            ShopItemUI shopItemUI = itemPanel.GetComponent<ShopItemUI>();
            if (shopItemUI != null && shopItemUI.itemName == itemName)
            {
                return itemPanel;
            }
        }
        return null;
    }

    private void CreateUIItem(Item item, string itemType)
    {
        GameObject itemPanel = Instantiate(itemPrefab, contentParent);
        ShopItemUI shopItemUI = itemPanel.GetComponent<ShopItemUI>();
        if (shopItemUI != null)
        {
            shopItemUI.SetItemDetails(item.name, item.price);
            
        }
        else
        {
            Debug.LogError("ShopItemUI component not found in itemPrefab.");
        }
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int price;
    public Sprite sprite;
}
