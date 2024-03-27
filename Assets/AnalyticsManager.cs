using UnityEngine;
using UnityEngine.UI;
using Firebase.Analytics;
using System;
using Firebase;
using Firebase.Extensions;

public class AnalyticsManager : MonoBehaviour
{
    public Text scoreText;
    protected bool firebaseInitialized = false;
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;


    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        FirebaseAnalytics.SetUserProperty(
          FirebaseAnalytics.UserPropertySignUpMethod,
          "Google");
        FirebaseAnalytics.SetUserId("user_510");
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        firebaseInitialized = true;
    }

    public void ScoredPoints(int points)
    {
        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPostScore, FirebaseAnalytics.ParameterScore,
             points);
    }

   
    public void CompletedLevel(int levelNumber)
    {

        // Trigger Firebase Analytics event
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelUp, "level_number", levelNumber);
    }

    
    public void PurchasedItem(string itemName, float itemPrice)
    {
        // Trigger Google Analytics event

        // Trigger Firebase Analytics event
        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEcommercePurchase, "item_name", itemName, "item_price", itemPrice.ToString());
    }

    public void TrackShopButtonClick()
    {
        // Log event to Firebase Analytics
        FirebaseAnalytics.LogEvent("Shop_button_Clicked");
        //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectContent, new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterItemName, "Shop_Button_Click"));
    }
}
