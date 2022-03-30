using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

//Written by Matthew Ihebom

public class LeaveMatch : NetworkBehaviour
{
   
    public void exitMatch()
    {

        SceneManager.LoadScene("BoardList");
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();

    }


}
