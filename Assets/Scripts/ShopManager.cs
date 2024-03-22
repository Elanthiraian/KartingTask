using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    FirebaseFirestore dp;

    public GameObject prefab;
    public Transform parent;
    public AllShopData allConfigData;

    void Start()
    {
        dp = FirebaseFirestore.DefaultInstance;
        CheckRemoteConfigValues();
    }

    public Task CheckRemoteConfigValues()
    {
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
            return;

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

                string configData = remoteConfig.GetValue("shop_configdata").StringValue;
                allConfigData = JsonUtility.FromJson<AllShopData>(configData);

                Debug.Log("Total values: " + remoteConfig.AllValues.Count + remoteConfig.AllValues.Keys);

                foreach (var item in remoteConfig.AllValues)
                {
                    Debug.Log("Key :" + item.Key);
                    Debug.Log("Value: " + item.Value.StringValue);
                }
            });
    }


    IEnumerator DownloadImage(string url, RawImage rawImage)
    {
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference imageRef = storage.GetReferenceFromUrl(url);

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            // Send request and wait for a response
            yield return www.SendWebRequest();
            // Get downloaded texture
            Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
            rawImage.texture = texture;
        }
    }

[Serializable]
public class AllShopData
{
    public shop_data shop_data;
}

[Serializable]
public class shop_data
{
    public helmets[] helmets;
    public karts[] karts;
}

[Serializable]
public class helmets
{
    public string name;
    public float url;
    public int price;
}

[Serializable]
public class karts
{
    public string name;
    public float url;
    public int price;
}
}
