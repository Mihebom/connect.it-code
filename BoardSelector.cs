using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Written by Matthew Ihebom

public class BoardSelector : MonoBehaviour
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

    public void nigeriaLobby()
    {
        SceneManager.LoadScene("NigeriaLobby");
    }

    public void nigeriaBoard()
    {
        SceneManager.LoadScene("NigeriaBoard");
    }

    public void pakistanLobby()
    {
        SceneManager.LoadScene("PakistanLobby");
    }

    public void franceMatch()
    {
        SceneManager.LoadScene("FranceMatchMaking");
    }

    public void chinaMatch()
    {
        SceneManager.LoadScene("ChinaMatchMaking");
    }

    public void pakistanMatch()
    {
        SceneManager.LoadScene("PakistanMatchMaking");
    }

    public void nigeriaMatch()
    {
        SceneManager.LoadScene("NigeriaMatchMaking");
    }
}
