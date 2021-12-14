using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boardSelector : MonoBehaviour
{
    public void franceBoard()
    {
        SceneManager.LoadScene("FranceBoard");


    }

    public void chinaBoard()
    {
        SceneManager.LoadScene("ChinaBoard");


    }

    public void chinaLobby()
    {
        SceneManager.LoadScene("ChinaLobby");


    }
  
    public void franceLobby()
    {
        SceneManager.LoadScene("FranceLobby");


    }
}
