using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Written by Matthew Ihebom

public class MainMenu : MonoBehaviour
{

  public void boardList()
    {
        SceneManager.LoadScene("BoardList");
    }

    public void leaderBoards()
    {
        SceneManager.LoadScene("LeaderBoard");
    }
    public void userProfiles()
    {
        SceneManager.LoadScene("PlayerProfile");
    }

  
    public void userLogin()
    {
        SceneManager.LoadScene("UserLogin");
    }

    public void userRegister()
    {
        SceneManager.LoadScene("UserRegister");
    }

    public void joinChat()
    {
        SceneManager.LoadScene("Join-HostChat");
    }

    public void toInventory()
    {
        SceneManager.LoadScene("userInventory");    
    }

    public void shop()
    {
        SceneManager.LoadScene("PlayerStore");
    }

    public void test()
    {
        SceneManager.LoadScene("LobbyMenu");
    }
}
