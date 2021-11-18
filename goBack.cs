using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class goBack : MonoBehaviour
{
    public void backToMain()
    {
        SceneManager.LoadScene("MainMenu");
   

    }
}
