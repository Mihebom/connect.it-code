using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Written by Matthew Ihebom

public class ChatHost : MonoBehaviour
{
    public NetworkManager netMangager;

    public void Host()
    {
        
        netMangager.StartHost();
    }

    public void joinIP()
    {

        netMangager.networkAddress = "localhost";

    }

    public void Join()
    {
        netMangager.StartClient();
    }
}
