using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class LeaveMatch : NetworkBehaviour
{
   
    public void exitMatch()
    {

        SceneManager.LoadScene("BoardList");
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();

    }


}
