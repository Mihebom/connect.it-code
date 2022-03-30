using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

//Written by Matthew Ihebom

public class DisplayUserInfo : MonoBehaviour
{

    public static string userName { get; private set; }
    private const string playerPrefNameKey = "PlayerName";


    void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetUsernameResult, OnLoginFailure);
    }

    public void OnGetUsernameResult(GetAccountInfoResult result)
    {
        userName = result.AccountInfo.Username;
        PlayerPrefs.SetString(playerPrefNameKey, userName);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error);
    }
}
