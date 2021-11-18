using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class serverConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update



    void Start()
    {

        
        PhotonNetwork.ConnectUsingSettings();
     
    }

  
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();
        Debug.Log("Connected to Photon: " + this);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {


        Debug.Log("Failed to connect to Photon: " + cause.ToString(), this);

    }

    public override void OnJoinedLobby()
    {

        SceneManager.LoadScene("protoLobby");
        Debug.Log("Joined lobby");

    }
}
