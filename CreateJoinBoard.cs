using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class CreateJoinBoard : MonoBehaviourPunCallbacks
{

    public TMP_InputField jInput;
    public TMP_InputField cInput;
   
    public void createRoom()
    {
        
        PhotonNetwork.CreateRoom(cInput.text);
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Connected to room: " + this);

    }

    public void joinRoom()
    {

        PhotonNetwork.JoinRoom(jInput.text);
        Debug.Log("Joined Level");
    }

    public override void OnJoinedRoom()
    {

        PhotonNetwork.LoadLevel("FranceBoard");
       
    }


}
