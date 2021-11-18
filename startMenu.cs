using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class startMenu : MonoBehaviourPunCallbacks
{

    public void startGame()
    {
        SceneManager.LoadScene("FranceBoard");


    }
    
    public void comingSoon()
    {

        SceneManager.LoadScene("ComingSoon");

    }

    public void multiPlayer()
    {

        SceneManager.LoadScene("LoadScreen");

    }

    public void returnToMain()
    {

        SceneManager.LoadScene("MainMenu");
        PhotonNetwork.Disconnect();
    }

}
