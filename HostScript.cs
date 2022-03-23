using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HostScript : MonoBehaviour
{

    public NetworkManager networkManager;

    public void Host()
    {

        networkManager.StartHost();
    }

    public void joinIP(string joiningIP)
    {

        networkManager.networkAddress = joiningIP;

    }

    public void Join()
    {
        networkManager.StartClient();
    }


}
