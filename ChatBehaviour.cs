using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using Mirror;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Written by Matthew Ihebom and Ibrahim Khan

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject Chat_UI = null;
    [SerializeField] private TMP_Text Chat_Text = null;
    [SerializeField] private TMP_InputField inputField = null;

    public Button button;

    public bool buttonPress;

   

    private static event Action<string> OnMessage;
    private string userName;
    void Start()
    {

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetUsernameResult, OnLoginFailure);

    }
    public override void OnStartAuthority()
    {
        Chat_UI.SetActive(true);
        OnMessage += HandleNewMessage;
    }

    [ClientCallback]

    private void OnDestroy()
    {
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        Chat_Text.text += message;
    }

    [Client]
    public void Send(string message)
    {
        
        if (string.IsNullOrWhiteSpace(message)) { return; }
        CmdSendMessage(message, userName);

        inputField.text = string.Empty;

    }
    public void OnGetUsernameResult(GetAccountInfoResult result)
    {
        userName = result.AccountInfo.Username;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error);
    }

    [Command(requiresAuthority = false)]
    private void CmdSendMessage(string message,string uName)
    {
        RpcHandleMessage($"[{uName}]: {message}");

    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n\n{message}");
    }

   
}
