using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Written by Matthew Ihebom

public class GoBack : MonoBehaviour
{
    public void backToMain()
    {
        SceneManager.LoadScene("MainMenu");
   

    }

 

    public void backToSelector()
    {

        SceneManager.LoadScene("BoardList");
    }
}
