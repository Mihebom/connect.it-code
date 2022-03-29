using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using UnityEngine.UI;
using TMPro;

public class HostScript : MonoBehaviour
{

    public NetworkManager networkManager;

    public void Host()
    {
        networkManager.StartHost();
    }
    public void Join()
    {
        
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }


}
