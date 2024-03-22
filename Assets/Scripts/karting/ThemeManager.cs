using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using static ShopManager;

public class ThemeManager : MonoBehaviour
{
    public Texture2D CurrentTheme;
    public RawImage Theme;

    public RawImage BannerImage;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Date;

    public AllEventConfigdata allEventConfigdata;
    public SummerEvent summerEvent;

    void Start()
    {
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

                string configData = remoteConfig.GetValue("event_data_config").StringValue;
                allEventConfigdata = JsonUtility.FromJson<AllEventConfigdata>(configData);
                summerEvent = allEventConfigdata.events_data.summer_events[0];
                Debug.Log("Total values: " + remoteConfig.AllValues.Count + remoteConfig.AllValues.Keys);

                foreach (var item in remoteConfig.AllValues)
                {
                    Debug.Log("Key :" + item.Key);
                    Debug.Log("Value: " + item.Value.StringValue);
                }
                OnSetData();
            });
    }

    private void OnSetData()
    {
        StartCoroutine(LoadImage( summerEvent.texture));
        Name.text = summerEvent.name;
        Date.text = $"{summerEvent.startTime} \nTo \n{summerEvent.endTime}";
    }

    IEnumerator DownloadImage(string url)
    {
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference imageRef = storage.GetReferenceFromUrl(url);

        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            // Send request and wait for a response
            yield return www.SendWebRequest();
            // Get downloaded texture
            Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
            CurrentTheme = texture;
            Theme.texture = CurrentTheme;
            BannerImage.texture = CurrentTheme;
        }
    }

    private IEnumerator LoadImage(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                CurrentTheme = texture;
                Theme.texture = CurrentTheme;
                BannerImage.texture = CurrentTheme;
            }
            else
            {
                Debug.LogError("Failed to download image: " + www.error);
            }
        }
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

[System.Serializable]
public class AllEventConfigdata
{
    public EventsData events_data;
}

[System.Serializable]
public class EventsData
{
    public List<SummerEvent> summer_events;
    public List<WinterEvent> winter_events;
}

[System.Serializable]
public class SummerEvent
{
    public string name;
    public string startTime;
    public string endTime;
    public string texture;
}

[System.Serializable]
public class WinterEvent
{
    public string name;
    public string startTime;
    public string endTime;
    public string texture;
}



