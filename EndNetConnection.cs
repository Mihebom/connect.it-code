using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class EndNetConnection : MonoBehaviour
{

    public void back()
    {

        SceneManager.LoadScene("MainMenu");
        
        
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.StopHost();
    }
}
