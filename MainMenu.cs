using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
}
