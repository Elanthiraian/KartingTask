using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Google.Protobuf.WellKnownTypes;
using Firebase.Firestore;
using Firebase.Extensions;

public class WalletManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _WalletText;
    private string Wallet = "0";
    public AnalyticsManager AnalyticsManager;

    public void Start()
    {
        OnUpdateWalletData();
    }

    private void OnUpdateWalletData()
    {
        if (PlayerPrefs.HasKey("Wallet"))
        {
            Wallet = PlayerPrefs.GetInt("Wallet").ToString();
        }
        _WalletText.text = $"Coin : \n {Wallet}";
        AnalyticsManager.ScoredPoints(int.Parse(Wallet));
    }
}
